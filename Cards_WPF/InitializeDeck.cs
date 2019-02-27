using System.Collections.Generic;

namespace Cards_WPF
{
    static class InitializeDeck
    {
        public static void InitializeNewDeck()
        {
            DeckOfCards = new List<Card>();
            for (Card.CardSuit i = Card.CardSuit.Hjärter; i <= Card.CardSuit.Klöver; i++)
            {
                for (Card.CardRank j = Card.CardRank.Två; j <= Card.CardRank.Ess; j++)
                {
                    DeckOfCards.Add(new Card(i, j));
                }
            }
        }
    }
}
