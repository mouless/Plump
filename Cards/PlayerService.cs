using Cards.Models;
using System.Collections.Generic;

namespace Cards
{
    public class PlayerService
    {
        public void CreatePlayers(List<Player> players)
        {
            Player west = new AiPlayer("West");
            players.Add(west);

            Player north = new AiPlayer("North");
            players.Add(north);

            Player east = new AiPlayer("East");
            players.Add(east);

            Player player1 = new HumanPlayer("Player1");
            players.Add(player1);
        }

        public void InitizialOrderOfPlayers(List<Player> players, Player playerThatGoesFirst)
        {
            var tempOrderedList = new List<Player>();

            var indexInList = players.FindIndex(x => x.Name == playerThatGoesFirst.Name);

            tempOrderedList.Add(players[indexInList]);

            for (int i = 0; i < players.Count - 1; i++)
            {
                indexInList++;

                if (indexInList > players.Count - 1)
                {
                    indexInList = 0;
                }

                tempOrderedList.Add(players[indexInList]);
            }

            players.Clear();
            foreach (var player in tempOrderedList)
            {
                players.Add(player);
            }
        }

        private void OrderTrickCountList(List<List<Card>> tricksCount, int indexOfFirstTricksCount)
        {
            var tempOrderedList = new List<List<Card>>();
            
            tempOrderedList.Add(tricksCount[indexOfFirstTricksCount]);

            for (int i = 0; i < tricksCount.Count - 1; i++)
            {
                indexOfFirstTricksCount++;

                if (indexOfFirstTricksCount > tricksCount.Count - 1)
                {
                    indexOfFirstTricksCount = 0;
                }

                tempOrderedList.Add(tricksCount[indexOfFirstTricksCount]);
            }

            tricksCount.Clear();
            foreach (var tricksCountList in tempOrderedList)
            {
                tricksCount.Add(tricksCountList);
            }
        }

        public void WhoGoesFirstHighestTricksAfterDealer(List<Player> players, Player playerThatGoesFirst, List<List<Card>> tricksCount)
        {
            var highestTrick = -1;
            var indexOfHighestTricksCountListItem = new int();

            foreach (var playerTricks in tricksCount)
            {
                if (playerTricks.Count > highestTrick)
                {
                    highestTrick = playerTricks.Count;
                    indexOfHighestTricksCountListItem = tricksCount.FindIndex(x => x == playerTricks);
                }
            }

            // ORDNA OM PLAYER-LISTAN
            var indexOfPlayerThatShouldStart = players[indexOfHighestTricksCountListItem];
            InitizialOrderOfPlayers(players, indexOfPlayerThatShouldStart);

            // MÅSTE OCKSÅ ORDNA OM TRICKSCOUNT-LISTAN SÅ ÄVEN DEN ÄR I RÄTT ORDNING
            OrderTrickCountList(tricksCount, indexOfHighestTricksCountListItem);

        }
    }
}