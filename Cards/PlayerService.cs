using System.Collections.Generic;
using Cards.Models;

namespace Cards
{
    public class PlayerService
    {
        public void CreatePlayers(List<Player> players)
        {
            Player west = new Player("West");
            players.Add(west);

            Player north = new Player("North");
            players.Add(north);

            Player east = new Player("East");
            players.Add(east);

            Player player1 = new Player("Player1");
            players.Add(player1);
        }

        public void OrderOfPlayers(List<Player> players, Player lastWinner, List<Player> orderOfPlayers)
        {
            var orderedList = new List<Player>();

            var indexInList = players.FindIndex(x => x.Name == lastWinner.Name);
            //var indexInList = players.IndexOf(lastWinner);

            orderedList.Add(players[indexInList]);

            for (int i = 0; i < players.Count - 1; i++)
            {
                indexInList += 1;

                if (indexInList > players.Count - 1)
                {
                    indexInList = 0;
                }

                orderedList.Add(players[indexInList]);
            }

            orderOfPlayers.Clear();
            foreach (var player in orderedList)
            {
                orderOfPlayers.Add(player);
            }
        }
    }
}
