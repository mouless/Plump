using Cards;
using Cards.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Cards_WPF
{
    public partial class MainWindow : Window
    {
        public int NumberOfSticks { get; set; } = 5;
        public int NumberOfPlayedRounds { get; set; } = 1;
        public List<VisualStuff> VisualStuffList { get; set; }
        public List<CardPicture> CardPicturesList { get; set; } = new List<CardPicture>();

        public GameService GameService { get; set; }


        public MainWindow()
        {
            InitializeComponent();
            ClickOnHumanCards();

            CreateCroppedBitmapCards(CardPicturesList);

            StartNewGame();

        }

        private void StartNewGame()
        {
            GameService = new GameService();

            GameService.ShowHumanStick += GameService_ShowHumanStick;
            GameService.PlayPlayerCard += GameService_PlayPlayerCard;
            GameService.InvalidSticksCount += GameService_InvalidSticksCount;

            StartGame_BackEnd();

            StartGame_FrontEnd(GameService);

        }

        private void GameService_PlayPlayerCard(object sender, Card e)
        {
            this.Dispatcher.Invoke(() =>
            {
                ShowHandText(GameService);
            });
        }

        private void GameService_InvalidSticksCount(object sender, int e)
        {
            MessageBox.Show("Invalid number of tricks! Can't be the same as the number of cards this round. \nPlease change your number of tricks.");
        }

        private void GameService_ShowHumanStick(object sender, int tricksCount)
        {
            if (sender is AiPlayer)
            {
                this.Dispatcher.Invoke(() =>
                {
                    var nameOfPlayer = (Player)sender;
                    switch (nameOfPlayer.Name)
                    {
                        case "West":
                            Label_WestnTricks.Content = $"({tricksCount})";
                            break;

                        case "North":
                            Label_NorthTricks.Content = $"({tricksCount})";
                            break;

                        case "East":
                            Label_EastnTricks.Content = $"({tricksCount})";
                            break;

                        //case "Player1":
                        //    Label_PlayaTricks.Content = $"({tricksCount})";
                        //    break;

                        default:
                            break;
                    }
                });
            }
            else
            {
                // OM VI KOMMER HIT EN ANDRA GÅNG SÅ VILL VI SE TILL SÅ ATT HUMAN SER KNAPPARNA
                this.Dispatcher.Invoke(() =>
                {
                    PlayCard_Button.IsEnabled = false;
                    PlayCard_Button.Visibility = Visibility.Hidden;
                    ChooseStick_Button.IsEnabled = true;
                    ChooseStick_Button.Visibility = Visibility.Visible;

                    var numberOfTricks = VisualStuffList.Count(x => x.CardSelected == true).ToString();
                    Label_PlayaTricks.Content = $"({numberOfTricks})";
                });
            }
        }

        public void StartGame_BackEnd()
        {
            Label_Number.Content = NumberOfPlayedRounds.ToString();

            GameService.CreateRound(NumberOfSticks);

            NumberOfPlayedRounds++;
        }

        private void StartGame_FrontEnd(GameService currentGame)
        {
            ShowHandText(currentGame);

            //ShowTricks(currentGame);

            ShowImageCards(currentGame);

            //ShowHighestTricks(currentGame, NumberOfSticks);

            if (currentGame.ValidHumanTricksCount == true)
            {
                MessageBox.Show("Last player's tricks equals the same as the amount of tricks for the current round...");
            }

        }



        private void ShowHandText(GameService currentGame)
        {
            ListBox_West.ItemsSource = new ObservableCollection<Card>(currentGame.Players.Find(name => name.Name == "West").Hand);
            ListBox_North.ItemsSource = new ObservableCollection<Card>(currentGame.Players.Find(name => name.Name == "North").Hand);
            ListBox_East.ItemsSource = new ObservableCollection<Card>(currentGame.Players.Find(name => name.Name == "East").Hand);
            ListBox_Player1.ItemsSource = new ObservableCollection<Card>(currentGame.Players.Find(name => name.Name == "Player1").Hand);
        }

        //private void ShowTricks(GameService currentGame)
        //{
        //    Label_WestnTricks.Content = $"({currentGame.TricksCount[0].Count})";
        //    Label_NorthTricks.Content = $"({currentGame.TricksCount[1].Count})";
        //    Label_EastnTricks.Content = $"({currentGame.TricksCount[2].Count})";
        //    Label_PlayaTricks.Content = $"({currentGame.TricksCount[3].Count})";
        //}

        private void ShowImageCards(GameService currentGame)
        {
            string cardNumber = "";
            for (int j = 0; j < currentGame.Players[3].Hand.Count; j++)
            {
                cardNumber = CardImageNumber(currentGame.Players[3].Hand.ElementAt(j));
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

        //private void ShowHighestTricks(GameService currentGame, int antalKortIRundan)
        //{
        //    // WHO HAS THE MOST TRICKS GOING IN ORDER OR STARTING WITH A NEW ONE EVERY ROUND?
        //    var numberOfTricks_West = currentGame.TricksCount[0].Count;
        //    var numberOfTricks_North = currentGame.TricksCount[1].Count;
        //    var numberOfTricks_East = currentGame.TricksCount[2].Count;
        //    var numberOfTricks_Player1 = currentGame.TricksCount[3].Count;

        //    var highestTricks = numberOfTricks_West; // FRÅGAN ÄR OM MAN VILL ATT OLIKA SPELARE SKA BÖRJA RUNDORNA
        //    var nameOfHighest = currentGame.Players[0]; // FÖR NU BÖRJAR BARA WEST HELA TIDEN

        //    for (int i = 0; i < currentGame.TricksCount.Count; i++)
        //    {
        //        if (highestTricks < currentGame.TricksCount[i].Count)
        //        {
        //            highestTricks = currentGame.TricksCount[i].Count;
        //            nameOfHighest = currentGame.Players[i];
        //        }
        //    }

        //    HighestTricks_Label.Content = $"{nameOfHighest.Name} har flest stick med {highestTricks}";
        //}

        private void StartAnotherRound_Click(object sender, RoutedEventArgs e)
        {
            StartGame_BackEnd();
            StartGame_FrontEnd(GameService);
        }

        private string CardImageNumber(Card cardToNum)
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

        private void ClickOnHumanCards()
        {
            VisualStuffList = new List<VisualStuff>
            {
                new VisualStuff
                {
                    CardName = "Image_Playa0",
                    CardIndex = 0,
                },
                new VisualStuff
                {
                    CardName = "Image_Playa1",
                    CardIndex = 1,
                },
                new VisualStuff
                {
                    CardName = "Image_Playa2",
                    CardIndex = 2,
                },
                new VisualStuff
                {
                    CardName = "Image_Playa3",
                    CardIndex = 3,
                },
                new VisualStuff
                {
                    CardName = "Image_Playa4",
                    CardIndex = 4,
                },
            };
        }

        private void Image_PlayaCard_MouseUp(object sender, MouseButtonEventArgs e)
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

            Label_PlayaTricks.Content = VisualStuffList.Count(x => x.CardSelected == true).ToString();
        }

        private void Image_Test_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var numberOfCards = CardPicturesList.Count;
            var randomCard = new Random();
            var randomNumber = randomCard.Next(0, 52);
            Image_Test.Source = CardPicturesList[randomNumber].Picture;
        }

        private void ChooseTricks_Click(object sender, RoutedEventArgs e)
        {
            PlayCard_Button.IsEnabled = true;
            PlayCard_Button.Visibility = Visibility.Visible;
            ChooseStick_Button.IsEnabled = false;
            ChooseStick_Button.Visibility = Visibility.Hidden;

            this.Dispatcher.Invoke(() =>
            {
                var numberOfTricks = 0;

                foreach (var humanCard in VisualStuffList)
                {
                    if (humanCard.CardSelected == true)
                    {
                        numberOfTricks++;
                    }
                }

                Label_PlayaTricks.Content = $"({numberOfTricks})";
            });

            var indexOfTricksCardsSelected = new List<int>();

            foreach (var humanCard in VisualStuffList)
            {
                if (humanCard.CardSelected)
                {
                    indexOfTricksCardsSelected.Add(humanCard.CardIndex);
                }

            }

            GameService.HumanPickedTricks(indexOfTricksCardsSelected);
        }

        private void PlayTricks_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CreateCroppedBitmapCards(List<CardPicture> cardPicturesList)
        {
            var spriteSheetService = new SpriteSheetService();
            spriteSheetService.CutImage(cardPicturesList);
        }

    }
}