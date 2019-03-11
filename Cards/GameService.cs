using System;
using System.Collections.Generic;
using Cards.Models;

namespace Cards
{
    public class GameService
    {
        public static readonly Random r = new Random();
        public MyState State { get; set; } = new MyState();
        public List<List<Card>> TricksCount { get; set; } = new List<List<Card>>();
        public List<Card> DeckOfCards { get; set; } = new List<Card>();
        public List<Player> Players { get; set; } = new List<Player>();

        public Card CardToPlay_North { get; set; }
        public Card CardToPlay_Eastn { get; set; }
        public Card CardToPlay_South { get; set; }
        public Card CardToPlay_Playa { get; set; }


        public void CreateRound(int numberOfSticksThisRound)
        {
            var listOfCards = new Deck();
            DeckOfCards = listOfCards.CreateDeck();

            Players = PlayerService.CreatePlayers();

            var fördelaKort = new DealCards();
            fördelaKort.DistributeCards(Players, DeckOfCards, numberOfSticksThisRound);

            var tricksCalculator = new TricksCalculator();
            tricksCalculator.HowManyTricks(Players, TricksCount, State);
        }

        public void TrickRound(int numberOfSticksThisRound)
        {

        }
    }

    public class MyState
    {
        public bool FyraLikaYao { get; set; }
    }
}
