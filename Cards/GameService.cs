using Cards.Models;
using System.Collections.Generic;

namespace Cards
{
    public class GameService
    {
        public static bool FyraLikaYao { get; set; } = false;

        public static List<List<Card>> TricksCount { get; set; } = new List<List<Card>>();
        public static List<Card> DeckOfCards { get; set; } = new List<Card>();
        public static List<Player> Players { get; set; } = new List<Player>();

        public static Card CardToPlay_North { get; set; }
        public static Card CardToPlay_Eastn { get; set; }
        public static Card CardToPlay_South { get; set; }
        public static Card CardToPlay_Playa { get; set; }

        //ListBox_North.ItemsSource = north.Hand;
        //ListBox_East.ItemsSource = east.Hand;
        //ListBox_South.ItemsSource = south.Hand;
        //ListBox_Player1.ItemsSource = player1.Hand;

        public void StartNewGame()
        {
            // VILL BARA HA ETT ANROP TILL CARDS-Projektet OCH DEN SKA I SIN TUR RETURNERA RESULTATET. SÅ BEHÖVER INSTANSIERA CARD.CS i CARDS-Projektet...?
            DeckOfCards = Deck.CreateDeck(); // Static class/method

            Players = PlayerService.CreatePlayers();
            DealCards.DistributeCards();

            var tricksCalculator = new TricksCalculator();
            tricksCalculator.HowManyTricks();
            ShowTricks();

            ShowImageCards();

            PlayHighestCard();
            ShowHands();
        }
    }
}
