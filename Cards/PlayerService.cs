using Cards.Models;
using System.Collections.Generic;

namespace Cards
{
    public static class PlayerService
    {
        public static List<Player> CreatePlayers()
        {
            var players = new List<Player>();

            Player north = new Player("North");
            ListBox_North.ItemsSource = north.Hand;
            players.Add(north);

            Player east = new Player("East");
            ListBox_East.ItemsSource = east.Hand;
            players.Add(east);

            Player south = new Player("South");
            ListBox_South.ItemsSource = south.Hand;
            players.Add(south);

            Player player1 = new Player("Player1");
            ListBox_Player1.ItemsSource = player1.Hand;
            players.Add(player1);

            return players;
        }
    }
}
