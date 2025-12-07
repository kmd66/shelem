using shelemApi.Helper;
using System;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace shelemApi.Models;

public class RoomProperty0
{
    protected static Random _random = new();

    public Guid Id { get; set; }
    public dynamic Info { get; set; }
    public List<RoomUser> Users { get; set; }
    public DateTime CreatedAt { get; set; }



    public GameState State = GameState.Reading;

    public CancellationTokenSource StartToken;
    public CancellationTokenSource FinishToken;
    public CancellationTokenSource ActionToken;

    public DateTime StartGameAt = DateTime.Now;

    public List<long> InitStrat;

    public bool IsStart => StartToken != null && StartToken.Token.IsCancellationRequested;
    public bool IsFinish => FinishToken != null && FinishToken.Token.IsCancellationRequested;
    public long Tourner = 0; // نوبت

    public List<byte> OflineCounts => [FirstOflineCount, SecondOflineCount];
    public byte FirstOflineCount = 0;
    public byte SecondOflineCount = 0;
    public bool CheckOflineCount => FirstOflineCount>2 || SecondOflineCount > 2;
    public void SetOflineCount()
    {
        var user = Users.FirstOrDefault(x => x.Id == Tourner);
        if (user == null) return;
        if (user.FirstUser) FirstOflineCount++;
        else SecondOflineCount++;
        actionTimeRatio = 3;
    }

    public int StartWait = 60;
    public int GameTime = 30 * 60;
    private readonly int actionTime = 14;
    public int ReadingTime = 30;
    public int BurningTime = 45;

    public int actionTimeRatio = 1;
    public int ActionTime => actionTime * actionTimeRatio;

    public int MinReading = 0;
    public long HakemUserId = 0;
    public byte HokmSuit = 0;
    public bool IsHakemFirstUser => HakemUserId > 0 ? Users.First(x => x.Id == HakemUserId).FirstUser : true;

    public List<Card> Cards1 = [];
    public List<Card> Cards2 = [];

    public List<Card> CardsGround0 = [];
    public List<List<Card>> CardsGround1 = [];
    public List<List<Card>> CardsGround2 = [];

    public List<CardGroup> CardGroup1 = [];
    public List<CardGroup> CardGroup2 = [];

    public List<Card> CardsWinning1 = [];
    public List<Card> CardsWinning2 = [];

    public List<Card> CardsMain = [];

    public int TotalScore1 = 0;
    public int TotalScore2 = 0;
    public bool CheckTotalScore => TotalScore1 >= 330 || TotalScore2 >= 330;
    public List<int> TotalScore => [TotalScore1, TotalScore2];

    public void dispose()
    {
        try
        {
            FinishToken?.Cancel();
            StartToken?.Cancel();
            ActionToken?.Cancel();
        }
        finally
        {
            FinishToken?.Dispose();
            StartToken?.Dispose();
            ActionToken?.Dispose();
        }
    }
}


public class RoomProperty: RoomProperty0
{
    public RoomProperty(Guid roomId, List<RoomUser> users) 
    {
        Id = roomId;
        Users = users;
    }

    public event Func<string, string, object, Task> NotifyUserAsync;
    public event Func<IEnumerable<string>, string, object, Task> NotifyUsersAsync;
    public event Action<Guid> Remove;
    public event Func<FinishModelRequest, List<byte>, Task> Reload;

    protected Task OnUserAsync(string conectionId, string eventName, object data)
        => NotifyUserAsync?.Invoke(conectionId, eventName, data) ?? Task.CompletedTask;
    private Task OnNotifyUsersAsync(IEnumerable<string> connectionIds, string eventName, object data)
        => NotifyUsersAsync?.Invoke(connectionIds, eventName, data) ?? Task.CompletedTask;

    //protected Task OnReceiveMainTime(int i)
    //    => NotifyUsersAsync?.Invoke(Users.Select(x => x.ConnectionId), "ReceiveMainTime", new
    //    {
    //        shotTime,
    //        i,
    //        tourner,
    //    }) ?? Task.CompletedTask;

    public Task InitEvent(string eventName)
        => NotifyUsersAsync?.Invoke(Users.Select(x => x.ConnectionId), eventName, true);

    public async Task ReceiveCansel()
    {
        await NotifyUsersAsync?.Invoke(Users.Select(x => x.ConnectionId), "ReceiveCansel", true);
        var url = $"{AppStrings.ApiUrl}/resultGame/cancelRoom";
        var cancelModelRequest = new CancelModelRequest(AppStrings.ApiKey, Id);
        var result = await new Helper.AppRequest().Post(cancelModelRequest, url);
        Remove?.Invoke(Id);
    }

    public async Task ReceivePhysicsStandard()
    {
        await Task.Delay(50);
        var data = new
        {
        }.ToJson();

        await NotifyUsersAsync?.Invoke(Users.Select(x => x.ConnectionId), "ReceivePhysicsStandard", data);
    }

    public async Task InitReading()
    {
        await Task.Delay(50);
        var data = new
        {
            MinReading,
            Tourner,
            ReadingTime
        };

        await NotifyUsersAsync?.Invoke(Users.Select(x => x.ConnectionId), "InitReading", data);
    }
    public async Task InitBurning()
    {
        await Task.Delay(50);
        var data = new
        {
            BurningTime,
            Tourner,
        };
        await NotifyUsersAsync?.Invoke(Users.Select(x => x.ConnectionId), "InitBurning", data);
    }
    public async Task InitGameAction()
    {
        await Task.Delay(50);
        var data = new
        {
            ActionTime,
            Tourner,
        };
        await NotifyUsersAsync?.Invoke(Users.Select(x => x.ConnectionId), "InitGameAction", data);
    }


    public async Task ReceiveHakem()
    {
        await Task.Delay(50);
        var data = new
        {
            MinReading,
            IsHakemFirstUser
        };
        await NotifyUsersAsync?.Invoke(Users.Select(x => x.ConnectionId), "ReceiveHakem", data);
    }
    public Task ReceiveHokm()=> NotifyUsersAsync?.Invoke(Users.Select(x => x.ConnectionId), "ReceiveHokm", HokmSuit);

    public async Task ReceiveCards()
    {
        await Task.Delay(50);
        var data1 = new
        {
            rivaCards = Cards2.Count,
            rivaCardGroup = CardGroup2,
            cards = Cards1,
            cardGroup = CardGroup1,
            CardsMain
        }.ToJson();
        var data2 = new
        {
            rivaCards = Cards1.Count,
            rivaCardGroup = CardGroup1,
            cards = Cards2,
            cardGroup = CardGroup2,
            CardsMain
        }.ToJson();

        _ = NotifyUserAsync?.Invoke(Users.First(x=> x.FirstUser).ConnectionId, "ReceiveCards", data1);
        _ = NotifyUserAsync?.Invoke(Users.First(x=> !x.FirstUser).ConnectionId, "ReceiveCards", data2);
    }
}
