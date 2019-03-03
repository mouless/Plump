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
        public MainWindow()
        {
            InitializeComponent();

            var gameRound = new GameService();
            gameRound.StartNewGame();

            FrontEnd(gameRound);
        }

        // TODO: MÅSTE SKICKA MED DEN HÄR TILL FRONTEND PÅ NÅGOT SÄTT SEN
        //ListBox_North.ItemsSource = north.Hand;
        //ListBox_East.ItemsSource = east.Hand;
        //ListBox_South.ItemsSource = south.Hand;
        //ListBox_Player1.ItemsSource = player1.Hand;

        private void FrontEnd(GameService currentGame)
        {
            ShowTricks(currentGame);

            ShowImageCards(currentGame);

            PlayHighestCard(currentGame);
            ShowHands(currentGame);
        }

        private void ShowTricks(GameService currentGame)
        {
            Label_NorthTricks.Content = $"({currentGame.TricksCount[0].Count()})";
            Label_EastnTricks.Content = $"({currentGame.TricksCount[1].Count()})";
            Label_SouthTricks.Content = $"({currentGame.TricksCount[2].Count()})";
            Label_PlayaTricks.Content = $"({currentGame.TricksCount[3].Count()})";
        }

        private void ShowImageCards(GameService currentGame)
        {
            string cardNumber = "";
            for (int j = 0; j < currentGame.Players[0].Hand.Count; j++)
            {
                cardNumber = CardImageNumber(currentGame.Players[0].Hand[j]);
                Uri uri = new Uri($"C:\\Users\\Mouless\\Source\\Repos\\Plump\\Cards_WPF\\Graphics\\{cardNumber}.jpeg");
                //Uri uri = new Uri($"C:\\Users\\William Boquist\\Plump\\Cards_WPF\\Graphics\\{cardNumber}.jpeg");
                Image img = FindName("Image_North" + j) as Image;
                img.Source = new BitmapImage(uri);

            }
        }

        public void PlayHighestCard(GameService currentGame)
        {
            currentGame.CardToPlay_North = currentGame.Players[0].Hand.OrderByDescending(v => v.Rank).First();
            string cardNumber = CardImageNumber(currentGame.CardToPlay_North);
            var uri = new Uri($"C:\\Users\\Mouless\\Source\\Repos\\Plump\\Cards_WPF\\Graphics\\{cardNumber}.jpeg");
            Image_NorthPlayed.Source = new BitmapImage(uri);

            currentGame.CardToPlay_Eastn = currentGame.Players[1].Hand.OrderByDescending(v => v.Rank).First();
            cardNumber = CardImageNumber(currentGame.CardToPlay_Eastn);
            uri = new Uri($"C:\\Users\\Mouless\\Source\\Repos\\Plump\\Cards_WPF\\Graphics\\{cardNumber}.jpeg");
            Image_EastnPlayed.Source = new BitmapImage(uri);

            currentGame.CardToPlay_South = currentGame.Players[2].Hand.OrderByDescending(v => v.Rank).First();
            cardNumber = CardImageNumber(currentGame.CardToPlay_South);
            uri = new Uri($"C:\\Users\\Mouless\\Source\\Repos\\Plump\\Cards_WPF\\Graphics\\{cardNumber}.jpeg");
            Image_SouthPlayed.Source = new BitmapImage(uri);

            currentGame.CardToPlay_Playa = currentGame.Players[3].Hand.OrderByDescending(v => v.Rank).First();
            cardNumber = CardImageNumber(currentGame.CardToPlay_Playa);
            uri = new Uri($"C:\\Users\\Mouless\\Source\\Repos\\Plump\\Cards_WPF\\Graphics\\{cardNumber}.jpeg");
            Image_PlayaPlayed.Source = new BitmapImage(uri);
        }


        public void ShowHands(GameService currentGame)
        {
            Label_NorthHand.Content = currentGame.CardToPlay_North;
            Label_SouthHand.Content = currentGame.CardToPlay_South;
            Label_EastnHand.Content = currentGame.CardToPlay_Eastn;
            Label_PlayaHand.Content = currentGame.CardToPlay_Playa;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //HowManyRestarts();
        }

        //private void HowManyRestarts(GameService currentGame)
        //{
        //    int numberOfRestarts = 0;
        //    do
        //    {
        //        numberOfRestarts++;
        //        Label_Number.Content = numberOfRestarts.ToString();
        //        FrontEnd(currentGame);
        //        //MessageBox.Show("UNDVIK");
        //    } while (!currentGame.FyraLikaYao);

        //    currentGame.FyraLikaYao = false;
        //}

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //Start_New_Game();
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

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            //int numberOfRestarts = 100;
            //do
            //{
            //    numberOfRestarts--;
            //    Start_New_Game();
            //} while (numberOfRestarts != 0);
        }

    }
}
