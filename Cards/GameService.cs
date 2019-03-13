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

            var listOfCards = new Deck();
            DeckOfCards = listOfCards.CreateDeck(DeckOfCards);

            var players = new PlayerService();
            players.CreatePlayers(Players);
            players.OrderOfPlayers(Players, WhoGoesFirst); // För att korten ska delas ut till förste spelaren först och inte i en annan ordning

            var fördelaKort = new DealCards();
            fördelaKort.DistributeCards(Players, DeckOfCards, numberOfSticksThisRound);

            var tricksCalculator = new TricksCalculator();
            tricksCalculator.HowManyTricks(Players, TricksCount, State);

            // MÅSTE GÖRA DET HÄR STEGVIS EFTERSOM MAN SOM SPELARE INTE SKA KUNNA SE DE VAD AI-SPELARNA HAR VALT FÖR NÅGRA STICKS

            PlayNextTrick();
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

        public void PlayNextTrick()
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
