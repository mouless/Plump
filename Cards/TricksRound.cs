using System.Collections.Generic;
using Cards.Models;

namespace Cards
{
    class TricksRound
    {
        public bool DecideTricks(int numberOfSticksThisRound, List<List<Card>> tricksCount, List<Player> orderOfPlayers, List<Player> players)
        {
            var totalPlayerSticks = new int();
            foreach (var player in tricksCount)
            {
                totalPlayerSticks += player.Count;
            }

            if (numberOfSticksThisRound == totalPlayerSticks)
            {
                // HITTA NAMNET PÅ DEN SOM ÄR SIST I ORDNINGEN OCH INTE FÅR ALLA STICK ATT SUMMERAS TILL RUNDANS NUMMER
                var nameLastPlayer = orderOfPlayers[3].Name;
                var indexLastPlayer = players.FindIndex(x => x.Name == nameLastPlayer);

                if (tricksCount[indexLastPlayer].Count > 0)
                {
                    // TA BORT ETT KORT FRÅN DENNES HAND
                    var lowestCardRank = 15; // ESS är ju 14
                    var lowestCardSuit = 0; // HJÄRTER är ju 1

                    foreach (var card in tricksCount[indexLastPlayer])
                    {
                        if ((int)card.Rank < lowestCardRank)
                        {
                            lowestCardRank = (int)card.Rank;
                            lowestCardSuit = (int)card.Suit;
                        }
                    }

                    // TA BORT DET LÄGSTA KORTET
                    var itemToRemove = tricksCount[indexLastPlayer].Find(x => (int)x.Rank == lowestCardRank && (int)x.Suit == lowestCardSuit);
                    tricksCount[indexLastPlayer].Remove(itemToRemove);
                    return true;
                }
                else
                {
                    // LÄGG TILL ETT KORT SÅ ATT DET INTE ALLA STICK SUMMERAS TILL RUNDANS NUMMER
                    var highestCardRank = 15; // ESS är ju 14
                    var highestCardSuit = 0; // HJÄRTER är ju 1

                    foreach (var player in players)
                    {

                    }

                    //foreach (var card in tricksCount[indexLastPlayer])
                    //{
                    //    if ((int)card.Rank > highestCardRank)
                    //    {
                    //        highestCardRank = (int)card.Rank;
                    //        highestCardSuit = (int)card.Suit;
                    //    }
                    //}

                    var itemToAdd = tricksCount[indexLastPlayer].Find(x => (int)x.Rank == highestCardRank && (int)x.Suit == highestCardSuit);
                    tricksCount[indexLastPlayer].Add(itemToAdd);

                    return true;
                }

            }
            return false;
        }

        public void PlayTricks(List<List<Card>> tricksCount, int numberOfSticksThisRound)
        {
            //throw new NotImplementedException();
        }
    }
}