using Cards.Models;
using System;
using System.Collections.Generic;

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
        public Player WhoGoesFirst { get; set; } // DET HÄR VÄRDET SÄTTS I "ResetVariableThings"
        public int IndexOfWhoGoesFirst { get; set; } = -1;

        public List<List<Card>> TricksCount { get; set; } = new List<List<Card>>();

        public Card CardToPlay_Westn { get; set; }
        public Card CardToPlay_North { get; set; }
        public Card CardToPlay_Eastn { get; set; }
        public Card CardToPlay_Playa { get; set; }


        public Action<Player> OnPlayerTurnStarted { get; set; }


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


            // NOTIFY NEXT PLAYER OF TURN START
            Player nextPlayer = Players[0];
            StartPlayerTurn(nextPlayer);
        }

        private void StartPlayerTurn(Player player)
        {
            OnPlayerTurnStarted?.Invoke(player);
        }


        public void PlayCard(object player, object card)
        {
            // validera att en spelare har kortet dom vill spela

            // end current player turn


            Player nextPlayer;
            // notify next player of turn start
            //StartPlayerTurn(nextPlayer);
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
                    WhoGoesFirst = new AiPlayer("West");
                    break;
                case 1:
                    WhoGoesFirst = new AiPlayer("North");
                    break;
                case 2:
                    WhoGoesFirst = new AiPlayer("East");
                    break;
                case 3:
                    WhoGoesFirst = new HumanPlayer("Player1");
                    break;
                default:
                    break;
            }
        }

        public bool DecideTricks(Player player)
        {
            var tricksRound = new TricksRound();
            NotPossibleTricks = tricksRound.DecideTricksForPlayer(player, NumberOfSticksThisRound, TricksCount, Players);

            if (NotPossibleTricks == true)
            {
                return true;
            }
            return false;
        }

        public void OrderPlayerByTricks()
        {
            var spelare = new PlayerService();
            spelare.OrderOfPlayers(Players, WhoGoesFirst); // Korten delas ut till den spelare som börjar omgången (vilket kommer att roteras)

        }

        public void PlayTrick(Player whoGoesFirst)
        {
            var tricksRound = new TricksRound();
            tricksRound.PlayTricks(TricksCount, NumberOfSticksThisRound);
        }

    }

    public class MyState
    {
        public bool FyraLikaYao { get; set; }
    }
}
