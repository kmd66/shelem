using System;
using System.Collections.Generic;

namespace shelemApi.Models;

public class RoomReading
{
    public CancellationTokenSource ReadingToken;
    public bool IsReading => ReadingToken != null ? ReadingToken.Token.IsCancellationRequested : false;

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
        await Task.Delay(TimeSpan.FromSeconds(5));
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
        ReadingToken = new CancellationTokenSource();
        try
        {
            await _property.InitReading();
            await Task.Delay(TimeSpan.FromSeconds(_property.ReadingTime), ReadingToken.Token);
        }
        catch (Exception)
        {

        }
    }

    public void dispose()
    {
        try
        {
            ReadingToken?.Cancel();
        }
        finally
        {
            ReadingToken?.Dispose();
        }
    }
}

