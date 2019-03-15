using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Cards;

namespace Cards_WPF
{
    public partial class MainWindow : Window
    {
        public int NumberOfSticks { get; set; } = 5;
        public int NumberOfPlayedRounds { get; set; } = 1;
        public List<VisualStuff> VisualStuffList { get; set; }
        public List<CardPicture> CardPicturesList { get; set; } = new List<CardPicture>();

        private GameService gameService;

        public MainWindow()
        {
            InitializeComponent();

            CreateVisualStuff();

            CreateCroppedBitmapCards(CardPicturesList);

            StartNewGame();

        }

        private void CreateCroppedBitmapCards(List<CardPicture> cardPicturesList)
        {
            var spriteSheetService = new SpriteSheetService();
            spriteSheetService.CutImage(cardPicturesList);
        }

        private void StartNewGame()
        {
            gameService = new GameService();
            StartGame_BackEnd();
            StartGame_FrontEnd(gameService);
        }

        public void StartGame_BackEnd()
        {
            Label_Number.Content = NumberOfPlayedRounds.ToString();

            gameService.CreateRound(NumberOfSticks);

            NumberOfPlayedRounds++;
        }

        private void StartGame_FrontEnd(GameService currentGame)
        {
            ShowHandText(currentGame);

            ShowTricks(currentGame);

            ShowImageCards(currentGame);

            ShowHighestTricks(currentGame, NumberOfSticks);

            if (currentGame.NotPossibleTricks == true)
            {
                MessageBox.Show("Last player's tricks equalled the same as the amount of tricks for the current round...");
            }

            //WinnerOfTheTrickRound();

            //UpdateScoreboard();
        }



        private void ShowHandText(GameService currentGame)
        {
            ListBox_West.ItemsSource = currentGame.Players.Find(name => name.Name == "West").Hand;
            ListBox_North.ItemsSource = currentGame.Players.Find(name => name.Name == "North").Hand;
            ListBox_East.ItemsSource = currentGame.Players.Find(name => name.Name == "East").Hand;
            ListBox_Player1.ItemsSource = currentGame.Players.Find(name => name.Name == "Player1").Hand;
        }

        private void ShowTricks(GameService currentGame)
        {
            Label_WestnTricks.Content = $"({currentGame.TricksCount[0].Count})";
            Label_NorthTricks.Content = $"({currentGame.TricksCount[1].Count})";
            Label_EastnTricks.Content = $"({currentGame.TricksCount[2].Count})";
            Label_PlayaTricks.Content = $"({currentGame.TricksCount[3].Count})";
        }

        private void ShowImageCards(GameService currentGame)
        {
            string cardNumber = "";
            for (int j = 0; j < currentGame.Players[3].Hand.Count; j++)
            {
                cardNumber = CardImageNumber(currentGame.Players[3].Hand[j]);
                //Uri uri = new Uri($"C:\\Users\\Mouless\\Source\\Repos\\Plump\\Cards_WPF\\Graphics\\{cardNumber}.jpeg");
                Uri uri = new Uri($"C:\\Users\\William Boquist\\Plump\\Cards_WPF\\Graphics\\{cardNumber}.jpeg");
                Image img = FindName("Image_Playa" + j) as Image;
                img.Source = new BitmapImage(uri);
            }
        }

