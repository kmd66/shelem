namespace shelemApi.Models;

public class Card(byte id, byte suit, byte rank)
{
    public byte Id { get; set; } = id;
    public byte Suit { get; set; } = suit;
    public byte Rank { get; set; } = rank;

    public override string ToString()
    {
        string[] suits = ["♠", "♥", "♣", "♦"];
        string rankStr = Rank switch
        {
            1 => "A",
            11 => "J",
            12 => "Q",
            13 => "K",
            _ => Rank.ToString()
        };
        return $"{rankStr}{suits[Suit]}";
    }
    public override bool Equals(object obj)
    {
        if (obj == null || obj is not Card) return false;
        if(Suit == (obj as Card).Suit && Rank == (obj as Card).Rank) return true;
        return false;
    }

    public override int GetHashCode()
    {
        throw new NotImplementedException();
    }
}

public class CardGroup
{
    public byte Id { get; set; }
    public int Count { get; set; }
    public int Suit { get; set; }
    public int Rank { get; set; }

    public CardGroup(int count, byte id = 0, int suit = -1, int rank = -1)
    {
        Id = id;
        Count = count;
        Suit = suit;
        Rank = rank;
    }

    public CardGroup(List<Card> cards)
    {
        Count = cards.Count;
        if (Count > 0)
        {
            var card = cards.Last();
            Id = card.Id;
            Suit = card.Suit;
            Rank = card.Rank;
        }
        else
        {
            Id = 0;
            Suit = -1;
            Rank = -1;
        }
    }
}