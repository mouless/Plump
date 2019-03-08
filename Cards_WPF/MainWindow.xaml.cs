using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Cards;

namespace Cards_WPF
{
    public partial class MainWindow : Window
    {
        public bool StartAnotherRound { get; set; } = false;
        public int NumberOfSticks { get; set; } = 5;
        public int NumberOfPlayedRounds { get; set; } = 1;

        private GameService gameService;

        public MainWindow()
        {
            InitializeComponent();

            gameService = new GameService();
            StartGame_BackEnd();
            StartGame_FrontEnd();
        }

        private void FrontEnd(GameService currentGame)
        {
            ShowHandText(currentGame);

            ShowTricks(currentGame);

            ShowImageCards(currentGame);

            PlayHighestCard(currentGame);

            PlayCard(currentGame);

            ShowHands(currentGame);
        }

        private void ShowHandText(GameService currentGame)
        {
            ListBox_North.ItemsSource = currentGame.Players.Find(name => name.Name == "North").Hand;
            ListBox_East.ItemsSource = currentGame.Players.Find(name => name.Name == "East").Hand;
            ListBox_South.ItemsSource = currentGame.Players.Find(name => name.Name == "South").Hand;
            ListBox_Player1.ItemsSource = currentGame.Players.Find(name => name.Name == "Player1").Hand;
        }

        private void ShowTricks(GameService currentGame)
        {
            Label_NorthTricks.Content = $"({currentGame.TricksCount[0].Count})";
            Label_EastnTricks.Content = $"({currentGame.TricksCount[1].Count})";
            Label_SouthTricks.Content = $"({currentGame.TricksCount[2].Count})";
            Label_PlayaTricks.Content = $"({currentGame.TricksCount[3].Count})";
        }

        private void ShowImageCards(GameService currentGame)
        {
            string cardNumber = "";
            for (int j = 0; j < currentGame.Players[0].Hand.Count; j++)
            {
                cardNumber = CardImageNumber(currentGame.Players[0].Hand[j]);
                //Uri uri = new Uri($"C:\\Users\\Mouless\\Source\\Repos\\Plump\\Cards_WPF\\Graphics\\{cardNumber}.jpeg");
                Uri uri = new Uri($"C:\\Users\\William Boquist\\Plump\\Cards_WPF\\Graphics\\{cardNumber}.jpeg");
                Image img = FindName("Image_North" + j) as Image;
                img.Source = new BitmapImage(uri);
            }
        }

        public void PlayHighestCard(GameService currentGame)
        {
            currentGame.CardToPlay_North = currentGame.Players[0].Hand.OrderByDescending(v => v.Rank).First();
            string cardNumber = CardImageNumber(currentGame.CardToPlay_North);
            //var uri = new Uri($"C:\\Users\\Mouless\\Source\\Repos\\Plump\\Cards_WPF\\Graphics\\{cardNumber}.jpeg");
            Uri uri = new Uri($"C:\\Users\\William Boquist\\Plump\\Cards_WPF\\Graphics\\{cardNumber}.jpeg");
            Image_NorthPlayed.Source = new BitmapImage(uri);

            currentGame.CardToPlay_Eastn = currentGame.Players[1].Hand.OrderByDescending(v => v.Rank).First();
            cardNumber = CardImageNumber(currentGame.CardToPlay_Eastn);
            //uri = new Uri($"C:\\Users\\Mouless\\Source\\Repos\\Plump\\Cards_WPF\\Graphics\\{cardNumber}.jpeg");
            uri = new Uri($"C:\\Users\\William Boquist\\Plump\\Cards_WPF\\Graphics\\{cardNumber}.jpeg");
            Image_EastnPlayed.Source = new BitmapImage(uri);

            currentGame.CardToPlay_South = currentGame.Players[2].Hand.OrderByDescending(v => v.Rank).First();
            cardNumber = CardImageNumber(currentGame.CardToPlay_South);
            //uri = new Uri($"C:\\Users\\Mouless\\Source\\Repos\\Plump\\Cards_WPF\\Graphics\\{cardNumber}.jpeg");
            uri = new Uri($"C:\\Users\\William Boquist\\Plump\\Cards_WPF\\Graphics\\{cardNumber}.jpeg");
            Image_SouthPlayed.Source = new BitmapImage(uri);

            currentGame.CardToPlay_Playa = currentGame.Players[3].Hand.OrderByDescending(v => v.Rank).First();
            cardNumber = CardImageNumber(currentGame.CardToPlay_Playa);
            //uri = new Uri($"C:\\Users\\Mouless\\Source\\Repos\\Plump\\Cards_WPF\\Graphics\\{cardNumber}.jpeg");
            uri = new Uri($"C:\\Users\\William Boquist\\Plump\\Cards_WPF\\Graphics\\{cardNumber}.jpeg");
            Image_PlayaPlayed.Source = new BitmapImage(uri);
        }

