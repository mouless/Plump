namespace Cards.Models
{
    public class HumanPlayer : Player
    {
        public HumanPlayer(string name) 
            : base(name)
        {
        }

        public override void StartTurn()
        {
            throw new System.NotImplementedException();
        }
    }
}
