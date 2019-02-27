using System.Collections.Generic;

namespace Cards
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
