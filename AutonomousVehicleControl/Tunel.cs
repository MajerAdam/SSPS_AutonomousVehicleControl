namespace AutonomousVehicleControl
{
    public class Tunel : Silnice
    {
        public Tunel(string jmeno, double delka) : base(70, jmeno, delka)
        {
        }

        public override void Left(Auto auto) // změní se stav světel a rychlost
        {
            base.Left(auto);
            auto.StavSvetel = false;
        }

        public override void Entered(Auto auto) // změní se stav světel a rychlost
        {
            base.Entered(auto);
            auto.StavSvetel = true;
        }


    }
}
