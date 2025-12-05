namespace shelemApi.Models;

public class RoomGame
{

    private readonly RoomProperty _roomProperty;
    public event Action<int> CompletGame;

    public RoomGame(RoomProperty roomProperty)
    {
        _roomProperty = roomProperty;
    }

    public async Task Start()
    {
        await Task.Delay(50);
    }
    public void dispose()
    {
    }
}

