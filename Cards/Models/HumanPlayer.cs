using System.Collections.Generic;

namespace Cards.Models
{
    public class HumanPlayer : Player
    {
        public HumanPlayer(string name) 
            : base(name)
        {
        }

        public override void CheckIfTricksAreValid(Player player, int numberOfSticksThisRound, List<List<Card>> tricksCount, List<Player> players)
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

                    // TODO: INPUT FROM FRONT-END!!!
                    throw new System.Exception("IMPLEMENT!!!");
                }
            }
        }

        public override void PlayOutCard(Player player, int numberOfSticksThisRound, List<List<Card>> tricksCount, List<Player> players)
        {
            // TODO: INPUT FROM FRONT-END!!!
            throw new System.Exception("IMPLEMENT!!!");
        }
    }
}
