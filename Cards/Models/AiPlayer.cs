using System.Collections.Generic;

namespace Cards.Models
{
    public class AiPlayer : Player
    {
        public AiPlayer(string name)
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

                    ChangeToValidTricksCount(numberOfSticksThisRound, player, lastPlayer, players);
                }
            }
            return true;
        }

        private void ChangeToValidTricksCount(int numberOfSticksThisRound, Player player, int lastPlayerIndex, List<Player> players)
        {
            if (players[lastPlayerIndex].TricksCount.Count > 0) // TODO: Kolla hur den här beter sig när det bara finns två kort på handen...
            {
                var lowestCardRank = 15; // ESS är ju 14
                var lowestCardSuit = 0; // HJÄRTER är ju 1

                // TODO: MAN KANSKE VILL TA BORT ETT KORT AV EN ANNAN FÄRG OM DET ÄR LIKA LÅGT RANKAD SOM ETT ANNAT KORT SOM HAR EN KOMPIS I SAMMA FÄRG
                foreach (var card in players[lastPlayerIndex].TricksCount) // KOLLA VILKET KORT SOM ÄR DET LÄGSTA RANKADE
                {
                    if ((int)card.Rank < lowestCardRank)
                    {
                        lowestCardRank = (int)card.Rank;
                        lowestCardSuit = (int)card.Suit;
                    }
                }

                // TA BORT DET LÄGSTA KORTET
                var cardToRemove = players[lastPlayerIndex].TricksCount.Find(x => (int)x.Rank == lowestCardRank && (int)x.Suit == lowestCardSuit);
                players[lastPlayerIndex].TricksCount.Remove(cardToRemove);
            }
            else // OM SPELAREN INTE HAR VALT NÅGOT STICK SÅ MÅSTE DEN NU VÄLJA MINST ETT
            {
                var highestCardRank = 15; // ESS är ju 14
                var highestCardSuit = 0; // HJÄRTER är ju 1

                foreach (var card in players[lastPlayerIndex].TricksCount) // KOLLA VILKET KORT SOM ÄR DET HÖGST RANKADE
                {
                    if ((int)card.Rank < highestCardRank)
                    {
                        highestCardRank = (int)card.Rank;
                        highestCardSuit = (int)card.Suit;
                    }
                }
                // TODO: Man kanske vill kunna lägga till fler än ett kort om andra spelare inte har valt några stick tex...


                // LÄGG TILL ETT KORT SÅ ATT INTE ALLA STICK SUMMERAS TILL RUNDANS NUMMER
                var itemToAdd = players[lastPlayerIndex].TricksCount.Find(x => (int)x.Rank == highestCardRank && (int)x.Suit == highestCardSuit);
                players[lastPlayerIndex].TricksCount.Add(itemToAdd);
            }
        }

        public override bool PlayOutCard(Player player, int numberOfSticksThisRound, List<Player> players, Card firstCardPlayed)
        {
            var theCardRank = new int();
            Card cardToPlay = null;

            if (firstCardPlayed == null) // DÅ ÄR SPELAREN DEN SOM FÅR ÄRAN ATT BÖRJA SPELA UT ETT KORT
            {
                // VÄLJ VILKET KORT FRÅN "TRICKS-COUNT HANDEN"
                if (player.TricksCount.Count != 0)
                {
                    foreach (var card in player.TricksCount) // TA DET HÖGSTA KORTET VARJE GÅNG
                    {
                        if ((int)card.Rank > theCardRank)
                        {
                            theCardRank = (int)card.Rank;
                            cardToPlay = card;
                        }
                    }

                    firstCardPlayed = cardToPlay; // HÄR SÄTTER VI DET KORTET SOM BLIR FÖRST LAGT PÅ BORDET I RUNDAN
                    player.CardToPlay = cardToPlay;
                    player.TricksCount.Remove(cardToPlay);
                    player.Hand.Remove(cardToPlay);
                    return true;
                }
                else
                {
                    // HÄR SPELAR VI ETT KORT I FRÅN SPELARENS "HAND"
                    foreach (var card in player.Hand) // TA DET HÖGSTA KORTET VARJE GÅNG
                    {
                        if ((int)card.Rank > theCardRank)
                        {
                            theCardRank = (int)card.Rank;
                            cardToPlay = card;
                        }
                    }

                    firstCardPlayed = cardToPlay; // HÄR SÄTTER VI DET KORTET SOM BLIR FÖRST LAGT PÅ BORDET I RUNDAN
                    player.CardToPlay = cardToPlay;
                    player.Hand.Remove(cardToPlay);
                    player.TricksCount.Remove(cardToPlay);
                    return true;
                }
            }
            else // NU MÅSTE SPELARE FÖRSÖKA VINNA ÖVER 
            {
                var leadingSuit = firstCardPlayed.Suit;
                var leadingRank = firstCardPlayed.Rank;

                // TODO: SE TILL ATT AI TAR ETT KORT I SAMMA FÄRG FAST LÄGRE RANK FÖR ATT INTE "SLÖSA" PÅ SINA STICK-KORT
                // TODO: JAG BEHÖVER JU HA ALLA KORT SOM LIGGER PÅ BORDET FÖR ATT VETA VILKET KORT JAG BEHÖVER JÄMFÖRA MED

                if (player.TricksCount.Count != 0) // ANVÄND TRICKS-CARD OM DET FINNS NÅGRA I SAMMA FÄRG OCH SOM ÄR HÖGRE RANK
                {
                    var anyCardInSameSuit = 0;
                    foreach (var card in player.TricksCount) // KOLLA HUR MÅNGA DET FINNS I DEN HÄR FÄRGEN
                    {
                        if (card.Suit == leadingSuit)
                        {
                            anyCardInSameSuit++;
                        }
                    }

                    if (anyCardInSameSuit != 0) // FINNS DET NÅGRA STICK-KORT I DEN HÄR FÄRGEN
                    {
                        foreach (var card in player.TricksCount)
                        {
                            if (card.Suit == leadingSuit && (int)card.Rank > theCardRank) // OM KORTET ÄR I SAMMA FÄRG SAMT VILKEN RANK
                            {
                                theCardRank = (int)card.Rank;
                                cardToPlay = card;
                            }
                        }

                        if (cardToPlay.Rank < leadingRank) // ÄR KORTET HÖGRE RANKAT ÄN DET SOM LIGGER
                        {
                            player.CardToPlay = cardToPlay;
                            player.TricksCount.Remove(cardToPlay);
                            player.Hand.Remove(cardToPlay);
                            return true;
                        }
                        else // ÄR KORTET LÄGRE RANKAT
                        {
                            foreach (var card in player.Hand) // TA DET HÖGSTA KORTET FRÅN HANDEN ISTÄLLET VILKET KANSKE INTE ÄR ETT STICK-KORT
                            {
                                if (card.Suit == leadingSuit && (int)card.Rank > theCardRank)
                                {
                                    theCardRank = (int)card.Rank;
                                    cardToPlay = card;
                                }
                            }

                            player.CardToPlay = cardToPlay;
                            player.Hand.Remove(cardToPlay);
                            player.TricksCount.Remove(cardToPlay);
                            return true;
                        }
                    }
                    else // HIT KOMMER VI BARA OM DET INTE FINNS NÅGOT KORT PÅ HAND I DEN FÄRGEN SOM ÄR SPELAD
                    {
                        foreach (var card in player.Hand) // TA DET LÄGSTA KORTET FRÅN HANDEN ISTÄLLET VILKET KANSKE INTE ÄR ETT STICK-KORT
                        {
                            if (player.TricksCount.Exists(x => x.Rank == card.Rank && x.Suit == card.Suit))
                            {
                                continue;
                            }
                            if ((int)card.Rank < theCardRank)
                            {
                                theCardRank = (int)card.Rank;
                                cardToPlay = card;
                            }
                        }

                        player.CardToPlay = cardToPlay;
                        player.Hand.Remove(cardToPlay);
                        player.TricksCount.Remove(cardToPlay);
                        return true;
                    }
                }
                else
                {
                    var anyCardInSameSuit = 0;
                    foreach (var card in player.Hand) // KOLLA HUR MÅNGA DET FINNS I DEN HÄR FÄRGEN
                    {
                        if (card.Suit == leadingSuit)
                        {
                            anyCardInSameSuit++;
                        }
                    }

                    if (anyCardInSameSuit != 0)
                    {

                        foreach (var card in player.Hand)
                        {
                            if (card.Suit == leadingSuit && (int)card.Rank > theCardRank)
                            {
                                theCardRank = (int)card.Rank;
                                cardToPlay = card;
                            }
                        }

                        player.CardToPlay = cardToPlay;
                        player.Hand.Remove(cardToPlay);
                        player.TricksCount.Remove(cardToPlay);
                        return true;
                    }
                    else // HÄR FINNS DET INGA KORT PÅ HAND SOM ÄR I SAMMA FÄRG SOM DET SOM LIGGER, DVS FÅR LÄGGA VAD VI VILL
                    {
                        foreach (var card in player.Hand)
                        {
                            if ((int)card.Rank < theCardRank) // SÅ JUST HÄR VÄLJER VI DET LÄGSTA KORTET ATT SPELA UT
                            {
                                theCardRank = (int)card.Rank;
                                cardToPlay = card;
                            }
                        }

                        player.CardToPlay = cardToPlay;
                        player.Hand.Remove(cardToPlay);
                        player.TricksCount.Remove(cardToPlay);
                        return true;
                    }
                }
            }
        }
    }
}