        public void ShowPlayersTrickCard(GameService currentGame)
        {
            currentGame.CardToPlay_Westn = currentGame.Players[0].Hand.OrderByDescending(v => v.Rank).First();
            string cardNumber = CardImageNumber(currentGame.CardToPlay_Westn);
            //var uri = new Uri($"C:\\Users\\Mouless\\Source\\Repos\\Plump\\Cards_WPF\\Graphics\\{cardNumber}.jpeg");
            Uri uri = new Uri($"C:\\Users\\William Boquist\\Plump\\Cards_WPF\\Graphics\\{cardNumber}.jpeg");
            Image_WestnPlayed.Source = new BitmapImage(uri);

            currentGame.CardToPlay_North = currentGame.Players[1].Hand.OrderByDescending(v => v.Rank).First();
            cardNumber = CardImageNumber(currentGame.CardToPlay_North);
            //uri = new Uri($"C:\\Users\\Mouless\\Source\\Repos\\Plump\\Cards_WPF\\Graphics\\{cardNumber}.jpeg");
            uri = new Uri($"C:\\Users\\William Boquist\\Plump\\Cards_WPF\\Graphics\\{cardNumber}.jpeg");
            Image_NorthPlayed.Source = new BitmapImage(uri);

            currentGame.CardToPlay_Eastn = currentGame.Players[2].Hand.OrderByDescending(v => v.Rank).First();
            cardNumber = CardImageNumber(currentGame.CardToPlay_Eastn);
            //uri = new Uri($"C:\\Users\\Mouless\\Source\\Repos\\Plump\\Cards_WPF\\Graphics\\{cardNumber}.jpeg");
            uri = new Uri($"C:\\Users\\William Boquist\\Plump\\Cards_WPF\\Graphics\\{cardNumber}.jpeg");
            Image_EastnPlayed.Source = new BitmapImage(uri);

            currentGame.CardToPlay_Playa = currentGame.Players[3].Hand.OrderByDescending(v => v.Rank).First();
            cardNumber = CardImageNumber(currentGame.CardToPlay_Playa);
            //uri = new Uri($"C:\\Users\\Mouless\\Source\\Repos\\Plump\\Cards_WPF\\Graphics\\{cardNumber}.jpeg");
            uri = new Uri($"C:\\Users\\William Boquist\\Plump\\Cards_WPF\\Graphics\\{cardNumber}.jpeg");
            Image_PlayaPlayed.Source = new BitmapImage(uri);

        }

        private void ShowHighestTricks(GameService currentGame, int antalKortIRundan)
        {
            // WHO HAS THE MOST TRICKS GOING IN ORDER OR STARTING WITH A NEW ONE EVERY ROUND?
            var numberOfTricks_West = currentGame.TricksCount[0].Count;
            var numberOfTricks_North = currentGame.TricksCount[1].Count;
            var numberOfTricks_East = currentGame.TricksCount[2].Count;
            var numberOfTricks_Player1 = currentGame.TricksCount[3].Count;

            var highestTricks = numberOfTricks_West; // FRÅGAN ÄR OM MAN VILL ATT OLIKA SPELARE SKA BÖRJA RUNDORNA
            var nameOfHighest = currentGame.Players[0]; // FÖR NU BÖRJAR BARA WEST HELA TIDEN

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

        private void StartAnotherRound_Click(object sender, RoutedEventArgs e)
        {
            StartGame_BackEnd();
            StartGame_FrontEnd(gameService);
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

        private void UpdateScoreboard()
        {
            //throw new NotImplementedException();
        }

        private void WinnerOfTheTrickRound()
        {
            //throw new NotImplementedException();
        }

        private void CreateVisualStuff()
        {
            VisualStuffList = new List<VisualStuff>
            {
                new VisualStuff
                {
                    CardName = "Image_Playa0",
                },
                new VisualStuff
                {
                    CardName = "Image_Playa1",
                },
                new VisualStuff
                {
                    CardName = "Image_Playa2",
                },
                new VisualStuff
                {
                    CardName = "Image_Playa3",
                },
                new VisualStuff
                {
                    CardName = "Image_Playa4",
                },
            };
        }

        private void Image_PlayaCard_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var cardOfSender = (Image)sender;
            var nameOfSender = cardOfSender.Name;
            var marginOfSender = cardOfSender.Margin;

            foreach (var bild in VisualStuffList)
            {
                if (bild.CardName == nameOfSender)
                {
                    if (bild.CardSelected == true)
                    {
                        marginOfSender.Top += 30;
                        cardOfSender.Margin = marginOfSender;
                        bild.CardSelected = false;
                    }
                    else
                    {
                        marginOfSender.Top -= 30;
                        cardOfSender.Margin = marginOfSender;
                        bild.CardSelected = true;
                    }

                    return;
                }
            }
        }

        private void Image_Test_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var numberOfCards = CardPicturesList.Count;
            var randomCard = new Random();
            var randomNumber = randomCard.Next(0, 52);
            Image_Test.Source = CardPicturesList[randomNumber].Picture;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}