using Cards.Models;
using System.Collections.Generic;

namespace Cards
{
    class TricksRound
    {
        public bool DecideTricksForPlayer(Player player, int numberOfSticksThisRound, List<List<Card>> tricksCount, List<Player> players)
        {
            var lastPlayer = players.Count - 1;
            if (player == players[lastPlayer]) // KOLLAR OM DEN SPELARE SOM ANROPAR METODEN ÄR DEN SISTA, ANNARS SÅ BEHÖVER VI INTE BRY OSS VILKET NUMMER DEN VÄLJER
            {
                var totalPlayerSticks = new int();
                foreach (var spelare in tricksCount)
                {
                    totalPlayerSticks += spelare.Count;
                }

                if (numberOfSticksThisRound == totalPlayerSticks) // ÄR DET LIKA MÅNGA STICK TAGNA SOM OMGÅNGEN ÄR PÅ SÅ MÅSTE VI ÅTGÄRDA DEN HÄR
                {
                    if (player is HumanPlayer)
                    {
                        return true; // VI RETURNERAR TRUE OM DET ÄR SPELAREN SOM MÅSTE VÄLJA ETT "VALID" STICKNUMMER
                    }
                    else
                    {
                        DecideTricksForAI(numberOfSticksThisRound, tricksCount, player, lastPlayer);
                    }
                }
            }
            return false;
        }

        private void DecideTricksForAI(int numberOfSticksThisRound, List<List<Card>> tricksCount, Player player, int lastPlayerIndex)
        {
            if (tricksCount[lastPlayerIndex].Count > 0) // TODO: Kolla hur den här beter sig när det bara finns två kort på handen...
            {
                var lowestCardRank = 15; // ESS är ju 14
                var lowestCardSuit = 0; // HJÄRTER är ju 1

                // TODO: MAN KANSKE VILL TA BORT ETT KORT AV EN ANNAN FÄRG OM DET ÄR LIKA LÅGT RANKAD SOM ETT ANNAT KORT SOM HAR EN KOMPIS I SAMMA FÄRG
                foreach (var card in tricksCount[lastPlayerIndex]) // KOLLA VILKET KORT SOM ÄR DET LÄGSTA RANKADE
                {
                    if ((int)card.Rank < lowestCardRank)
                    {
                        lowestCardRank = (int)card.Rank;
                        lowestCardSuit = (int)card.Suit;
                    }
                }

                // TA BORT DET LÄGSTA KORTET
                var itemToRemove = tricksCount[lastPlayerIndex].Find(x => (int)x.Rank == lowestCardRank && (int)x.Suit == lowestCardSuit);
                tricksCount[lastPlayerIndex].Remove(itemToRemove);
            }
            else // OM SPELAREN INTE HAR VALT NÅGOT STICK SÅ MÅSTE DEN NU VÄLJA MINST ETT
            {
                var highestCardRank = 15; // ESS är ju 14
                var highestCardSuit = 0; // HJÄRTER är ju 1

                foreach (var card in tricksCount[lastPlayerIndex])
                {
                    if ((int)card.Rank < highestCardRank)
                    {
                        highestCardRank = (int)card.Rank;
                        highestCardSuit = (int)card.Suit;
                    }
                }

                // LÄGG TILL ETT KORT SÅ ATT INTE ALLA STICK SUMMERAS TILL RUNDANS NUMMER
                var itemToAdd = tricksCount[lastPlayerIndex].Find(x => (int)x.Rank == highestCardRank && (int)x.Suit == highestCardSuit);
                tricksCount[lastPlayerIndex].Add(itemToAdd);
            }
        }

        public void PlayTricks(List<List<Card>> tricksCount, int numberOfSticksThisRound)
        {
            //throw new NotImplementedException();
        }
    }
}