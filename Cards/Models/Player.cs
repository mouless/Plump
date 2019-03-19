using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Cards.Models
{
    public abstract class Player
    {
        public ObservableCollection<Card> Hand { get; set; }

        public string Name { get; set; }

        public Player(string name)
        {
            Hand = new ObservableCollection<Card>();
            Name = name;
        }

        public abstract void CheckIfTricksAreValid(Player player, int numberOfSticksThisRound, List<List<Card>> tricksCount, List<Player> players);

        public abstract void PlayOutCard(Player player, int numberOfSticksThisRound, List<List<Card>> tricksCount, List<Player> players);
    }
}
