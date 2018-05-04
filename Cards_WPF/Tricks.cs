using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cards_WPF
{
    public class Tricks
    {
        public List<Card> NumOfTricks { get; set; }

        public Tricks(List<Card> numOfTricks)
        {
            NumOfTricks = numOfTricks;
        }
    }
}
