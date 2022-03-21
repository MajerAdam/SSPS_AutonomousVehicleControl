namespace AutonomousVehicleControl
{
    public class Silnice
    {
        public Silnice(double maxRychlost, string jmeno, double delka)
        {
            MaxRychlost = maxRychlost;
            Jmeno = jmeno;
            Delka = delka;
        }

        private string jmeno;
        public string Jmeno
        {
            get => jmeno; //jmeno silnice
            set => jmeno = value;
        }

        private double maxRychlost;  // maximální možná rychlost na sinici
        public double MaxRychlost
        {
            get => maxRychlost;
            set => maxRychlost = value;
        }

        private double delka; // jak dlouhá je silnice
        public double Delka
        {
            get => delka;
            set => delka = value;
        }

        /// <summary>
        /// Metoda, ktera se zavola kdyz auto sjede z teto silnice
        /// </summary>
        /// <param name="auto">Auto, ktere sjelo ze silnice</param>
        public virtual void Left(Auto auto) // dostane novou maxrychlost když opustí část silnice
        {

        }

        /// <summary>
        /// Metoda, ktera se zavola kdyz auto vjede na tuto silnici
        /// </summary>
        /// <param name="auto">Auto, ktere najelo na silnici</param>
        public virtual void Entered(Auto auto) // změna při opuštění oblasti
        {
            auto.Rychlost = MaxRychlost;
        }

        public override string ToString()
        {
            return $"{Jmeno} {{Delka: {Delka}; MaxRychlost: {MaxRychlost}}}";
        }
    }
}
