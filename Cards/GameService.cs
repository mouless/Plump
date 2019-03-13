using System;
using System.Collections.Generic;
using Cards.Models;

namespace Cards
{
    public class GameService
    {
        public bool NotPossibleTricks { get; set; } = false;
        public int NumberOfSticksThisRound { get; set; }

        public static Random r;
        public MyState State { get; set; } = new MyState();
        public List<Card> DeckOfCards { get; set; } = new List<Card>();

        public List<Player> Players { get; set; } = new List<Player>();
        public Player WhoGoesFirst { get; set; }
        public int IndexOfWhoGoesFirst { get; set; } = -1;

        public List<List<Card>> TricksCount { get; set; } = new List<List<Card>>();

        public Card CardToPlay_Westn { get; set; }
        public Card CardToPlay_North { get; set; }
        public Card CardToPlay_Eastn { get; set; }
        public Card CardToPlay_Playa { get; set; }


        public void CreateRound(int numberOfSticksThisRound)
        {
            ResetVariableThings(IndexOfWhoGoesFirst, WhoGoesFirst, numberOfSticksThisRound);

            var kortleksLista = new Deck();
            DeckOfCards = kortleksLista.CreateDeck(DeckOfCards);

            var spelare = new PlayerService();
            spelare.CreatePlayers(Players);
            spelare.OrderOfPlayers(Players, WhoGoesFirst); // Korten delas ut till den spelare som börjar omgången (vilket kommer att roteras)

            var fördelaKort = new DealCards();
            fördelaKort.DistributeCards(Players, DeckOfCards, numberOfSticksThisRound);

            var tricksCalculator = new TricksCalculator();
            tricksCalculator.HowManyTricks(Players, TricksCount, State);
        }

        private void ResetVariableThings(int indexOfWhoGoesFirst, Player whoGoesFirst, int numberOfSticksThisRound)
        {
            r = new Random();
            NumberOfSticksThisRound = numberOfSticksThisRound;
            TricksCount.Clear();
            DeckOfCards.Clear();
            Players.Clear();
            NotPossibleTricks = false;
            indexOfWhoGoesFirst++;

            if (indexOfWhoGoesFirst >= Players.Count)
            {
                indexOfWhoGoesFirst = 0;
            }
            switch (indexOfWhoGoesFirst)
            {
                case 0:
                    WhoGoesFirst = new Player("West");
                    break;
                case 1:
                    WhoGoesFirst = new Player("North");
                    break;
                case 2:
                    WhoGoesFirst = new Player("East");
                    break;
                case 3:
                    WhoGoesFirst = new Player("Player1");
                    break;
                default:
                    break;
            }
        }

        public void PlayNextTrick(int IndexOfWhoGoesFirst)
        {
            var playerService = new PlayerService();
            playerService.OrderOfPlayers(Players, WhoGoesFirst);

            var tricksRound = new TricksRound();
            tricksRound.DecideTricksForPlayer(Players);
            NotPossibleTricks = tricksRound.DecideTricksForAI(NumberOfSticksThisRound, TricksCount, Players);

            tricksRound.PlayTricks(TricksCount, NumberOfSticksThisRound);
        }

    }

    public class MyState
    {
        public bool FyraLikaYao { get; set; }
    }
}
