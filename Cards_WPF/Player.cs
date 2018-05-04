using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cards_WPF
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
