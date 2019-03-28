using System.Collections.Generic;

namespace Cards
{
    public class Scoreboard
    {
        public List<Round> Round { get; set; }
    }

    public class Round
    {
        public int RoundName { get; set; }
        public string PlayerName { get; set; }
        public int TotalScore { get; set; }
        public Turn Turn { get; set; }
    }

    public class Turn
    {
        public int TurnName { get; set; }
        public int TricksCount { get; set; }
        public int TricksWon { get; set; }
    }
}
