using System.Collections.ObjectModel;

namespace Cards.Models
{
    public class Player
    {
        public ObservableCollection<Card> Hand { get; set; }
        public string Name { get; set; }

        public Player(string name)
        {
            Hand = new ObservableCollection<Card>();
            Name = name;
        }
    }
}
