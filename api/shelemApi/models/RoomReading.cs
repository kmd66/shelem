using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using System;
using System.Collections.Generic;

namespace shelemApi.Models;

public class RoomReading
{
    public event Action<int> CompletSetReading;
    public CancellationTokenSource token;
    public bool IsToken => token != null ? token.Token.IsCancellationRequested : false;

    private readonly RoomProperty _property;

    public RoomReading(RoomProperty roomProperty)
    {
        _property = roomProperty;
    }

    public async Task Start()
    {
        _property.MinReading = 0;
        _property.HakemUserId = 0;

        List<int> suits = [0, 1, 2, 3]; // ♠, ♥, ♣, ♦
        List<int> ranks = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13];
        List<Card> allCards = new List<Card>();
        foreach (int suit in suits)
        {
            foreach (int rank in ranks)
            {
                allCards.Add(new Card(suit, rank));
            }
        }
        ShuffleCards(allCards);
        DistributeCards(allCards);
        await Task.Delay(50);
        await _property.ShufflingCardsInit();
        await Task.Delay(TimeSpan.FromSeconds(5));
        await _property.ReceiveCards();
        _ = Main();
    }

    private void ShuffleCards(List<Card> cards)
    {
        Random rng = new Random();

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
        int index = 0;

        _property.Cards1 = allCards.GetRange(index, 12);
        index += 12;

        _property.Cards2 = allCards.GetRange(index, 12);
        index += 12;

        _property.CardsGround0 = allCards.GetRange(index, 4);
        index += 4;

        _property.CardsGround1 = new List<List<Card>>();
        for (int i = 0; i < 3; i++)
        {
            _property.CardsGround1.Add(allCards.GetRange(index, 4));
            index += 4;
        }
        
        _property.CardsGround2 = new List<List<Card>>();
        for (int i = 0; i < 3; i++)
        {
            _property.CardsGround2.Add(allCards.GetRange(index, 4));
            index += 4;
        }
        _property.CardGroup1 =
        [
            new CardGroup(_property.CardsGround1[0].Count),
            new CardGroup(_property.CardsGround1[1].Count),
            new CardGroup(_property.CardsGround1[2].Count),
        ];
        _property.CardGroup2 =
        [
            new CardGroup(_property.CardsGround2[0].Count),
            new CardGroup(_property.CardsGround2[1].Count),
            new CardGroup(_property.CardsGround2[2].Count),
        ];
    }

    public async Task Main()
    {
        token = new CancellationTokenSource();
        try
        {
            await Task.Delay(TimeSpan.FromSeconds(3));
            await _property.InitReading();
            await Task.Delay(TimeSpan.FromSeconds(_property.ReadingTime), token.Token);
            CompletSetReading?.Invoke(0);
        }
        catch (Exception)
        {

        }
    }

    public void SetReading(Guid key, int reading)
    {
        if (reading !=0 && reading <= _property.MinReading) return;
        if (reading > 365) return;
        var user = _property.Users.FirstOrDefault(x => x.Key == key);
        if (user == null || user.Id != _property.Tourner) return;
        token?.Cancel();
        if (reading < 100 || reading > 360) {
            CompletSetReading?.Invoke(reading);
            return;
        }
        _property.MinReading = reading;
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

