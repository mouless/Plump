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

        public override bool PlayOutCard(Player player, int numberOfSticksThisRound, List<Player> players, ref Card firstCardPlayed)
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
                var tempTricksCount = player.TricksCount;
                var tempPlayerHand = new List<Card>();

                foreach (var card in player.Hand)
                {
                    if (!tempTricksCount.Exists(x => x == card)) // Skapar en lista med alla kort som inte finns i tricksCount
                    {
                        tempPlayerHand.Add(card);
                    }
                }


                if (player.Hand.Exists(x => x.Suit == leadingSuit)) // Det finns kort på hand i samma färg
                {
                    if (tempTricksCount.Exists(x => x.Suit == leadingSuit)) // Finns det ett STICK-kort i samma färg
                    {
                        theCardRank = 15; // För att kunna ta det lägsta stick-kortet
                        foreach (var card in tempTricksCount)
                        {
                            if (card.Suit == leadingSuit)
                            {
                                if (card.Rank > leadingRank) // Lägsta tillgängliga tricks-kort som är högre rank än det som ligger
                                {
                                    if ((int)card.Rank < theCardRank) // Finns det fler stick-kort som är högre än det som ligger
                                    {
                                        theCardRank = (int)card.Rank;
                                        cardToPlay = card;
                                    }

                                }
                                else if (!tempPlayerHand.Exists(x => x.Suit == leadingSuit)) // Om trick-kort är lägre rankat och hand-kort i samma färg är tom, så måste vi välja ett kort ändå
                                {
                                    if ((int)card.Rank < theCardRank) // Finns det fler stick-kort som är högre än det som ligger
                                    {
                                        theCardRank = (int)card.Rank;
                                        cardToPlay = card;
                                    }
                                }
                            }
                        }

                        if (cardToPlay != null) // Om vi hittat ett Tricks-kort så tar vi det, finns det inte går vi vidare till nästa if
                        {
                            player.CardToPlay = cardToPlay;
                            player.Hand.Remove(cardToPlay);
                            player.TricksCount.Remove(cardToPlay);
                            return true;
                        }
                    }

                    if (tempPlayerHand.Exists(x => x.Suit == leadingSuit)) // Finns HAND-kort i samma färg
                    {
                        theCardRank = 15; // För att kunna ta det lägsta stick-kortet
                        foreach (var card in tempPlayerHand)
                        {
                            if (card.Suit == leadingSuit)
                            {
                                if (card.Rank > leadingRank)
                                {
                                    if ((int)card.Rank < theCardRank) // Finns det fler hand-kort som är högre än det som ligger
                                    {
                                        theCardRank = (int)card.Rank;
                                        cardToPlay = card;
                                    }
                                }
                                else if ((int)card.Rank < theCardRank) // Finns det fler hand-kort som är högre än det som ligger
                                {
                                    theCardRank = (int)card.Rank;
                                    cardToPlay = card;
                                }
                            }
                        }
                        player.CardToPlay = cardToPlay;
                        player.Hand.Remove(cardToPlay);
                        player.TricksCount.Remove(cardToPlay);
                        return true;
                    }

                }
                else // Inga kort i samma färg, får spela vad man vill (gärna undvika stick-kort)
                {
                    theCardRank = 15; // För att kunna ta det lägsta kortet
                    foreach (var card in tempPlayerHand)
                    {
                        if (card.Rank > leadingRank)
                        {
                            if ((int)card.Rank < theCardRank)
                            {
                                theCardRank = (int)card.Rank;
                                cardToPlay = card;
                            }
                        }
                        else if ((int)card.Rank < theCardRank)
                        {
                            theCardRank = (int)card.Rank;
                            cardToPlay = card;
                        }
                    }

                    if (cardToPlay == null) // Om det inte fanns kort utan vi måste använda stick-kort
                    {
                        foreach (var card in tempTricksCount)
                        {
                            if (card.Rank > leadingRank)
                            {
                                if ((int)card.Rank < theCardRank)
                                {
                                    theCardRank = (int)card.Rank;
                                    cardToPlay = card;
                                }
                            }
                            else if ((int)card.Rank < theCardRank)
                            {
                                theCardRank = (int)card.Rank;
                                cardToPlay = card;
                            }
                        }
                    }

                    player.CardToPlay = cardToPlay;
                    player.Hand.Remove(cardToPlay);
                    player.TricksCount.Remove(cardToPlay);
                    return true;
                }
            }

            return false; // FÖRSTÅR INTE RIKTIGT VARFÖR DEN HÄR MÅSTE VARA HÄR???
        }
    }
}
