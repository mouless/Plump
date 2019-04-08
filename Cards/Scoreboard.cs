using System.Collections.Generic;

namespace Cards
{
    public class Scoreboard
    {
        public List<PlayerScore> PlayerScoreList { get; set; }
    }

    public class PlayerScore
    {
        public List<Round> RoundList { get; set; }
        public string Name { get; set; }
        public int TotalScore { get; set; }
    }

    public class Round
    {
        public int RoundName { get; set; }
        public int RoundScore { get; set; }

        public int TricksCount { get; set; }
        public int TricksWon { get; set; }
    }
}