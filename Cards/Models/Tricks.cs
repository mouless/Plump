using System.Collections.Generic;

namespace Cards.Models
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
