namespace shelemApi.Models;

public class Card
{
    public int Suit { get; set; } // 0: ♠, 1: ♥, 2: ♣, 3: ♦
    public int Rank { get; set; } // 1-13

    public Card(int suit, int rank)
    {
        Suit = suit;
        Rank = rank;
    }

    public override string ToString()
    {
        string[] suits = { "♠", "♥", "♣", "♦" };
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
}
public class CardGroup
{
    public int Count { get; set; }
    public int Suit { get; set; }
    public int Rank { get; set; }

    public CardGroup(int count, int suit = -1, int rank = -1)
    {
        Count = count;
        Suit = suit;
        Rank = rank;
    }
}