﻿using System.Collections.Generic;

namespace Cards.Models
{
    public class HumanPlayer : Player
    {
        public HumanPlayer(string name)
            : base(name)
        {
        }

        public override bool CheckIfTricksAreValid(Player player, int numberOfSticksThisRound, List<List<Card>> tricksCount, List<Player> players)
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
                    return false;
                }
                return true;
            }
            return true;
        }

        public override bool PlayOutCard(Player player, int numberOfSticksThisRound, List<List<Card>> tricksCount, List<Player> players, Card firstCardPlayed)
        {

            //this.CardToPlay = new Card();

            // TODO: INPUT FROM FRONT-END!!!
            //throw new System.Exception("IMPLEMENT!!!");
            return true;
        }
    }
}
