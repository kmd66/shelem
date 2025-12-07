using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using System;
using System.Collections.Generic;

namespace shelemApi.Models;

public class RoomReading(RoomProperty roomProperty)
{
    public event Func<int, Task> CompletSetReading;
    public CancellationTokenSource token;
    public bool IsToken => token != null ? token.Token.IsCancellationRequested : false;

    private readonly RoomProperty _p = roomProperty;

    public async Task Start()
    {
        _p.MinReading = 0;
        _p.HakemUserId = 0;

        List<byte> suits = [0, 1, 2, 3]; // ♠, ♥, ♣, ♦
        List<byte> ranks = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13];
        List<Card> allCards = [];
        byte id = 0;
        foreach (byte suit in suits)
        {
            id++;
            foreach (byte rank in ranks)
            {
                id++;
                allCards.Add(new Card(id, suit, rank));
            }
        }
        ShuffleCards(allCards);
        DistributeCards(allCards);
        await Task.Delay(50);
        await _p.InitEvent("ShufflingCardsInit");
        await Task.Delay(TimeSpan.FromSeconds(5));
        await _p.ReceiveCards();
        _ = Main();
    }

    private void ShuffleCards(List<Card> cards)
    {
        Random rng = new();

        for (int i = cards.Count - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1);

            // جابجایی
            Card temp = cards[i];
            cards[i] = cards[j];
            cards[j] = temp;
        }
    }

    private void DistributeCards(List<Card> allCards)
    {
        _p.CardsWinning1 = [];
        _p.CardsWinning2 = [];
        _p.CardsMain = [];
        int index = 0;

        _p.Cards1 = allCards.GetRange(index, 12);
        index += 12;

        _p.Cards2 = allCards.GetRange(index, 12);
        index += 12;

        _p.CardsGround0 = allCards.GetRange(index, 4);
        index += 4;

        _p.CardsGround1 = [];
        for (int i = 0; i < 3; i++)
        {
            _p.CardsGround1.Add(allCards.GetRange(index, 4));
            index += 4;
        }
        
        _p.CardsGround2 = [];
        for (int i = 0; i < 3; i++)
        {
            _p.CardsGround2.Add(allCards.GetRange(index, 4));
            index += 4;
        }
        _p.CardGroup1 =
        [
            new CardGroup(_p.CardsGround1[0].Count),
            new CardGroup(_p.CardsGround1[1].Count),
            new CardGroup(_p.CardsGround1[2].Count),
        ];
        _p.CardGroup2 =
        [
            new CardGroup(_p.CardsGround2[0].Count),
            new CardGroup(_p.CardsGround2[1].Count),
            new CardGroup(_p.CardsGround2[2].Count),
        ];
    }

    public async Task Main()
    {
        token = new CancellationTokenSource();
        try
        {
            await Task.Delay(TimeSpan.FromSeconds(3));
            await _p.InitReading();
            await Task.Delay(TimeSpan.FromSeconds(_p.ReadingTime), token.Token);
            CompletSetReading?.Invoke(0);
        }
        catch (Exception)
        {

        }
    }

    public void SetReading(Guid key, int reading)
    {
        if (reading !=0 && reading <= _p.MinReading) return;
        if (reading > 165) return;
        var user = _p.Users.FirstOrDefault(x => x.Key == key);
        if (user == null || user.Id != _p.Tourner) return;
        token?.Cancel();
        if (reading < 100 || reading > 160) {
            CompletSetReading?.Invoke(reading);
            return;
        }
        _p.MinReading = reading;
        _p.Tourner = _p.Users.First(x => x.Id != _p.Tourner).Id;
        _ = Main();
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

