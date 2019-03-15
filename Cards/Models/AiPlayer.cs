namespace Cards.Models
{
    public class AiPlayer : Player
    {
        public AiPlayer(string name)
            : base(name)
        {
        }

        public override void StartTurn()
        {
            var myTurnIndex = GameService.Players.IndexOf(this);


            GameService.PlayCard(this, Hand[0]);
        }
    }
}
