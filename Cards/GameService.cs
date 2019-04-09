using Cards.Models;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Cards
{
    public class GameService
    {
        #region
        #region Events
        public event EventHandler<string> PresentTextInfo;
        public event EventHandler<int> ShowHumanStick;
        public event EventHandler<int> InvalidSticksCount;
        public event EventHandler<int> InvalidPlayedCard;
        public event EventHandler<Card> PlayPlayerCard;
        public event EventHandler<int> ResetPlayerCards;
        public event EventHandler<int> RoundIsFinished;
        public event EventHandler<Card> ShowPlayedCard;
        public event EventHandler<Player> UpdateCrowne;
        public AutoResetEvent HumanPlayerStickAwaiter { get; set; } = new AutoResetEvent(false);
        #endregion

        public bool ValidHumanTricksCount { get; set; } = false;
        public int NumberOfSticksThisRound { get; set; }

        public static Random r;
        public Scoreboard Scoreboarden { get; set; } = new Scoreboard();
        public PlayerScore PlayerScoreList { get; set; } = new PlayerScore();
        public Round Round { get; set; } = new Round();

        public MyState State { get; set; } = new MyState();
        public List<Card> DeckOfCards { get; set; } = new List<Card>();

        public List<Player> Players { get; set; } = new List<Player>();
        public Player WhoGoesFirst { get; set; } // DET HÄR VÄRDET SÄTTS I "ConfigureVariableThings()"
        public Player WhoStartsNextTrick { get; set; } = new HumanPlayer("Placeholder"); // DET HÄR VÄRDET SÄTTS I "ResetVariableThings"
        private int _indexOfWhoGoesFirst = -1;

        private Card _firstCardPlayed = new Card(Card.CardSuit.Hjärter, Card.CardRank.Två);

        #endregion

        public GameService()
        {
            PresentTextInfo += GameService_PresentTextInfo;
            ShowHumanStick += ShowHumanStick_Event;
            PlayPlayerCard += GameService_PlayPlayerCard;
            InvalidSticksCount += GameService_InvalidSticksCount;
            ShowPlayedCard += GameService_ShowPlayedCard;
            InvalidPlayedCard += GameService_InvalidPlayedCard;
            ResetPlayerCards += GameService_ResetPlayerCards;
            RoundIsFinished += GameService_RoundIsFinished;
            UpdateCrowne += GameService_UpdateCrowne;
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public void CreateRound(int numberOfSticksThisRound)
        {
            ConfigureVariableThings(ref _indexOfWhoGoesFirst, WhoGoesFirst, numberOfSticksThisRound);

            var kortleksLista = new Deck();
            DeckOfCards = kortleksLista.CreateDeck(DeckOfCards);

            var spelare = new PlayerService();
            spelare.CreatePlayers(Players);
            spelare.InitizialOrderOfPlayers(Players, WhoGoesFirst); // Korten delas ut till den spelare som börjar omgången (vilket kommer att roteras)

            var fördelaKort = new DealCards();
            fördelaKort.DistributeCards(Players, DeckOfCards, numberOfSticksThisRound);

            var task = StartFirstTurn();
        }

        private void EndRound()
        {
            // UPPDATERA SCOREBOARDEN NÄR ALLA KORT I EN OMGÅNG ÄR UTSPELADE
            UpdateScoreboard();

            RoundIsFinished.Invoke(this, 99);
        }


        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void ConfigureVariableThings(ref int indexOfFirstPlayer, Player whoGoesFirst, int numberOfSticksThisRound)
        {
            r = new Random();
            _firstCardPlayed = null;
            NumberOfSticksThisRound = numberOfSticksThisRound;
            DeckOfCards.Clear();
            Players.Clear();
            ValidHumanTricksCount = false;
            indexOfFirstPlayer++;
                       
            if (indexOfFirstPlayer >= 4)
            {
                indexOfFirstPlayer = 0;
            }
            switch (indexOfFirstPlayer)
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

        public Task StartFirstTurn()
        {
            return Task.Run(() =>
            {
                // ANROPA SPELARNA FÖR ATT BESTÄMMA ANTAL STICK
                DecidePlayerTricks();

                // KOLLA OM SPELARNA KAN TA DERAS ÖNSKADE STICK
                ValidatePlayerTricksCount();

                // SE TILL SÅ ATT DEN SPELARE SOM VANN FÖREGÅENDE STICK FÅR BÖRJA
                var spelare = new PlayerService();
                spelare.WhoGoesFirstHighestTricksAfterDealer(Players, WhoGoesFirst);
                UpdateCrowne.Invoke(this, Players[0]);

                // ANROPA SPELARNA FÖR ATT SPELA UT KORT
                PlayPlayerCards();

                // POÄNGSTÄLLNING SAMT ANROPA NÄSTA STICK/RUNDA
                EndTurn(); // End TURN!

            }).ContinueWith(x =>
            {
                // OM DET FINNS FLER KORT ATT SPELA UT I EN OMGÅNG
                var startingCount = Players[0].Hand.Count;
                for (int i = 0; i < startingCount; i++)
                {
                    StartFollowingRound();
                }

                UpdateCrowne.Invoke(this, WhoStartsNextTrick);
                EndRound(); // NÄR ALLA KORT PÅ HANDEN ÄR UTSPELADE
            });
        }

        private void StartFollowingRound()
        {
            _firstCardPlayed = null;

            // SE TILL SÅ ATT DEN SPELARE SOM VANN FÖREGÅENDE STICK FÅR BÖRJA
            var spelare = new PlayerService();
            spelare.InitizialOrderOfPlayers(Players, WhoStartsNextTrick);
            UpdateCrowne.Invoke(this, WhoStartsNextTrick);

            // ANROPA SPELARNA FÖR ATT SPELA UT KORT
            PlayPlayerCards();

            // POÄNGSTÄLLNING SAMT ANROPA NÄSTA STICK/RUNDA
            EndTurn(); // End TURN!
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        #region Kodbas


        private void PlayPlayerCards()
        {
            foreach (var player in Players)
            {
                if (player is AiPlayer)
                {
                    player.PlayOutCard(player, NumberOfSticksThisRound, Players, ref _firstCardPlayed);

                    ShowPlayedCard.Invoke(player, player.CardToPlay);
                    PlayPlayerCard.Invoke(player, player.CardToPlay); // Visa i Front-End att man spelat ut ett kort & ta bort det från "listan"

                    Thread.Sleep(888);
                }
                else if (player is HumanPlayer)
                {
                    HumanPlayerStickAwaiter.WaitOne(); // PAUSA SPELET och vänta på input (dvs att Human väljer vilket kort som denne vill spela)

                    var resultIsValid = false;

                    do
                    {
                        // MÅSTE SE TILL SÅ ATT SPELAREN FÖLJER FÄRG
                        resultIsValid = player.PlayOutCard(player, NumberOfSticksThisRound, Players, ref _firstCardPlayed);

                        // Väntar på att Human ska välja ett kort att spela
                        if (resultIsValid == false)
                        {
                            InvalidPlayedCard(player, 99); // Visa i Front-End att det man har valt är INVALID (Messagebox.Show)
                            HumanPlayerStickAwaiter.WaitOne();
                        }

                    } while (resultIsValid == false);

                    ShowPlayedCard.Invoke(player, player.CardToPlay);
                    PlayPlayerCard.Invoke(player, player.CardToPlay); // Visa i Front-End att man spelat ut ett kort & ta bort det från "listan"
                }
            }
        }

        private void ValidatePlayerTricksCount()
        {
            foreach (var player in Players)
            {
                var resultIsValid = false;

                do
                {
                    // Kolla ifall en spelare kan ta en viss mängd stick, AI borde alltid returnera TRUE
                    resultIsValid = player.CheckIfTricksAreValid(player, NumberOfSticksThisRound, Players);

                    // Ifall en Player har ett non-valid tricks count, gå in här
                    if (resultIsValid == false)
                    {
                        // Meddela Human att man har valt ett non-valid stick nummer (Messagebox.Show)
                        InvalidSticksCount.Invoke(player, 99);
                        // Visa upp resultatet av vilka stick som är valda
                        ShowHumanStick.Invoke(player, 99);
                        // Väntar på att Human ska välja ett nytt stick nummer
                        HumanPlayerStickAwaiter.WaitOne();
                    }

                } while (resultIsValid == false);
                player.NumberOfTricksChosen = player.TricksCount.Count;
            }
        }

        private void DecidePlayerTricks()
        {
            var tricksCalculator = new TricksCalculator();
            foreach (var player in Players)
            {
                if (player is AiPlayer)
                {
                    tricksCalculator.HowManyTricks(player, State);
                    var indexOfPlayer = Players.FindIndex(x => x.Name == player.Name);
                    var numberOfTricks = player.TricksCount.Count;

                    ShowHumanStick.Invoke(player, numberOfTricks);
                }
                else if (player is HumanPlayer)
                {
                    // Här visar vi upp hur många stick de andra spelarna har tagit innan Human får möjlighet
                    ShowHumanStick.Invoke(player, 99);
                    // Här väntar vi på input ifrån Human innan vi kan gå vidare
                    HumanPlayerStickAwaiter.WaitOne();
                }
            }
        }

        public void HumanPickedTricks(List<int> indexOfTrickCardsSelected)
        {
            var indexOfHumanPlayer = Players.FindIndex(x => x.Name == "Player1");
            // DET KAN HA FUNNITS NÅGOT VALT AV HUMAN SEDAN TIDIGARE, SÅ VI TÖMMER LISTAN FÖR ATT VARA PÅ DEN SÄKRA SIDAN
            Players[indexOfHumanPlayer].TricksCount.Clear();

            foreach (var index in indexOfTrickCardsSelected)
            {
                Players[indexOfHumanPlayer].TricksCount.Add(Players[indexOfHumanPlayer].Hand[index]);
            }

            HumanPlayerStickAwaiter.Set();
        }

        public void HumanPlayedCardCheckForValidity(string nameOfPlayedCard)
        {
            var indexOfHumanPlayer = Players.FindIndex(x => x.Name == "Player1");
            var humanPlayer = Players[indexOfHumanPlayer];
            Card cardToBePlayed = null;

            Regex reg = new Regex(@"\d+");

            var numberOfCard = reg.Match(nameOfPlayedCard);

            if (numberOfCard.Success)
            {
                var suitPart = numberOfCard.Value.Substring(0, 1);
                var rankPart = numberOfCard.Value.Substring(1);

                Card.CardSuit suit = (Card.CardSuit)Enum.Parse(typeof(Card.CardSuit), suitPart);
                Card.CardRank rank = (Card.CardRank)Enum.Parse(typeof(Card.CardRank), rankPart);

                cardToBePlayed = new Card(suit, rank);
            }

            var card = humanPlayer.Hand.Find(x => x.Rank == cardToBePlayed.Rank && x.Suit == cardToBePlayed.Suit);

            humanPlayer.CardToPlay = card;

            HumanPlayerStickAwaiter.Set();
        }

        private void UpdateScoreboard()
        {
            foreach (var player in Players)
            {
                var playerScoren = new PlayerScore();
                playerScoren.Name = player.Name;
                Scoreboarden.PlayerScoreList.Add(playerScoren);
            }

            foreach (var playerScore in Scoreboarden.PlayerScoreList)
            {
                var playerTricksCount = Players.Find(x => x.Name == playerScore.Name).NumberOfTricksChosen;
                var playerTricksWon = Players.Find(x => x.Name == playerScore.Name).TricksWon;
                var roundName = NumberOfSticksThisRound;

                var newRound = new Round
                {
                    RoundName = roundName,
                    RoundScore = 0,
                    TricksCount = playerTricksCount,
                    TricksWon = playerTricksWon,
                };

                if (playerTricksCount == playerTricksWon)
                {
                    newRound.RoundScore = 10 + playerTricksWon;
                }

                playerScore.RoundList.Add(newRound);
                playerScore.TotalScore += newRound.RoundScore;
            }
        }


        #endregion

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public void EndTurn()
        {
            var winningCard = _firstCardPlayed;
            Player winningPlayer = null;
            Player playerThatWon = null;

            foreach (var player in Players)
            {
                if (player.CardToPlay.Rank > winningCard.Rank && player.CardToPlay.Suit == winningCard.Suit)
                {
                    winningCard = player.CardToPlay;
                }
            }

            if (winningCard != _firstCardPlayed) // Någon spelade ett högre kort än det som spelades först
            {
                winningPlayer = Players.Find(x => x.CardToPlay == winningCard);
                playerThatWon = Players.Find(x => x == winningPlayer);
                playerThatWon.TricksWon++;
            }
            else // Det första kortet vann rundan
            {
                winningPlayer = Players.Find(x => x.CardToPlay == _firstCardPlayed);
                playerThatWon = Players[0];
                playerThatWon.TricksWon++;
            }

            Round.RoundName = NumberOfSticksThisRound;
            
            ResetPlayerCards.Invoke(this, 99);
            WhoStartsNextTrick = playerThatWon;
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // EVENTS

        private void GameService_PresentTextInfo(object sender, string e) { }

        private void ShowHumanStick_Event(object sender, int i) { }

        private void GameService_PlayPlayerCard(object sender, Card e) { }

        private void GameService_InvalidSticksCount(object sender, int e) { }

        private void GameService_ShowPlayedCard(object sender, Card e) { }

        private void GameService_InvalidPlayedCard(object sender, int e) { }

        private void GameService_ResetPlayerCards(object sender, int e) { }

        private void GameService_RoundIsFinished(object sender, int e) { }

        private void GameService_UpdateCrowne(object sender, Player e) { }

        public void MoveOn()
        {
            HumanPlayerStickAwaiter.Set();
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public class MyState
    {
        public bool FyraLikaYao { get; set; }
    }
}
