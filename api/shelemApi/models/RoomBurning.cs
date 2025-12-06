using System.Collections.Generic;

namespace shelemApi.Models;

public class RoomBurning
{

    private readonly RoomProperty _p;

    public event Action CompletBurning;
    public event Action CompletDetermination;
    public CancellationTokenSource token;
    public bool IsToken => token != null ? token.Token.IsCancellationRequested : false;

    public RoomBurning(RoomProperty roomProperty)
    {
        _p = roomProperty;
    }

    public void StartBurning()
    {
        _ = MainBurning();
    }

    private async Task MainBurning()
    {
        token = new CancellationTokenSource();
        try
        {
            await _p.InitBurning();
            await Task.Delay(TimeSpan.FromSeconds(_p.BurningTime), token.Token);
            _p.SetOflineCount();
            if(_p.CheckOflineCount) CompletBurning?.Invoke();
            else _ = MainBurning();
        }
        catch (Exception)
        {

        }
    }
    private void SetBurning(Guid key, List<byte> list)
    {
        var user = _p.Users.FirstOrDefault(x => x.Key == key);
        if (user == null || user.Id != _p.Tourner) return;
       
        var check = CheckBurning(user.FirstUser, list);
        if (!check) return;

        token?.Cancel();

        DrawnBurning(user.FirstUser, list);
        CompletBurning?.Invoke();

    }

    private bool CheckBurning(bool firstUser, List<byte> list)
    {
        if (list.Count != 4)
            return false;
        var cardsToCheck = firstUser ? _p.Cards1 : _p.Cards2;
        if (cardsToCheck == null || cardsToCheck.Count == 0)
            return false;

        var cardIdSet = new HashSet<byte>(cardsToCheck.Select(c => c.Id));
        return list.All(id => cardIdSet.Contains(id));
    }
    private void DrawnBurning(bool firstUser, List<byte> list)
    {
        var cardsToCheck = firstUser ? _p.Cards1 : _p.Cards2;
        var cardsWinning = firstUser ? _p.CardsWinning1 : _p.CardsWinning2;

        var cardsToMove = new List<Card>();

        for (int i = cardsToCheck.Count - 1; i >= 0; i--)
        {
            if (list.Contains(cardsToCheck[i].Id))
            {
                cardsToMove.Add(cardsToCheck[i]);
                cardsToCheck.RemoveAt(i);
            }
        }

        cardsWinning.AddRange(cardsToMove);
    }

    public void StartDetermination()
    {
        _p.actionTimeRatio = 1;
        _ = MainDetermination();
    }

    private async Task MainDetermination()
    {
        token = new CancellationTokenSource();
        try
        {
            await _p.ReceiveCards();
            await _p.InitDetermination();
            await Task.Delay(TimeSpan.FromSeconds(_p.ActionTime), token.Token);
            _p.SetOflineCount();
            if (_p.CheckOflineCount) CompletDetermination?.Invoke();
            else _ = MainDetermination();
        }
        catch (Exception)
        {
            _p.actionTimeRatio = 1;
        }
    }
    private void SetDetermination(Guid key, byte id)
    {
        var user = _p.Users.FirstOrDefault(x => x.Key == key);
        if (user == null || user.Id != _p.Tourner) return;

        var cardsToCheck = user.FirstUser ? _p.Cards1 : _p.Cards2;
        if (cardsToCheck == null || cardsToCheck.Count == 0)
            return;

        var card = cardsToCheck.FirstOrDefault(x => x.Id == id);
        if (card == null)
            return;

        token?.Cancel();

        cardsToCheck.Remove(card);
        _p.CardsMain.Add(card);

        _p.Tourner = _p.Users.First(x => x.Id != _p.Tourner).Id;
        if (_p.CardsMain.Count > 1)
            CompletDetermination?.Invoke();
        else
            _ = MainDetermination();
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

