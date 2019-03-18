using Cards.Models;
using System;
using System.Collections.Generic;

namespace Cards
{
    public class GameService
    {
        #region
        public bool PlayerIsHuman { get; set; } = false;
        public int NumberOfSticksThisRound { get; set; }

        public static Random r;
        public Dictionary<int, Scoreboard> Scoreboard { get; set; } = new Dictionary<int, Scoreboard>();
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
        #endregion

        public Action<Player> OnPlayerTurnStarted { get; set; }


        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///
        public void CreateRound(int numberOfSticksThisRound)
        {
            ConfigureVariableThings(IndexOfWhoGoesFirst, WhoGoesFirst, numberOfSticksThisRound);

            var kortleksLista = new Deck();
            DeckOfCards = kortleksLista.CreateDeck(DeckOfCards);

            var spelare = new PlayerService();
            spelare.CreatePlayers(Players);
            spelare.InitizialOrderOfPlayers(Players, WhoGoesFirst); // Korten delas ut till den spelare som börjar omgången (vilket kommer att roteras)

            var fördelaKort = new DealCards();
            fördelaKort.DistributeCards(Players, DeckOfCards, numberOfSticksThisRound);

            // FOR-LOOP FÖR ATT GÅ IGENOM ALLA SPELARE - AI OCH HUMAN HAR SAMMA METOD, MEN DESSA GÖR OLIKA SAKER BARA
            StartRound();
            // EN ANNAN FOR-LOOP FÖR ATT SPELA UT KORTEN
            // EN AVSLUTANDE METOD FÖR ATT HÅLLA REDA PÅ STÄLLNINGEN
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void ConfigureVariableThings(int indexOfWhoGoesFirst, Player whoGoesFirst, int numberOfSticksThisRound)
        {
            r = new Random();
            Scoreboard.Add(numberOfSticksThisRound, new Scoreboard());
            NumberOfSticksThisRound = numberOfSticksThisRound;
            TricksCount.Clear();
            DeckOfCards.Clear();
            Players.Clear();
            PlayerIsHuman = false;
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

        public void StartRound()
        {
            var whoGoesFirst = Players[0];
            // ANROPA DEN SPELAREN SÅ DEN KAN SPELA UT SITT KORT
            foreach (var player in Players)
            {
                player.BestämmaStick();
            }
        }

        private void StartPlayerTurn(Player player)
        {
            OnPlayerTurnStarted?.Invoke(player);

            // TODO:
            // 1. Räkna ut antalet stick
            // 2. 
            // 3. 
            // 4. 
            // 5. 
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public void DecideHowManyTricksToTake(Player player)
        {
            var tricksCalculator = new TricksCalculator();
            tricksCalculator.HowManyTricks(new Player, TricksCount, State);
        }

        public bool CheckIfTricksCountIsValid(Player player)
        {
            var tricksRound = new TricksRound();
            PlayerIsHuman = tricksRound.DecideTricksForPlayer(player, NumberOfSticksThisRound, TricksCount, Players);

            if (PlayerIsHuman == true)
            {
                // TODO: BE OM ETT TRICKS COUNT
                return true;
            }
            return false;
        }

        public void OrderPlayerByTricks()
        {
            var spelare = new PlayerService();
            spelare.InitizialOrderOfPlayers(Players, WhoGoesFirst); // Korten delas ut till den spelare som börjar omgången (vilket kommer att roteras)
        }

        public void PlayCard(Player player, object card)
        {
            // validera att en spelare har kortet dom vill spela

            // end current player turn

            var tricksRound = new TricksRound();
            tricksRound.PlayTricks(player, TricksCount, NumberOfSticksThisRound);

            Player nextPlayer;
            // notify next player of turn start
            //StartPlayerTurn(nextPlayer);
        }

        public void EndGame()
        {
            var score = Scoreboard[numberOfSticksThisRound];

        }
    }

    public class MyState
    {
        public bool FyraLikaYao { get; set; }
    }
}
