using System.Collections.Generic;

namespace Cards
{
    public class Scoreboard
    {
        public List<PlayerScore> PlayerScoreList { get; set; } = new List<PlayerScore>();
    }

    public class PlayerScore
    {
        public string Name { get; set; }
        public List<Round> RoundList { get; set; } = new List<Round>();
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