using System;
using System.Collections.Generic;
using Cards.Models;

namespace Cards
{
    class TricksRound
    {
        public bool DecideTricksForAI(int numberOfSticksThisRound, List<List<Card>> tricksCount, List<Player> players)
        {
            var totalPlayerSticks = new int();
            foreach (var player in tricksCount)
            {
                totalPlayerSticks += player.Count;
            }

            if (numberOfSticksThisRound == totalPlayerSticks)
            {

                if (tricksCount[3].Count > 0)
                {
                    // TA BORT ETT KORT FRÅN DENNES HAND
                    var lowestCardRank = 15; // ESS är ju 14
                    var lowestCardSuit = 0; // HJÄRTER är ju 1

                    foreach (var card in tricksCount[3])
                    {
                        if ((int)card.Rank < lowestCardRank)
                        {
                            lowestCardRank = (int)card.Rank;
                            lowestCardSuit = (int)card.Suit;
                        }
                    }

                    // TA BORT DET LÄGSTA KORTET
                    var itemToRemove = tricksCount[3].Find(x => (int)x.Rank == lowestCardRank && (int)x.Suit == lowestCardSuit);
                    tricksCount[3].Remove(itemToRemove);
                    return true;
                }
                else
                {
                    if (numberOfSticksThisRound == 1)
                    {

                    }

                    // LÄGG TILL ETT KORT SÅ ATT DET INTE ALLA STICK SUMMERAS TILL RUNDANS NUMMER
                    var highestCardRank = 15; // ESS är ju 14
                    var highestCardSuit = 0; // HJÄRTER är ju 1

                    foreach (var card in tricksCount[3])
                    {
                        if ((int)card.Rank < highestCardRank)
                        {
                            highestCardRank = (int)card.Rank;
                            highestCardSuit = (int)card.Suit;
                        }
                    }

                    
                    var itemToAdd = tricksCount[3].Find(x => (int)x.Rank == highestCardRank && (int)x.Suit == highestCardSuit);
                    tricksCount[3].Add(itemToAdd);

                    return true;
                }

            }
            return false;
        }

        internal void DecideTricksForPlayer(List<Player> players)
        {
            if (players[3].Name == "Player1")
            {
                //throw new System.Exception("SPELAREN MÅSTE SKRIVA IN ETT GILTIGT STICK-NUMMER");
            }
        }

        public void PlayTricks(List<List<Card>> tricksCount, int numberOfSticksThisRound)
        {
            //throw new NotImplementedException();
        }
    }
}