namespace Cards.Models
{
    public class HumanPlayer : Player
    {
        public HumanPlayer(string name) 
            : base(name)
        {
        }

        public override void BestämmaStick()
        {
            throw new System.NotImplementedException();
        }
    }
}
