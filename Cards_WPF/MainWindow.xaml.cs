using Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Cards_WPF
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var gameRound = new GameService();
            gameRound.StartNewGame();

            Start_New_Game();
        }


        private void Start_New_Game()
        {
            // VILL BARA HA ETT ANROP TILL CARDS-Projektet OCH DEN SKA I SIN TUR RETURNERA RESULTATET. SÅ BEHÖVER INSTANSIERA CARD.CS i CARDS-Projektet...?
            InitializeDeck();

            var tricksCalculator = new TricksCalculator();
            DeckOfCards = ShuffleDeck(DeckOfCards);

            CreatePlayers();
            DealCards();

            tricksCalculator.HowManyTricks();
            ShowTricks();

            ShowImageCards();

            PlayHighestCard();
            ShowHands();
        }

        private void ShowImageCards()
        {
            string cardNumber = "";
            for (int j = 0; j < Players[0].Hand.Count; j++)
            {
                cardNumber = CardImageNumber(Players[0].Hand[j]);
                Uri uri = new Uri($"C:\\Users\\Mouless\\Source\\Repos\\Plump\\Cards_WPF\\Graphics\\{cardNumber}.jpeg");
                //Uri uri = new Uri($"C:\\Users\\William Boquist\\Plump\\Cards_WPF\\Graphics\\{cardNumber}.jpeg");
                Image img = FindName("Image_North" + j) as Image;
                img.Source = new BitmapImage(uri);

            }
        }

        private void ShowTricks()
        {
            Label_NorthTricks.Content = $"({TricksCount[0].Count()})";
            Label_EastnTricks.Content = $"({TricksCount[1].Count()})";
            Label_SouthTricks.Content = $"({TricksCount[2].Count()})";
            string text = $"({TricksCount[3].Count()})";

            Label_PlayaTricks.Content = text;
        }

        public void PlayHighestCard()
        {
            CardToPlay_North = Players[0].Hand.OrderByDescending(v => v.Rank).First();
            string cardNumber = CardImageNumber(CardToPlay_North);
            var uri = new Uri($"C:\\Users\\Mouless\\Source\\Repos\\Plump\\Cards_WPF\\Graphics\\{cardNumber}.jpeg");
            Image_NorthPlayed.Source = new BitmapImage(uri);

            CardToPlay_Eastn = Players[1].Hand.OrderByDescending(v => v.Rank).First();
            cardNumber = CardImageNumber(CardToPlay_Eastn);
            uri = new Uri($"C:\\Users\\Mouless\\Source\\Repos\\Plump\\Cards_WPF\\Graphics\\{cardNumber}.jpeg");
            Image_EastnPlayed.Source = new BitmapImage(uri);

            CardToPlay_South = Players[2].Hand.OrderByDescending(v => v.Rank).First();
            cardNumber = CardImageNumber(CardToPlay_South);
            uri = new Uri($"C:\\Users\\Mouless\\Source\\Repos\\Plump\\Cards_WPF\\Graphics\\{cardNumber}.jpeg");
            Image_SouthPlayed.Source = new BitmapImage(uri);

            CardToPlay_Playa = Players[3].Hand.OrderByDescending(v => v.Rank).First();
            cardNumber = CardImageNumber(CardToPlay_Playa);
            uri = new Uri($"C:\\Users\\Mouless\\Source\\Repos\\Plump\\Cards_WPF\\Graphics\\{cardNumber}.jpeg");
            Image_PlayaPlayed.Source = new BitmapImage(uri);
        }


        

        

        //public void CreatePlayers()
        //{
        //    Players = new List<Player>();

        //    Player north = new Player("North");
        //    ListBox_North.ItemsSource = north.Hand;
        //    Players.Add(north);

        //    Player east = new Player("East");
        //    ListBox_East.ItemsSource = east.Hand;
        //    Players.Add(east);

        //    Player south = new Player("South");
        //    ListBox_South.ItemsSource = south.Hand;
        //    Players.Add(south);

        //    Player player1 = new Player("Player1");
        //    ListBox_Player1.ItemsSource = player1.Hand;
        //    Players.Add(player1);
        //}

        //public void DealCards()
        //{
        //    for (int i = 0; i < 5; i++)
        //    {
        //        foreach (var player in Players)
        //        {
        //            player.Hand.Add(DeckOfCards[0]);
        //            DeckOfCards.RemoveAt(0);
        //        }
        //    }
        //    foreach (var player in Players)
        //    {
        //        var nyHand = player.Hand.OrderBy(c => c.Suit).ThenByDescending(c => c.Rank).ToList();
        //        player.Hand.Clear();
        //        foreach (var card in nyHand)
        //        {
        //            player.Hand.Add(card);
        //        }
        //    }
        //}

        public void ShowHands()
        {
            Label_NorthHand.Content = CardToPlay_North;
            Label_SouthHand.Content = CardToPlay_South;
            Label_EastnHand.Content = CardToPlay_Eastn;
            Label_PlayaHand.Content = CardToPlay_Playa;
        }

        private void HowManyRestarts()
        {
            int numberOfRestarts = 0;
            do
            {
                numberOfRestarts++;
                Label_Number.Content = numberOfRestarts.ToString();
                Start_New_Game();
                //MessageBox.Show("UNDVIK");
            } while (!FyraLikaYao);

            FyraLikaYao = false;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            HowManyRestarts();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Start_New_Game();
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

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            int numberOfRestarts = 100;
            do
            {
                numberOfRestarts -= 1;
                Start_New_Game();
            } while (numberOfRestarts != 0);
        }

        private void BytUtKortenIPlayaListan(Player player)
        {
            player.Hand.Clear();
            player.Hand.Add(new Card(Card.CardSuit.Spader, Card.CardRank.Ess));
            player.Hand.Add(new Card(Card.CardSuit.Spader, Card.CardRank.Dam));
            player.Hand.Add(new Card(Card.CardSuit.Spader, Card.CardRank.Tio));
            player.Hand.Add(new Card(Card.CardSuit.Spader, Card.CardRank.Nio));
            player.Hand.Add(new Card(Card.CardSuit.Hjärter, Card.CardRank.Tio));
        }

        //private List<Card> idk(List<Card> cardList, List<Card> tricksCount, Card current)
        //{
        //    Card NyttKortSomViVillHaMed = cardList.SingleOrDefault(c => c.Suit == current.Suit && c.Rank == current.Rank - 1);

        //    if (NyttKortSomViVillHaMed == null)
        //    {
        //        return tricksCount;
        //    }

        //    tricksCount.Add(NyttKortSomViVillHaMed);
        //    return idk(cardList, tricksCount, NyttKortSomViVillHaMed);
        //}
    }
}
