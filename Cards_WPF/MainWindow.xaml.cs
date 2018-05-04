﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Cards_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            InitializeDeck();
            DeckOfCards = ShuffleDeck(DeckOfCards);

            CreatePlayers();
            DealCards();
            HowManyTricks();
            ShowTricks();

            PlayHighestCard();
            ShowHands();
        }

        private void ShowTricks()
        {
            Label_NorthTricks.Content = $"({TricksCount[0].Count()})";
            Label_EastnTricks.Content = $"({TricksCount[1].Count()})";
            Label_SouthTricks.Content = $"({TricksCount[2].Count()})";
            Label_PlayaTricks.Content = $"({TricksCount[3].Count()})";
        }

        public List<List<Card>> TricksCount = new List<List<Card>>();
        private void HowManyTricks()
        {
            List<Card> NorthTricks = new List<Card>(new List<Card>());
            TricksCount.Add(NorthTricks);
            List<Card> EastnTricks = new List<Card>(new List<Card>());
            TricksCount.Add(EastnTricks);
            List<Card> SouthTricks = new List<Card>(new List<Card>());
            TricksCount.Add(SouthTricks);
            List<Card> PlayaTricks = new List<Card>(new List<Card>());
            TricksCount.Add(PlayaTricks);

            // TODO - i 66% av fallen ta med en Knekt som ett stick???
            for (int i = 0; i < 4; i++)
            {
                TricksCount[i] = Players[i].Hand.Where(v => v.Rank > Card.CardRank.Knekt).ToList();
            }

            for (int i = 0; i < 4; i++)
            {
                if (TricksCount[i].Count == 0)
                {
                    int num = r.Next(0, 2);
                    if (num == 1) //Bara varannan gång så väljer AI att ta Knekt som ett "säkert" stickkort
                        TricksCount[i] = Players[i].Hand.Where(v => v.Rank > Card.CardRank.Tio).ToList();
                }
                else if (TricksCount[i].Count == 1) //Om man har ett "säkert" kort på handen
                {
                    int num = r.Next(0, 2);
                    if (num == 1) //Bara varannan gång så väljer AI att ta Knekt som ett "säkert" stickkort
                        TricksCount[i] = Players[i].Hand.Where(v => v.Rank > Card.CardRank.Tio).ToList();
                }
                else if (TricksCount[i].Count == 2) //Om man har två "säkra" kort på handen, så tar man även kort som är tio och uppåt i SAMMA färg
                {
                    List<Card> AllaMedSammaFärgSomDeÖverKnekt = Players[i].Hand.Where(v => TricksCount[i].Select(c => c.Suit).Contains(v.Suit)).ToList();
                    AllaMedSammaFärgSomDeÖverKnekt.RemoveAll(x => x.Rank < Card.CardRank.Tio);
                    TricksCount[i] = AllaMedSammaFärgSomDeÖverKnekt;
                }
                else if (TricksCount[i].Count == 3) //Om man har tre "säkra" kort på handen
                {
                    int num = r.Next(0, 2);
                    if (num == 1) //Varannan gång så tar man tior och över
                    {
                        List<Card> AllaMedSammaFärgSomDeÖverKnekt = Players[i].Hand.Where(v => v.Rank > Card.CardRank.Nio).ToList();
                        TricksCount[i] = AllaMedSammaFärgSomDeÖverKnekt;
                    }
                    else //Varannan gång så tar man bara tior och över om de är i samma färg
                    {
                        List<Card> AllaMedSammaFärgSomDeÖverKnekt = Players[i].Hand.Where(v => TricksCount[i].Select(c => c.Suit).Contains(v.Suit)).ToList();
                        AllaMedSammaFärgSomDeÖverKnekt.RemoveAll(x => x.Rank < Card.CardRank.Tio);
                        TricksCount[i] = AllaMedSammaFärgSomDeÖverKnekt;
                    }
                }
                else if (TricksCount[i].Count >= 4) //Om man har fyra "säkra" kort på handen
                {
                    List<Card> AllaMedSammaFärgSomDeÖverKnekt = Players[i].Hand.Where(v => v.Rank > Card.CardRank.Nio).ToList();
                    TricksCount[i] = AllaMedSammaFärgSomDeÖverKnekt;
                }
            }
        }

        public Card CardToPlay_North { get; set; }
        public Card CardToPlay_Eastn { get; set; }
        public Card CardToPlay_South { get; set; }
        public Card CardToPlay_Playa { get; set; }
        public void PlayHighestCard()
        {
            CardToPlay_North = Players[0].Hand.OrderByDescending(v => v.Rank).First();
            CardToPlay_South = Players[1].Hand.OrderByDescending(v => v.Rank).First();
            CardToPlay_Eastn = Players[2].Hand.OrderByDescending(v => v.Rank).First();
            CardToPlay_Playa = Players[3].Hand.OrderByDescending(v => v.Rank).First();
        }

        public List<Card> DeckOfCards { get; set; }
        static Random r = new Random();
        public List<Player> Players { get; set; }

        public void InitializeDeck()
        {
            DeckOfCards = new List<Card>();
            for (Card.CardSuit i = Card.CardSuit.Hjärter; i <= Card.CardSuit.Klöver; i++)
            {
                for (Card.CardRank j = Card.CardRank.Två; j <= Card.CardRank.Ess; j++)
                {
                    DeckOfCards.Add(new Card(i, j));
                }
            }
        }

        static public List<Card> ShuffleDeck(List<Card> deck)
        {
            for (int n = deck.Count - 1; n > 0; --n)
            {
                int k = r.Next(n + 1);
                var temp = deck[n];
                deck[n] = deck[k];
                deck[k] = temp;
            }
            return deck;
        }

        public void CreatePlayers()
        {
            Players = new List<Player>();

            Player north = new Player("North");
            ListBox_North.ItemsSource = north.Hand;
            Players.Add(north);

            Player east = new Player("East");
            ListBox_East.ItemsSource = east.Hand;
            Players.Add(east);

            Player south = new Player("South");
            ListBox_South.ItemsSource = south.Hand;
            Players.Add(south);

            Player player1 = new Player("Player1");
            ListBox_Player1.ItemsSource = player1.Hand;
            Players.Add(player1);
        }

        public void DealCards()
        {
            for (int i = 0; i < 5; i++)
            {
                foreach (var player in Players)
                {
                    player.Hand.Add(DeckOfCards[0]);
                    DeckOfCards.RemoveAt(0);
                }
            }
            foreach (var player in Players)
            {
                var nyHand = player.Hand.OrderBy(c => c.Suit).ThenBy(c => c.Rank).ToList();
                player.Hand.Clear();
                foreach (var card in nyHand)
                {
                    player.Hand.Add(card);
                }
            }
        }

        public void ShowHands()
        {
            Label_NorthHand.Content = CardToPlay_North;
            Label_SouthHand.Content = CardToPlay_South;
            Label_EastnHand.Content = CardToPlay_Eastn;
            Label_PlayaHand.Content = CardToPlay_Playa;
        }
    }
}
