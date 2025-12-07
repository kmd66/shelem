namespace shelemApi.Models;

public class RoomGame
{

    private readonly RoomProperty _p;
    public CancellationTokenSource token;
    public event Func<Task> CompletGame;

    public RoomGame(RoomProperty roomProperty)
    {
        _p = roomProperty;
    }

    public async Task Start()
    {
        await Task.Delay(50);
    }

    private async Task MainDetermination()
    {
        token = new CancellationTokenSource();
        try
        {
            await _p.InitGameAction();
            await Task.Delay(TimeSpan.FromSeconds(_p.ActionTime), token.Token);
            _p.SetOflineCount();
            if (_p.CheckOflineCount) CompletGame?.Invoke();
            else _ = MainDetermination();
        }
        catch (Exception)
        {
            _p.actionTimeRatio = 1;
        }
    }

    public void SetCardGroup()
    {
        _p.CardGroup1 =
        [
            new CardGroup(_p.CardsGround1[0]),
            new CardGroup(_p.CardsGround1[1]),
            new CardGroup(_p.CardsGround1[2]),
        ];
        _p.CardGroup2 =
        [
            new CardGroup(_p.CardsGround2[0]),
            new CardGroup(_p.CardsGround2[1]),
            new CardGroup(_p.CardsGround2[2]),
        ];
    }
    public void dispose()
    {
        try
        {
            token?.Cancel();
        }
        finally
        {
            token?.Dispose();
        }
    }
}

