using System;
using System.Collections.Generic;
using Cards.Models;

namespace Cards
{
    public class GameService
    {
        public static readonly Random r = new Random();
        public bool FyraLikaYao { get; set; } = false;

        public List<List<Card>> TricksCount { get; set; } = new List<List<Card>>();
        public List<Card> DeckOfCards { get; set; } = new List<Card>();
        public List<Player> Players { get; set; } = new List<Player>();

        public Card CardToPlay_North { get; set; }
        public Card CardToPlay_Eastn { get; set; }
        public Card CardToPlay_South { get; set; }
        public Card CardToPlay_Playa { get; set; }

        // TODO: MÅSTE SKICKA MED DEN HÄR TILL FRONTEND PÅ NÅGOT SÄTT SEN
        //ListBox_North.ItemsSource = north.Hand;
        //ListBox_East.ItemsSource = east.Hand;
        //ListBox_South.ItemsSource = south.Hand;
        //ListBox_Player1.ItemsSource = player1.Hand;

        public void StartNewGame()
        {
            var listOfCards = new Deck();
            DeckOfCards = listOfCards.CreateDeck();

            Players = PlayerService.CreatePlayers();

            var fördelaKort = new DealCards();
            fördelaKort.DistributeCards(Players, DeckOfCards);

            var tricksCalculator = new TricksCalculator();
            tricksCalculator.HowManyTricks(Players, TricksCount);
            //ShowTricks();

            //ShowImageCards();

            //PlayHighestCard();
            //ShowHands();
        }
    }
}
