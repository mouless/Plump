using Cards;
using Cards.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static Cards.Models.Card;

namespace Cards_ML
{
    public class Program
    {
        static void Main(string[] args)
        {
            var gameService = new GameService();

            var mlPlayers = new List<MLPlayer>();

            for (var i = 0; i < 100_000; i++)
            {
                gameService.CreateRound(5);

                var players = gameService.Players.Select((player, j) =>
                {
                    var playerTricksCount = gameService.TricksCount[j];

                    var mlPlayer = new MLPlayer(player, playerTricksCount);

                    return mlPlayer;
                }).ToList();

                mlPlayers.AddRange(players);
            }

            var json = JsonConvert.SerializeObject(mlPlayers);

            File.WriteAllText(@"C:\Plump\PlumpResultat.txt", json);

        }
    }

    public class MLPlayer
    {
        public string Name { get; }

        public ICollection<MLCard> Cards { get; }

        public int TricksCount { get; set; }

        public MLPlayer(Player player, List<Card> tricksCount)
        {
            Name = player.Name;
            Cards = player.Hand.Select(card => new MLCard(card)).ToList();
            TricksCount = tricksCount.Count;
        }
    }

    public class MLCard
    {
        public string Suit { get; }

        public string Rank { get; }

        public MLCard(Card card)
        {
            Suit = Enum.GetName(typeof(CardSuit), card.Suit);
            Rank = Enum.GetName(typeof(CardRank), card.Rank);
        }
    }
}