        private void PlayCard(GameService currentGame)
        {
            // WHO HAS THE MOST TRICKS GOING IN ORDER OR STARTING WITH A NEW ONE EVERY ROUND?
            var numberOfTricks_North = currentGame.TricksCount[0].Count;
            var numberOfTricks_East = currentGame.TricksCount[1].Count;
            var numberOfTricks_South = currentGame.TricksCount[2].Count;
            var numberOfTricks_Player1 = currentGame.TricksCount[3].Count;

            var highestTricks = numberOfTricks_North; // FRÅGAN ÄR OM MAN VILL ATT OLIKA SPELARE SKA BÖRJA RUNDORNA
            var nameOfHighest = currentGame.Players[0]; // FÖR NU BÖRJAR BARA NORTH HELA TIDEN

            for (int i = 0; i < currentGame.TricksCount.Count; i++)
            {
                if (highestTricks < currentGame.TricksCount[i].Count)
                {
                    highestTricks = currentGame.TricksCount[i].Count;
                    nameOfHighest = currentGame.Players[i];
                }
            }

            HighestTricks_Label.Content = $"{nameOfHighest.Name} har flest stick med {highestTricks}";
        }

        public void ShowHands(GameService currentGame)
        {
            Label_NorthHand.Content = currentGame.CardToPlay_North;
            Label_SouthHand.Content = currentGame.CardToPlay_South;
            Label_EastnHand.Content = currentGame.CardToPlay_Eastn;
            Label_PlayaHand.Content = currentGame.CardToPlay_Playa;
        }

        private void KörTillsDetFinnsFyraKortISammaFärg_Click(object sender, RoutedEventArgs e)
        {
            do
            {
                NumberOfPlayedRounds++;
                Label_Number.Content = NumberOfPlayedRounds.ToString();
                StartGame_BackEnd();

            } while (!gameService.State.FyraLikaYao);

            gameService.State.FyraLikaYao = false;
            StartGame_FrontEnd();
        }

        private void StartAnotherRound_Click(object sender, RoutedEventArgs e)
        {
            StartGame_BackEnd();
            StartGame_FrontEnd();
        }

        private string CardImageNumber(Cards.Models.Card cardToNum)
        {
            int suit = (int)cardToNum.Suit;
            int rank = (int)cardToNum.Rank;
            string temp = rank.ToString();
            if (rank < 10)
            {
                temp = "0" + rank.ToString();
            }
            return suit.ToString() + temp;
        }

        private void Starta100NyaRundor_Click(object sender, RoutedEventArgs e)
        {
            int numberOfRestarts = 100;
            do
            {
                numberOfRestarts--;
                StartGame_BackEnd();
            } while (numberOfRestarts != 0);
            StartGame_FrontEnd();
        }

        public void StartGame_BackEnd()
        {
            Label_Number.Content = NumberOfPlayedRounds.ToString();

            gameService.CreateRound(NumberOfSticks);

            NumberOfPlayedRounds++;
        }

        public void StartGame_FrontEnd()
        {
            FrontEnd(gameService);
        }
    }
}