using System.Collections.Generic;
using System.Linq;
using Cards.Models;

namespace Cards
{
    public class TricksCalculator
    {
        public void HowManyTricks(List<Player> players, List<List<Card>> tricksCount, MyState state)
        {
            List<Card> WestnTricks = new List<Card>(new List<Card>());
            tricksCount.Add(WestnTricks);
            List<Card> NorthTricks = new List<Card>(new List<Card>());
            tricksCount.Add(NorthTricks);
            List<Card> EastnTricks = new List<Card>(new List<Card>());
            tricksCount.Add(EastnTricks);
            List<Card> PlayaTricks = new List<Card>(new List<Card>());
            tricksCount.Add(PlayaTricks);

            //BytUtKortenIPlayaListan(players[3]);


            // Plockar ut alla kort som är över KNEKT som "säkra" kort
            for (int i = 0; i < 4; i++)
            {
                tricksCount[i] = players[i].Hand.Where(v => v.Rank > Card.CardRank.Knekt).ToList();
            }

            for (int i = 0; i < 4; i++)
            {
                if (tricksCount[i].Count == 0)
                {
                    int num = GameService.r.Next(0, 2);
                    if (num == 1) //Bara varannan gång så väljer AI att ta Knekt som ett "säkert" stickkort
                        tricksCount[i] = players[i].Hand.Where(v => v.Rank > Card.CardRank.Tio).ToList();
                }
                else if (tricksCount[i].Count == 1) //Om man har ett "säkert" kort på handen
                {
                    tricksCount[i] = KollaOmDetFinnsKnektEllerLägreIFärgstege(players[i].Hand.ToList(), tricksCount[i]);
                    if (tricksCount[i].Count == players[i].Hand.Count())
                        continue;

                    int num = GameService.r.Next(0, 2);
                    if (num == 1) //Bara varannan gång så väljer AI att ta Knekt som ett "säkert" stickkort
                        tricksCount[i] = players[i].Hand.Where(v => v.Rank > Card.CardRank.Tio).ToList();
                }
                else if (tricksCount[i].Count == 2) //Om man har två "säkra" kort på handen, så tar man även kort som är tio och uppåt i SAMMA färg
                {
                    tricksCount[i] = KollaOmDetFinnsKnektEllerLägreIFärgstege(players[i].Hand.ToList(), tricksCount[i]);
                    if (tricksCount[i].Count == players[i].Hand.Count())
                        continue;
                    tricksCount[i] = TaMedEnTiaSomStickOchKollaOmDetFinnsNiaISammaFärg(i, players, tricksCount);
                }
                else if (tricksCount[i].Count == 3) //Om man har tre "säkra" kort på handen
                {
                    tricksCount[i] = KollaOmDetFinnsKnektEllerLägreIFärgstege(players[i].Hand.ToList(), tricksCount[i]);
                    if (tricksCount[i].Count == players[i].Hand.Count())
                        continue;

                    if (tricksCount[i].GroupBy(c => c.Suit).Select(grp =>
                    {
                        int antal = grp.Count();
                        return new { grp.Key, antal };
                    }).All(c => c.antal != 3)) //Om någon färg har tre "säkra" kort
                    {
                        if (KollaOmDetFinnsFyraKortISammaFärg(players[i].Hand.ToList())) //Metod som kollar om 4st kort utav 5st är i samma färg
                        {
                            state.FyraLikaYao = true;

                            tricksCount[i] = players[i].Hand.ToList();
                            continue;
                        }
                    }
                    else { } //Om man kommer hit så har man inte varit i överstående IF

                    int num = GameService.r.Next(0, 2);
                    if (num == 1) //Varannan gång så tar man tior och över
                    {
                        tricksCount[i] = players[i].Hand.Where(v => v.Rank > Card.CardRank.Nio).ToList();
                    }
                    else //Varannan gång så tar man bara tior och över om de är i samma färg
                    {
                        List<Card> AllaMedSammaFärgSomDeÖverKnekt = players[i].Hand.Where(v => tricksCount[i].Select(c => c.Suit).Contains(v.Suit)).ToList();
                        AllaMedSammaFärgSomDeÖverKnekt.RemoveAll(x => x.Rank < Card.CardRank.Tio);
                        tricksCount[i] = AllaMedSammaFärgSomDeÖverKnekt;
                    }
                }
                else if (tricksCount[i].Count == 4) //Om man har fyra "säkra" kort på handen
                {
                    tricksCount[i] = FyraKortIEnFärgMedEssOchKung(players[i].Hand.ToList(), tricksCount[i]);
                    if (tricksCount[i].Count == players[i].Hand.Count())
                        continue;

                    tricksCount[i] = FyraSäkraVaravTvåIEnFärgManHarTreKortAv(players[i].Hand.ToList(), tricksCount[i]);
                    if (tricksCount[i].Count == players[i].Hand.Count())
                        continue;

                    tricksCount[i] = KollaOmDetFinnsKnektEllerLägreIFärgstege(players[i].Hand.ToList(), tricksCount[i]);
                    if (tricksCount[i].Count == players[i].Hand.Count())
                        continue;

                    tricksCount[i] = players[i].Hand.Where(v => v.Rank > Card.CardRank.Nio).ToList();
                }
            }

            // MAN BEHÖVER JU INTE RETURNERA NÅGOT EFTERSOM JAG SKICKADE IN REFERENSEN TILL TRICKSCOUNT-LISTAN
        }

