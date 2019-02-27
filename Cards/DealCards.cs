using System.Linq;

namespace Cards
{
    public static class DealCards
    {
        public static void DistributeCards()
        {
            for (int i = 0; i < 5; i++)
            {
                foreach (var player in GameService.Players)
                {
                    player.Hand.Add(GameService.DeckOfCards[0]);
                    GameService.DeckOfCards.RemoveAt(0);
                }
            }

            foreach (var player in GameService.Players)
            {
                var newHand = player.Hand.OrderBy(c => c.Suit).ThenByDescending(c => c.Rank).ToList();
                player.Hand.Clear();

                foreach (var card in newHand)
                {
                    player.Hand.Add(card);
                }
            }
        }
    }
}
