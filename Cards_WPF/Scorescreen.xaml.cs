using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

namespace Cards_WPF
{
    public partial class Scorescreen : Window
    {
        public List<ScoreboardData> GameOver_ScoreboardItemsSource { get; set; } = new List<ScoreboardData>();

        public Scorescreen()
        {
            InitializeComponent();

        }

        public void ShowScoreboard(List<ScoreboardData> scores)
        {
            Scoreboard_GameOver_Datagrid.ItemsSource = scores;
            Show();

            var finishedRound = GameOver_ScoreboardItemsSource[GameOver_ScoreboardItemsSource.Count - 1];

            var dict = new Dictionary<string, int>();

            dict.Add("West", finishedRound.West);
            dict.Add("North", finishedRound.North);
            dict.Add("East", finishedRound.East);
            dict.Add("Player1", finishedRound.Player1);

            var highestScores = dict.Max(x => x.Value);
            var highestNames = dict.Where(x => x.Value == highestScores).ToList();

            var textForWinners = "Congratulations\n";
            Scoreboard_Winner_Label.Content = textForWinners;

            if (highestNames.Count > 1)
            {
                Scoreboard_Winner_Label.Content += "It's a draw between ";
            }

            foreach (var playa in highestNames)
            {
                Scoreboard_Winner_Label.Content += playa.Key;
                if (highestNames.Count > 1)
                {
                    Scoreboard_Winner_Label.Content += " & ";
                }
            }

            Scoreboard_Winner_Label.Content += $" with\n{highestScores}";
            Scoreboard_Winner_Label.HorizontalContentAlignment = HorizontalAlignment.Center;

            string fromHighscoreTxt = File.ReadAllText($@"{Directory.GetCurrentDirectory()}\..\..\Highscore.txt");

            var highscoreList = new List<HighscoreModel>();

            if (fromHighscoreTxt != "")
            {
                highscoreList = JsonConvert.DeserializeObject<List<HighscoreModel>>(fromHighscoreTxt);
            }

            if (highestNames.Count == 1)
            {
                highscoreList.Add(
                    new HighscoreModel
                    {
                        Name = highestNames[0].Key,
                        Score = highestNames[0].Value
                    });

                highscoreList.OrderByDescending(x => x.Score);

                var result = JsonConvert.SerializeObject(highscoreList);

                File.WriteAllText($@"{Directory.GetCurrentDirectory()}\..\..\Highscore.txt", result);
            }

            Highscore_GameOver_Datagrid.ItemsSource = highscoreList;
        }
    }

    public class HighscoreModel
    {
        public string Name { get; set; }
        public int Score { get; set; }
    }
}
