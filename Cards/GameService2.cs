using Cards.Models;
using System.Collections.Generic;
using System.Linq;

namespace Cards
{
    public class GameService2
    {
        public Game StartGame(List<Player> players)
        {
            var game = new Game(players);

            game.Start();

            return game;
        }
    }


    public class Game
    {
        private readonly List<Player> _players;
        private readonly List<GameTurn> _turns;


        public Game(List<Player> players)
        {
            _players = players;
            _turns = new List<GameTurn>();
        }

        public void Start()
        {
            StartGameTurn();
        }

        public void PlayPlayerRound(Player player)
        {
            var currentTurn = _turns.Last();

            if (currentTurn.AllPlayersHavePlayed())
            {
                // starta nästa runda
                // eventuellt avsluta spelet

                StartGameTurn();
            }
        }

        private void StartGameTurn()
        {
            var previousTurn = _turns.LastOrDefault();
            var orderedPlayers = _players;

            if (previousTurn != null)
            {
                // sortera spelare beroende på förra rundan
                // orderedPlayers.OrderBy
            }

            //var turn = new GameTurn(orderedPlayers);

            //_turns.Add(turn);

            //turn.Start();
        }
    }

    public class GameTurn
    {
        private readonly List<GamePlayer> _players;
        private readonly List<GamePlayer> _playedPlayers;

        public GameTurn(List<GamePlayer> players)
        {
            _players = players;
            _playedPlayers = new List<GamePlayer>();
        }

        public bool AllPlayersHavePlayed()
        {
            // TODO
            return true;
        }

        public void Start()
        {
            foreach (var player in _players)
            {
                player.DecideTricks(this);
            }

            // decidePlayerTricks
        }

        public void PlayPlayerRound(GamePlayer player)
        {
            if (!_players.Contains(player))
            {
                // spelare inte med i rundan
                throw new System.Exception("spelare inte med i rundan");
            }
        }
    }

    public class GameRound
    {

    }

    public class GamePlayer
    {
        private readonly Player player;

        public GamePlayer(Player player)
        {
            this.player = player;
        }

        public void DecideTricks(GameTurn turn)
        {

        }
    }
}
