using System.Collections.Generic;

namespace Cards.Models
{
    public class HumanPlayer : Player
    {
        public HumanPlayer(string name)
            : base(name)
        {
        }

        public override bool CheckIfTricksAreValid(Player player, int numberOfSticksThisRound, List<Player> players)
        {
            var lastPlayer = players.Count - 1;
            if (player == players[lastPlayer]) // KOLLAR OM DEN SPELARE SOM ANROPAR METODEN ÄR DEN SISTA, ANNARS SÅ BEHÖVER VI INTE BRY OSS VILKET NUMMER DEN VÄLJER
            {
                var totalPlayerSticks = new int();
                foreach (var spelare in players)
                {
                    totalPlayerSticks += spelare.TricksCount.Count;
                }

                if (numberOfSticksThisRound == totalPlayerSticks) // ÄR DET LIKA MÅNGA STICK TAGNA SOM OMGÅNGEN ÄR PÅ SÅ MÅSTE VI ÅTGÄRDA DEN HÄR
                {
                    return false;
                }
                return true;
            }
            return true;
        }

        public override Card PlayOutCard(Player player, int numberOfSticksThisRound, List<Player> players, Card firstCardPlayed)
        {
            // TODO: INPUT FROM FRONT-END!!!

            return null;
        }
    }
}
