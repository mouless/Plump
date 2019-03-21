using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Cards.Models
{
    public abstract class Player
    {
        public List<Card> Hand { get; set; }
        public List<Card> TricksCount { get; set; }
        public string Name { get; set; }
        public Card CardToPlay { get; set; }

        protected Player(string name)
        {
            Hand = new List<Card>();
            TricksCount = new List<Card>();
            Name = name;
        }

        public abstract bool CheckIfTricksAreValid(Player player, int numberOfSticksThisRound, List<Player> players);

        public abstract bool PlayOutCard(Player player, int numberOfSticksThisRound, List<Player> players, Card firstCardPlayed);

        public void ClearHand()
        {
            Hand = new List<Card>();
            TricksCount = new List<Card>();
        }
    }
}
