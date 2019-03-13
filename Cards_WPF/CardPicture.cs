using System.Windows.Media.Imaging;

namespace Cards_WPF
{
    public class CardPicture
    {
        public enum CardSuit { Hjärter = 1, Spader, Ruter, Klöver }
        public enum CardRank { Baksida = 1, Två, Tre, Fyra, Fem, Sexa, Sju, Åtta, Nio, Tio, Knekt, Dam, Knug, Ess, Joker }

        public CroppedBitmap Picture { get; set; }
        public CardSuit Suit { get; set; }
        public CardRank Rank { get; set; }
        public string Symbol { get; set; }

        public override string ToString()
        {
            return Suit.ToString() + " - " + Rank.ToString();
        }
    }
}
