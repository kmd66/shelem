namespace shelemApi.Models;

public class RoomGame
{

    private readonly RoomProperty _p;
    public event Action<int> CompletGame;

    public RoomGame(RoomProperty roomProperty)
    {
        _p = roomProperty;
    }

    public async Task Start()
    {
        await Task.Delay(50);
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
    }
}

