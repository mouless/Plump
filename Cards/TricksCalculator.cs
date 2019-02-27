using System;
using System.Collections.Generic;
using System.Linq;

namespace Cards
{
    public class TricksCalculator
    {

        static Random r = new Random();

        public List<List<Card>> TricksCount { get; set; }
        public List<Card> DeckOfCards { get; set; }
        public List<Player> Players { get; set; }

        public Card CardToPlay_North { get; set; }
        public Card CardToPlay_Eastn { get; set; }
        public Card CardToPlay_South { get; set; }
        public Card CardToPlay_Playa { get; set; }

        public bool FyraLikaYao { get; set; } = false;
        //bool FyraLikaYao = false;

        public void HowManyTricks()
        {
            TricksCount = new List<List<Card>>();

            List<Card> NorthTricks = new List<Card>(new List<Card>());
            TricksCount.Add(NorthTricks);
            List<Card> EastnTricks = new List<Card>(new List<Card>());
            TricksCount.Add(EastnTricks);
            List<Card> SouthTricks = new List<Card>(new List<Card>());
            TricksCount.Add(SouthTricks);
            List<Card> PlayaTricks = new List<Card>(new List<Card>());
            TricksCount.Add(PlayaTricks);
            BytUtKortenIPlayaListan(Players[3]);

            // Plockar ut alla kort som är över KNEKT som "säkra" kort
            for (int i = 0; i < 4; i++)
            {
                TricksCount[i] = Players[i].Hand.Where(v => v.Rank > Card.CardRank.Knekt).ToList();
            }

            for (int i = 0; i < 4; i++)
            {
                if (TricksCount[i].Count == 0)
                {
                    int num = r.Next(0, 2);
                    if (num == 1) //Bara varannan gång så väljer AI att ta Knekt som ett "säkert" stickkort
                        TricksCount[i] = Players[i].Hand.Where(v => v.Rank > Card.CardRank.Tio).ToList();
                }
                else if (TricksCount[i].Count == 1) //Om man har ett "säkert" kort på handen
                {
                    TricksCount[i] = KollaOmDetFinnsKnektEllerLägreIFärgstege(Players[i].Hand.ToList(), TricksCount[i]);
                    if (TricksCount[i].Count == Players[i].Hand.Count())
                        continue;

                    int num = r.Next(0, 2);
                    if (num == 1) //Bara varannan gång så väljer AI att ta Knekt som ett "säkert" stickkort
                        TricksCount[i] = Players[i].Hand.Where(v => v.Rank > Card.CardRank.Tio).ToList();
                }
                else if (TricksCount[i].Count == 2) //Om man har två "säkra" kort på handen, så tar man även kort som är tio och uppåt i SAMMA färg
                {
                    TricksCount[i] = KollaOmDetFinnsKnektEllerLägreIFärgstege(Players[i].Hand.ToList(), TricksCount[i]);
                    if (TricksCount[i].Count == Players[i].Hand.Count())
                        continue;
                    TricksCount[i] = TaMedEnTiaSomStickOchKollaOmDetFinnsNiaISammaFärg(i);
                }
                else if (TricksCount[i].Count == 3) //Om man har tre "säkra" kort på handen
                {
                    TricksCount[i] = KollaOmDetFinnsKnektEllerLägreIFärgstege(Players[i].Hand.ToList(), TricksCount[i]);
                    if (TricksCount[i].Count == Players[i].Hand.Count())
                        continue;

                    if (TricksCount[i].GroupBy(c => c.Suit).Select(grp =>
                    {
                        int antal = grp.Count();
                        return new { grp.Key, antal };
                    }).All(c => c.antal != 3)) //Om någon färg har tre "säkra" kort
                    {
                        if (KollaOmDetFinnsFyraKortISammaFärg(Players[i].Hand.ToList())) //Metod som kollar om 4st kort utav 5st är i samma färg
                        {
                            TricksCount[i] = Players[i].Hand.ToList();
                            continue;
                        }
                    }
                    else { } //Om man kommer hit så har man inte varit i överstående IF

                    int num = r.Next(0, 2);
                    if (num == 1) //Varannan gång så tar man tior och över
                    {
                        List<Card> AllaMedSammaFärgSomDeÖverKnekt = Players[i].Hand.Where(v => v.Rank > Card.CardRank.Nio).ToList();
                        TricksCount[i] = AllaMedSammaFärgSomDeÖverKnekt;
                    }
                    else //Varannan gång så tar man bara tior och över om de är i samma färg
                    {
                        List<Card> AllaMedSammaFärgSomDeÖverKnekt = Players[i].Hand.Where(v => TricksCount[i].Select(c => c.Suit).Contains(v.Suit)).ToList();
                        AllaMedSammaFärgSomDeÖverKnekt.RemoveAll(x => x.Rank < Card.CardRank.Tio);
                        TricksCount[i] = AllaMedSammaFärgSomDeÖverKnekt;
                    }
                }
                else if (TricksCount[i].Count == 4) //Om man har fyra "säkra" kort på handen
                {
                    TricksCount[i] = FyraKortIEnFärgMedEssOchKung(Players[i].Hand.ToList(), TricksCount[i]);
                    if (TricksCount[i].Count == Players[i].Hand.Count())
                        continue;

                    TricksCount[i] = FyraSäkraVaravTvåIEnFärgManHarTreKortAv(Players[i].Hand.ToList(), TricksCount[i]);
                    if (TricksCount[i].Count == Players[i].Hand.Count())
                        continue;

                    TricksCount[i] = KollaOmDetFinnsKnektEllerLägreIFärgstege(Players[i].Hand.ToList(), TricksCount[i]);
                    if (TricksCount[i].Count == Players[i].Hand.Count())
                        continue;

                    List<Card> AllaMedSammaFärgSomDeÖverKnekt = Players[i].Hand.Where(v => v.Rank > Card.CardRank.Nio).ToList();
                    TricksCount[i] = AllaMedSammaFärgSomDeÖverKnekt;
                }
            }
        }

        private List<Card> TaMedEnTiaSomStickOchKollaOmDetFinnsNiaISammaFärg(int i)
        {
            List<Card> tempList = new List<Card>(Players[i].Hand);
            List<Card> AllaMedSammaFärgSomDeÖverKnekt = Players[i].Hand.Where(v => TricksCount[i].Select(c => c.Suit).Contains(v.Suit)).ToList();
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
                FyraLikaYao = true;
                return true;
            }
            return false;
        }

    }
}
