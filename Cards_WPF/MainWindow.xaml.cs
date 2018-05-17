using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Cards_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Start_New_Game();
        }

        private void Start_New_Game()
        {
            InitializeDeck();
            DeckOfCards = ShuffleDeck(DeckOfCards);

            CreatePlayers();
            DealCards();
            HowManyTricks();
            ShowTricks();

            ShowImageCards();

            PlayHighestCard();
            ShowHands();
        }

        private void ShowImageCards()
        {
            string cardNumber = "";
            for (int j = 0; j < Players[0].Hand.Count; j++)
            {
                cardNumber = CardImageNumber(Players[0].Hand[j]);
                Uri uri = new Uri($"C:\\Users\\William Boquist\\Plump\\Cards_WPF\\Graphics\\{cardNumber}.jpeg");
                Image img = FindName("Image_North" + j) as Image;
                img.Source = new BitmapImage(uri);

            }
        }

        private void ShowTricks()
        {
            Label_NorthTricks.Content = $"({TricksCount[0].Count()})";
            Label_EastnTricks.Content = $"({TricksCount[1].Count()})";
            Label_SouthTricks.Content = $"({TricksCount[2].Count()})";
            string text = $"({TricksCount[3].Count()})";
            //if (text == "(1)")
            //{
            //    MessageBox.Show("HEJKON");
            //}
            Label_PlayaTricks.Content = text;
        }

        public List<List<Card>> TricksCount { get; set; }
        private void HowManyTricks()
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





        private List<Card> idk(List<Card> cardList, List<Card> tricksCount, Card current)
        {
            Card NyttKortSomViVillHaMed = cardList.SingleOrDefault(c => c.Suit == current.Suit && c.Rank == current.Rank - 1);

            if (NyttKortSomViVillHaMed == null)
            {
                return tricksCount;
            }

            tricksCount.Add(NyttKortSomViVillHaMed);
            return idk(cardList, tricksCount, NyttKortSomViVillHaMed);
        }







        private List<Card> TaMedEnTiaSomStickOchKollaOmDetFinnsNiaISammaFärg(int i)
        {
            List<Card> tempList = new List<Card>(Players[i].Hand);
            List<Card> AllaMedSammaFärgSomDeÖverKnekt = Players[i].Hand.Where(v => TricksCount[i].Select(c => c.Suit).Contains(v.Suit)).ToList();
            AllaMedSammaFärgSomDeÖverKnekt.RemoveAll(x => x.Rank < Card.CardRank.Tio);
            tempList.OrderByDescending(r => r.Rank);

            foreach (var card in AllaMedSammaFärgSomDeÖverKnekt)
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
            var tiansFärg = AllaMedSammaFärgSomDeÖverKnekt.Where(c => c.Rank == Card.CardRank.Tio).Select(s => s.Suit);

            var nyaTrickscount = idk(Players[i].Hand.ToList(), TricksCount[i], Players[3].Hand[1]);

            return AllaMedSammaFärgSomDeÖverKnekt; // TODO - Lägga till om tian har en nia i samma färg lägg även till den då
        }

        // Om man har fyra kort i en färg och man har Ess och Kung i den färgen = 5 säkra stick
        private List<Card> FyraKortIEnFärgMedEssOchKung(List<Card> cardList, List<Card> tricksCount)
        {
            //IEnumerable<Card.CardSuit> q;
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

        private void BytUtKortenIPlayaListan(Player player)
        {
            player.Hand.Clear();
            player.Hand.Add(new Card(Card.CardSuit.Ruter, Card.CardRank.Ess));
            player.Hand.Add(new Card(Card.CardSuit.Spader, Card.CardRank.Dam));
            player.Hand.Add(new Card(Card.CardSuit.Spader, Card.CardRank.Knekt));
            player.Hand.Add(new Card(Card.CardSuit.Spader, Card.CardRank.Tio));
            player.Hand.Add(new Card(Card.CardSuit.Hjärter, Card.CardRank.Tio));
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


        bool FyraLikaYao = false;
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

        public Card CardToPlay_North { get; set; }
        public Card CardToPlay_Eastn { get; set; }
        public Card CardToPlay_South { get; set; }
        public Card CardToPlay_Playa { get; set; }
        public void PlayHighestCard()
        {
            CardToPlay_North = Players[0].Hand.OrderByDescending(v => v.Rank).First();
            string cardNumber = CardImageNumber(CardToPlay_North);
            var uri = new Uri($"C:\\Users\\William Boquist\\Plump\\Cards_WPF\\Graphics\\{cardNumber}.jpeg");
            Image_NorthPlayed.Source = new BitmapImage(uri);

            CardToPlay_Eastn = Players[1].Hand.OrderByDescending(v => v.Rank).First();
            cardNumber = CardImageNumber(CardToPlay_Eastn);
            uri = new Uri($"C:\\Users\\William Boquist\\Plump\\Cards_WPF\\Graphics\\{cardNumber}.jpeg");
            Image_EastnPlayed.Source = new BitmapImage(uri);

            CardToPlay_South = Players[2].Hand.OrderByDescending(v => v.Rank).First();
            cardNumber = CardImageNumber(CardToPlay_South);
            uri = new Uri($"C:\\Users\\William Boquist\\Plump\\Cards_WPF\\Graphics\\{cardNumber}.jpeg");
            Image_SouthPlayed.Source = new BitmapImage(uri);

            CardToPlay_Playa = Players[3].Hand.OrderByDescending(v => v.Rank).First();
            cardNumber = CardImageNumber(CardToPlay_Playa);
            uri = new Uri($"C:\\Users\\William Boquist\\Plump\\Cards_WPF\\Graphics\\{cardNumber}.jpeg");
            Image_PlayaPlayed.Source = new BitmapImage(uri);
        }

        public List<Card> DeckOfCards { get; set; }
        static Random r = new Random();
        public List<Player> Players { get; set; }

        public void InitializeDeck()
        {
            DeckOfCards = new List<Card>();
            for (Card.CardSuit i = Card.CardSuit.Hjärter; i <= Card.CardSuit.Klöver; i++)
            {
                for (Card.CardRank j = Card.CardRank.Två; j <= Card.CardRank.Ess; j++)
                {
                    DeckOfCards.Add(new Card(i, j));
                }
            }
        }

        static public List<Card> ShuffleDeck(List<Card> deck)
        {
            for (int n = deck.Count - 1; n > 0; --n)
            {
                int k = r.Next(n + 1);
                var temp = deck[n];
                deck[n] = deck[k];
                deck[k] = temp;
            }
            return deck;
        }

        public void CreatePlayers()
        {
            Players = new List<Player>();

            Player north = new Player("North");
            ListBox_North.ItemsSource = north.Hand;
            Players.Add(north);

            Player east = new Player("East");
            ListBox_East.ItemsSource = east.Hand;
            Players.Add(east);

            Player south = new Player("South");
            ListBox_South.ItemsSource = south.Hand;
            Players.Add(south);

            Player player1 = new Player("Player1");
            ListBox_Player1.ItemsSource = player1.Hand;
            Players.Add(player1);
        }

        public void DealCards()
        {
            for (int i = 0; i < 5; i++)
            {
                foreach (var player in Players)
                {
                    player.Hand.Add(DeckOfCards[0]);
                    DeckOfCards.RemoveAt(0);
                }
            }
            foreach (var player in Players)
            {
                var nyHand = player.Hand.OrderBy(c => c.Suit).ThenByDescending(c => c.Rank).ToList();
                player.Hand.Clear();
                foreach (var card in nyHand)
                {
                    player.Hand.Add(card);
                }
            }
        }

        public void ShowHands()
        {
            Label_NorthHand.Content = CardToPlay_North;
            Label_SouthHand.Content = CardToPlay_South;
            Label_EastnHand.Content = CardToPlay_Eastn;
            Label_PlayaHand.Content = CardToPlay_Playa;
        }

        private void HowManyRestarts()
        {
            int numberOfRestarts = 0;
            do
            {
                numberOfRestarts++;
                Label_Number.Content = numberOfRestarts.ToString();
                Start_New_Game();
                //MessageBox.Show("UNDVIK");
            } while (!FyraLikaYao);

            FyraLikaYao = false;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            HowManyRestarts();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Start_New_Game();
        }

        private string CardImageNumber(Card cardToNum)
        {
            int suit = (int)cardToNum.Suit;
            int rank = (int)cardToNum.Rank;
            string temp = rank.ToString();
            if (rank < 10)
            {
                temp = "0" + rank.ToString();
            }
            return suit.ToString() + temp;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            int numberOfRestarts = 100;
            do
            {
                numberOfRestarts -= 1;
                Start_New_Game();
            } while (numberOfRestarts != 0);
        }
    }
}
