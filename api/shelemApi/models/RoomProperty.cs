using shelemApi.Helper;
using System;

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

    public DateTime? StartGameAt;

    public List<long> InitStrat;

    public bool IsStart => StartToken != null ? StartToken.Token.IsCancellationRequested : false;
    public bool IsFinish => FinishToken != null ? FinishToken.Token.IsCancellationRequested : false;
    public long Tourner = 0; // نوبت

    public List<byte> OflineCounts => [FirstOflineCount, SecondOflineCount];
    public byte FirstOflineCount = 0;
    public byte SecondOflineCount = 0;

    public int StartWait = 60;
    public int GameTime = 30 * 60;
    public int ActionTime = 14;
    public int ReadingTime = 30;

    public int MinReading = 0;
    public long HakemUserId = 0;
    public bool IsHakemFirstUser => HakemUserId > 0 ? Users.First(x => x.Id == HakemUserId).FirstUser : true;

    public List<Card> Cards1 = [];
    public List<Card> Cards2 = [];

    public List<Card> CardsGround0 = [];
    public List<List<Card>> CardsGround1 = [];
    public List<List<Card>> CardsGround2 = [];

    public List<CardGroup> CardGroup1 = [];
    public List<CardGroup> CardGroup2 = [];

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
    public event Action<FinishModelRequest, List<byte>> Reload;

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

    public Task ShufflingCardsInit()
        => NotifyUsersAsync?.Invoke(Users.Select(x => x.ConnectionId), "ReceiveInit", true);
    public Task ReceiveInit()
        => NotifyUsersAsync?.Invoke(Users.Select(x => x.ConnectionId), "ReceiveInit", true);

    public async Task ReceiveCansel()
    {
        await OnNotifyUsersAsync(Users.Select(x => x.ConnectionId), "ReceiveCansel", true);
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

        await OnNotifyUsersAsync(Users.Select(x => x.ConnectionId), "ReceivePhysicsStandard", data);
    }

    public async Task InitReading()
    {
        await Task.Delay(50);
        var data = new
        {
            IsHakemFirstUser,
            ReadingTime
        };

        await OnNotifyUsersAsync(Users.Select(x => x.ConnectionId), "InitReading", data);
    }
    public async Task ReceiveCards()
    {
        await Task.Delay(50);
        var data1 = new
        {
            Cards1,
            CardGroup1
        }.ToJson();
        var data2 = new
        {
            Cards2,
            CardGroup2
        }.ToJson();

        _ = OnUserAsync(Users.First(x=>x.FirstUser).ConnectionId, "ReceiveCards", data1);
        _ = OnUserAsync(Users.First(x=>x.FirstUser).ConnectionId, "ReceiveCards", data2);
    }
}
