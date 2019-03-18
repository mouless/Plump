namespace Cards.Models
{
    public class AiPlayer : Player
    {
        public AiPlayer(string name)
            : base(name)
        {
        }

        public override void BestämmaStick()
        {
            throw new System.NotImplementedException();
        }
    }
}
