using System.Collections.Generic;
using Cards.Models;

namespace Cards
{
    class Deck
    {
        public List<Card> CreateDeck()
        {
            var DeckOfCards = new List<Card>();
            for (Card.CardSuit i = Card.CardSuit.Hjärter; i <= Card.CardSuit.Klöver; i++)
            {
                for (Card.CardRank j = Card.CardRank.Två; j <= Card.CardRank.Ess; j++)
                {
                    DeckOfCards.Add(new Card(i, j));
                }
            }

            return ShuffleDeck(DeckOfCards);
        }

        static public List<Card> ShuffleDeck(List<Card> deck)
        {
            for (int n = deck.Count - 1; n > 0; --n)
            {
                int k = GameService.r.Next(n + 1);
                var temp = deck[n];
                deck[n] = deck[k];
                deck[k] = temp;
            }

            return deck;
        }
    }
}
