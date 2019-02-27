namespace Cards.Models
{
    public class Card
    {
        public enum CardSuit { Hjärter = 1, Spader, Ruter, Klöver}
        public enum CardRank { Två = 2, Tre, Fyra, Fem, Sexa, Sju, Åtta, Nio, Tio, Knekt, Dam, Knug, Ess }

        public CardSuit Suit { get; set; }
        public CardRank Rank { get; set; }
        public string Symbol { get; set; }

        public Card(CardSuit suit, CardRank rank)
        {
            Suit = suit;
            Rank = rank;
        }

        public override string ToString()
        {
            return Suit.ToString() + " - " + Rank.ToString();
        }
    }
}
