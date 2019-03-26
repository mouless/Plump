using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cards.Models;

namespace Cards
{
    public class GameService
    {
        #region
        public event EventHandler<int> ShowHumanStick;
        public event EventHandler<int> InvalidSticksCount;
        public event EventHandler<int> InvalidPlayedCard;
        public event EventHandler<Card> PlayPlayerCard;
        public event EventHandler<Card> ShowPlayedCard;
        public AutoResetEvent HumanPlayerStickAwaiter { get; set; } = new AutoResetEvent(false);

        public bool ValidHumanTricksCount { get; set; } = false;
        public int NumberOfSticksThisRound { get; set; }

        public static Random r;
        public Dictionary<int, Scoreboard> Scoreboard { get; set; } = new Dictionary<int, Scoreboard>();
        public MyState State { get; set; } = new MyState();
        public List<Card> DeckOfCards { get; set; } = new List<Card>();

        public List<Player> Players { get; set; } = new List<Player>();
        public Player WhoGoesFirst { get; set; } // DET HÄR VÄRDET SÄTTS I "ResetVariableThings"
        public Player WhoStartsNextTrick { get; set; } = new HumanPlayer("Placeholder"); // DET HÄR VÄRDET SÄTTS I "ResetVariableThings"
        public int IndexOfWhoGoesFirst { get; set; } = -1;

        private Card _firstCardPlayed = new Card(Card.CardSuit.Hjärter, Card.CardRank.Två);

        public Card CardToPlay_Westn { get; set; }
        public Card CardToPlay_North { get; set; }
        public Card CardToPlay_Eastn { get; set; }
        public Card CardToPlay_Playa { get; set; }
        #endregion

        public GameService()
        {
            ShowHumanStick += ShowHumanStick_Event;
            PlayPlayerCard += GameService_PlayPlayerCard;
            InvalidSticksCount += GameService_InvalidSticksCount;
            ShowPlayedCard += GameService_ShowPlayedCard;
            InvalidPlayedCard += GameService_InvalidPlayedCard;
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

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

            StartRound();

            //EndGame();
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void ConfigureVariableThings(int indexOfWhoGoesFirst, Player whoGoesFirst, int numberOfSticksThisRound)
        {
            r = new Random();
            _firstCardPlayed = null;
            Scoreboard.Add(numberOfSticksThisRound, new Scoreboard());
            NumberOfSticksThisRound = numberOfSticksThisRound;
            DeckOfCards.Clear();
            Players.Clear();
            ValidHumanTricksCount = false;
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
            Task.Run(() =>
            {
                // ANROPA SPELARNA FÖR ATT BESTÄMMA ANTAL STICK
                DecidePlayerTricks();

                // KOLLA OM SPELARNA KAN TA DERAS ÖNSKADE STICK
                ValidatePlayerTricksCount();

                // SE TILL SÅ ATT DEN SPELARE SOM HAR HÖGST STICK EFTER "DEALER" FÅR BÖRJA SPELA UT
                var spelare = new PlayerService();
                spelare.WhoGoesFirstHighestTricksAfterDealer(Players, WhoGoesFirst);


                // ANROPA SPELARNA FÖR ATT SPELA UT KORT
                PlayPlayerCards();
                EndGame();
            });

        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void PlayPlayerCards()
        {
            foreach (var player in Players)
            {
                if (player is AiPlayer)
                {
                    player.PlayOutCard(player, NumberOfSticksThisRound, Players, ref _firstCardPlayed);

                    ShowPlayedCard.Invoke(player, player.CardToPlay);
                    PlayPlayerCard.Invoke(player, player.CardToPlay); // Visa i Front-End att man spelat ut ett kort & ta bort det från "listan"
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

        public void HumanPlayedCardCheckForValidity(int indexOfPlayedCard)
        {
            var indexOfHumanPlayer = Players.FindIndex(x => x.Name == "Player1");
            var humanPlayer = Players[indexOfHumanPlayer];

            humanPlayer.CardToPlay = humanPlayer.Hand[indexOfPlayedCard];

            HumanPlayerStickAwaiter.Set();
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


        public void EndGame()
        {
            var score = Scoreboard[NumberOfSticksThisRound];
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
            }
            else // Det första kortet vann rundan
            {
                winningPlayer = Players.Find(x => x.CardToPlay == _firstCardPlayed);
                playerThatWon = Players[0];
            }

            WhoStartsNextTrick = playerThatWon;
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void ShowHumanStick_Event(object sender, int i) { }

        private void GameService_PlayPlayerCard(object sender, Card e) { }

        private void GameService_InvalidSticksCount(object sender, int e) { }

        private void GameService_ShowPlayedCard(object sender, Card e) { }

        private void GameService_InvalidPlayedCard(object sender, int e) { }

    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public class MyState
    {
        public bool FyraLikaYao { get; set; }
    }
}
