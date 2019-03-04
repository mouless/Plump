using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cards.Models;

namespace Cards
{
    public class DealCards
    {
        public void DistributeCards(List<Player> players, List<Card> deckOfCards, int numberOfSticksThisRound)
        {
            for (int i = 0; i < numberOfSticksThisRound; i++)
            {
                foreach (var player in players)
                {
                    player.Hand.Add(deckOfCards[0]);
                    deckOfCards.RemoveAt(0);
                }
            }

            Stack urb = new Stack(); // TODO: GÖRA OM KORTLEKEN TILL EN "STACK"
            
            foreach (var player in players)
            {
                var newHand = player.Hand.OrderBy(c => c.Suit).ThenByDescending(c => c.Rank).ToList();
                player.Hand.Clear();

                foreach (var card in newHand)
                {
                    player.Hand.Add(card);
                }
            }
        }
    }
}