        private List<Card> TaMedEnTiaSomStickOchKollaOmDetFinnsNiaISammaFärg(int i, List<Player> players, List<List<Card>> tricksCount)
        {
            List<Card> tempList = new List<Card>(players[i].Hand);
            List<Card> AllaMedSammaFärgSomDeÖverKnekt = players[i].Hand.Where(v => tricksCount[i].Select(c => c.Suit).Contains(v.Suit)).ToList();
            AllaMedSammaFärgSomDeÖverKnekt.RemoveAll(x => x.Rank < Card.CardRank.Tio);
            tempList.OrderByDescending(r => r.Rank);
            List<Card> temporärAllaMedLista = new List<Card>(AllaMedSammaFärgSomDeÖverKnekt);

            foreach (var card in temporärAllaMedLista)
            {
                if (card.Rank == Card.CardRank.Tio)
                {
                    foreach (var kort in tempList)
                    {
                        if (kort.Rank == Card.CardRank.Nio && kort.Suit == card.Suit)
                        {
                            AllaMedSammaFärgSomDeÖverKnekt.Add(kort); //????????
                        }
                    }
                }
            }

            return AllaMedSammaFärgSomDeÖverKnekt; // TODO - Lägga till om tian har en nia i samma färg lägg även till den då
        }

        // Om man har fyra kort i en färg och man har Ess och Kung i den färgen = 5 säkra stick
        private List<Card> FyraKortIEnFärgMedEssOchKung(List<Card> cardList, List<Card> tricksCount)
        {
            var q = cardList.GroupBy(c => c.Suit).Where(d => d.Count() >= 4).Select(f => f.Key);
            if (!q.Any())
            {
                return tricksCount; // SKA returnera de gamla stick-korten. Vi vill inte spara något nytt.
            }

            bool essISammaFärg = false;
            bool knugISammaFärg = false;
            foreach (var card in cardList)
            {
                if (card.Rank == Card.CardRank.Ess && q.Contains(card.Suit))
                {
                    essISammaFärg = true;
                }
                else if (card.Rank == Card.CardRank.Knug && q.Contains(card.Suit))
                {
                    knugISammaFärg = true;
                }

                if (essISammaFärg && knugISammaFärg)
                    return cardList;
            }

            return tricksCount;
        }

        private List<Card> FyraSäkraVaravTvåIEnFärgManHarTreKortAv(List<Card> cardList, List<Card> tricksCount)
        {
            // Är två stickkort i samma färg?
            var q = tricksCount.GroupBy(c => c.Suit).Where(d => d.Count() >= 2).Select(f => f.Key);
            if (!q.Any())
            {
                return tricksCount; // SKA returnera de gamla stick-korten. Vi vill inte spara något nytt.
            }

            // IF TRUE => Är det icke stick-kortet i samma färg som två av stick-korten
            if (q.Contains(cardList.OrderByDescending(c => c.Rank).Last().Suit))
            {
                return cardList; // Vi har hittat ett extra kort som vi vill ha med, nämligen det sista.
            }
            return tricksCount;
        }

        // TODO - Fixa så att den håller koll på vilken färg den Knekten är i så att det inte finns fler Knektar som det rekursiverar sig vidare på sen...
        private List<Card> KollaOmDetFinnsKnektEllerLägreIFärgstege(List<Card> cardList, List<Card> tricksCount)
        {
            List<Card> tempCardList = new List<Card>(cardList.OrderByDescending(c => c.Rank).ToList());
            var damSuit = tempCardList[0].Suit;
            for (int i = 0; i < tempCardList.Count; i++)
            {
                if (tempCardList[0].Rank != Card.CardRank.Dam)
                    return tricksCount;

                if (tempCardList[i].Rank == Card.CardRank.Dam - i && tempCardList[i] != tempCardList[0] && tempCardList[i].Suit == damSuit)
                {
                    tricksCount.Add(tempCardList[i]);
                }
            }

            return tricksCount;
        }

        private bool KollaOmDetFinnsFyraKortISammaFärg(List<Card> playerList)
        {
            var ÄrAllaKortUtomEttISammaFärg = playerList
                .GroupBy(c => c.Suit)
                .Select(grp =>
                {
                    int antal = grp.Count();
                    return new { grp.Key, antal };
                });

            string s = "---";
            foreach (var item in ÄrAllaKortUtomEttISammaFärg)
            {
                s += item.antal + ", " + item.Key + "---";
            }
            //MessageBox.Show(s);
            if (ÄrAllaKortUtomEttISammaFärg.Any(c => c.antal == 4))
            {
                //MessageBox.Show("Fyra lika yao!");
                return true;
            }
            return false;
        }

        private void BytUtKortenIPlayaListan(Player player)
        {
            player.Hand.Clear();
            player.Hand.Add(new Card(Card.CardSuit.Spader, Card.CardRank.Ess));
            player.Hand.Add(new Card(Card.CardSuit.Spader, Card.CardRank.Dam));
            player.Hand.Add(new Card(Card.CardSuit.Spader, Card.CardRank.Tio));
            player.Hand.Add(new Card(Card.CardSuit.Spader, Card.CardRank.Nio));
            player.Hand.Add(new Card(Card.CardSuit.Hjärter, Card.CardRank.Tio));
        }
    }
}
