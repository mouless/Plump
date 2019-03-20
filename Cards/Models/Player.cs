using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Cards.Models
{
    public abstract class Player
    {
        public ConcurrentBag<Card> Hand { get; set; }
        public string Name { get; set; }
        public Card CardToPlay { get; set; }

        protected Player(string name)
        {
            Hand = new ConcurrentBag<Card>();
            Name = name;
        }

        public abstract bool CheckIfTricksAreValid(Player player, int numberOfSticksThisRound, List<List<Card>> tricksCount, List<Player> players);

        public abstract bool PlayOutCard(Player player, int numberOfSticksThisRound, List<List<Card>> tricksCount, List<Player> players, Card firstCardPlayed);

        public void ClearHand()
        {
            Hand = new ConcurrentBag<Card>();
        }
    }
}
