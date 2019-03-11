using System;
using System.Collections.Generic;
using Cards.Models;

namespace Cards
{
    public class GameService
    {
        public static Random r;
        public MyState State { get; set; } = new MyState();
        public List<Card> DeckOfCards { get; set; } = new List<Card>();

        public List<Player> Players { get; set; } = new List<Player>();
        public List<Player> OrderOfPlayers { get; set; } = new List<Player>();
        public Player WhoGoesFirst { get; set; } = new Player("West");

        public List<List<Card>> TricksCount { get; set; } = new List<List<Card>>();

        public Card CardToPlay_Westn { get; set; }
        public Card CardToPlay_North { get; set; }
        public Card CardToPlay_Eastn { get; set; }
        public Card CardToPlay_Playa { get; set; }


        public void CreateRound(int numberOfSticksThisRound)
        {
            ResetVariableThings();

            var listOfCards = new Deck();
            DeckOfCards = listOfCards.CreateDeck(DeckOfCards);

            var players = new PlayerService();
            players.CreatePlayers(Players);

            var fördelaKort = new DealCards();
            fördelaKort.DistributeCards(Players, DeckOfCards, numberOfSticksThisRound);

            players.OrderOfPlayers(Players, WhoGoesFirst, OrderOfPlayers);

            var tricksCalculator = new TricksCalculator();
            tricksCalculator.HowManyTricks(Players, TricksCount, State);
        }

        private void ResetVariableThings()
        {
            r = new Random();
            TricksCount.Clear();
            DeckOfCards.Clear();
            Players.Clear();
        }

        public void PlayNextTrick(int numberOfSticksThisRound)
        {
            var order = new PlayerService();
            order.OrderOfPlayers(Players, WhoGoesFirst, OrderOfPlayers);

            var tricksRound = new TricksRound();
            tricksRound.PlayTricks(TricksCount, numberOfSticksThisRound);
        }

    }

    public class MyState
    {
        public bool FyraLikaYao { get; set; }
    }
}
