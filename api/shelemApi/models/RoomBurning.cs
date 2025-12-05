namespace shelemApi.Models;

public class RoomBurning
{

    public event Action<int> CompletBurning;
    public event Action<int> CompletDetermination;
    private readonly RoomProperty _roomProperty;

    public RoomBurning(RoomProperty roomProperty)
    {
        _roomProperty = roomProperty;
    }
    public async Task StartBurning()
    {
    }
    public async Task StartDetermination()
    {
    }
    public void dispose()
    {
    }
}

