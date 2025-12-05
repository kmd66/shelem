namespace shelemApi.Models;

public class RoomBurning
{

    private readonly RoomProperty _property;

    public event Action<int> CompletBurning;
    public event Action<int> CompletDetermination;
    public CancellationTokenSource token;
    public bool IsToken => token != null ? token.Token.IsCancellationRequested : false;

    public RoomBurning(RoomProperty roomProperty)
    {
        _property = roomProperty;
    }
    public async Task StartBurning()
    {
        await Task.Delay(50);
    }

    public async Task MainBurning()
    {
        await Task.Delay(50);
    }

    public async Task SetBurning(Guid key, int reading)
    {
        await Task.Delay(50);
    }

    public async Task StartDetermination()
    {
        await Task.Delay(50);
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

