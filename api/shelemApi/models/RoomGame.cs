namespace shelemApi.Models;

public class RoomGame
{

    private readonly RoomProperty _roomProperty;

    public RoomGame(RoomProperty roomProperty)
    {
        _roomProperty = roomProperty;
    }
    public void dispose()
    {
    }
}

