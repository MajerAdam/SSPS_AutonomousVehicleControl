namespace AutonomousVehicleControl
{
    public class Most : Silnice
    {
        public Most(string jmeno, double delka) : base(50, jmeno, delka)
        {
        }

        public override void Entered(Auto auto) // zmení se rychlost podle počasí
        {
            base.Entered(auto);

            auto.Rychlost = auto.RidiciSystem.GetMostRychlost(auto);
        }

    }
}
