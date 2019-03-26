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

        public override bool PlayOutCard(Player player, int numberOfSticksThisRound, List<Player> players, ref Card firstCardPlayed)
        {
            //// TODO: HÄR SKA VI GÖRA LOGIK SOM KOLLAR OM HUMAN HAR VALT ETT KORT SOM FAKTISKT ÄR VALID ATT SPELA UT

            Card cardToPlay = player.CardToPlay;

            if (player.CardToPlay != null) // OM HUMAN INTE HAR VALT NÅGOT KORT ATT SPELA UT, VALID OR NOT
            {
                if (firstCardPlayed == null) // OM DET INTE FANNS NÅGOT KORT SOM VAR SPELAT SEDAN TIDIGARE, HUMAN ÄR FÖRST
                {
                    firstCardPlayed = cardToPlay;
                    player.CardToPlay = cardToPlay;
                    player.Hand.Remove(cardToPlay);
                    player.TricksCount.Remove(cardToPlay);

                    return true;
                }
                else // NU MÅSTE HUMAN SPELA ENLIGT FÄRG (SUIT)
                {
                    var howManyCardInSameSuit = 0;

                    foreach (var card in player.Hand)
                    {
                        if (card.Suit == firstCardPlayed.Suit)
                        {
                            howManyCardInSameSuit++;
                        }
                    }

                    if (howManyCardInSameSuit != 0 && player.CardToPlay.Suit != firstCardPlayed.Suit) // Finns det kort i samma färg, samt är det man försöker spela inte i den färgen
                    {
                        return false;
                    }
                    else
                    {
                        player.CardToPlay = cardToPlay;
                        player.TricksCount.Remove(cardToPlay); // TODO: KOLLA ÖVER ALLT HUR JAG SKA GÖRA MED DEN HÄR OM DET INTE FINNS NÅGOT MATCHANDE ELEMENT I LISTORNA
                        player.Hand.Remove(cardToPlay);

                        return true;
                    }
                }
            }

            return false;
        }
    }
}