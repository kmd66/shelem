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

    private async Task Start()
    {
        await Task.Delay(50);
    }

    public async Task Main(bool IsNotReceiveCards = false)
    {
        token = new CancellationTokenSource();
        try
        {
            if(!IsNotReceiveCards)
                await _p.ReceiveCards();
            await _p.InitGameAction();
            await Task.Delay(TimeSpan.FromSeconds(_p.ActionTime), token.Token);
            _p.SetOflineCount();
            if (_p.CheckOflineCount) CompletGame?.Invoke();
            else _ = Main();
        }
        catch (Exception)
        {
            _p.actionTimeRatio = 1;
        }
    }
    public async Task SetCard(Guid key, byte id)
    {
        var card = Check(key, id);
        if (card == null)
            return;

        token?.Cancel();

        _p.CardsMain.Add(card);

        _p.Tourner = _p.Users.First(x => x.Id != _p.Tourner).Id;

        //if (_p.CardsMain.Count > 1)
        //{
        //    CompletDetermination?.Invoke();
        //    return;
        //}
        //_p.HokmSuit = card.Suit;
        //_ = _p.ReceiveHokm();
        //_ = MainDetermination();
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

    private Card Check(Guid key, byte id)
    {
        var user = _p.Users.FirstOrDefault(x => x.Key == key);
        if (user == null || user.Id != _p.Tourner) return null;

        var card = _p.CardsMain.Count < 2 ? CheckFan(user, id): CheckGround(user, id);

        return card;
    }
    private Card CheckFan(RoomUser user, byte id)
    {
        var cardsToCheck = user.FirstUser ? _p.Cards1 : _p.Cards2;
        if (cardsToCheck == null)
            return null;

        var card = cardsToCheck.FirstOrDefault(x => x.Id == id);
        if (card == null)
            return null;

        if (_p.CardsMain.Count > 0)
        {
            var requiredSuit = _p.CardsMain[0].Suit;
            bool userHasRequiredSuit = cardsToCheck.Any(x => x.Suit == requiredSuit);
            if (userHasRequiredSuit && card.Suit != requiredSuit)
            {
                return null;
            }
        }

        cardsToCheck.Remove(card);
        return card;
    }

    private Card CheckGround(RoomUser user, byte id)
    {
        if (_p.CardsMain.Count < 2)
            return null;

        var cardsToCheck = user.FirstUser ? _p.CardGroup1 : _p.CardGroup2;
        var cardsToRemove = user.FirstUser ? _p.CardsGround1 : _p.CardsGround2;
        if (cardsToCheck == null || cardsToRemove == null)
            return null;

        var card = cardsToCheck.FirstOrDefault(x => x.Id == id);
        var listContainingCard = cardsToRemove.FirstOrDefault(list => list.Any(c => c.Id == id));
        var cardToRemove = listContainingCard.FirstOrDefault(c => c.Id == id);
        if (card == null || listContainingCard == null || cardToRemove == null)
            return null;

        var requiredSuit = _p.CardsMain[0].Suit;
        bool userHasRequiredSuit = cardsToCheck.Any(x => x.Suit == requiredSuit);
        if (userHasRequiredSuit && card.Suit != requiredSuit)
        {
            return null;
        }

        listContainingCard.Remove(cardToRemove);

        SetCardGroup();

        return cardToRemove;
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

