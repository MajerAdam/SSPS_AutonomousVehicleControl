using System;

namespace AutonomousVehicleControl
{
    class Program
    {
        static void Main(string[] args)
        {
            
        }
    }

    public class Silnice
    {
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

        private double delka;   // jak dlouhá je silnice
        public double Delka
        {
            get => delka;
            set => Delka = value;
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
            // TODO zmenit rychlost
            auto.Rychlost = MaxRychlost;
        }
           
    }

    public class Most : Silnice
    {
        public override void Entered(Auto auto) // zmení se rychlost podle počasí
        {
            base.Entered(auto);
            throw new NotImplementedException();
        }

    }

    public class Tunel : Silnice
    {
        public override void Left(Auto auto) // změní se stav světel a rychlost 
        {
            base.Left(auto);
            throw new NotImplementedException();
        }

        public override void Entered(Auto auto) // změní se stav světel a rychlost
        {
            base.Entered(auto);
            throw new NotImplementedException();
        }
        

    }

    public class BeznaCesta : Silnice
    {

    }

    public class Lokace
    {
        //TODO kostra lokace
    }

    public class RoadChangedEventArgs : EventArgs
    {
        public Silnice Silnice { get; set; }
        public Lokace VehicleLocation { get; set; }
    }

    public class Auto
    {
        public event EventHandler<RoadChangedEventArgs> NewRoadEntered;

        private double rychlost;
        public double Rychlost
        {
            get => rychlost;
            set => rychlost = value;
        }

        private double trasa;
        public double Trasa
        {
            get => trasa;
            set => trasa = value;
        }

        private Lokace poloha;
        public Lokace Poloha
        {
            get => poloha;
            set => poloha = value;
        }

        private bool stavSvetel;
        public bool StavSvetel
        {
            get => stavSvetel;
            set => stavSvetel = value;
        }

        public void porucha() // vyvolá metodu z Ridiciho systemu podle typu poruchy
        {
            throw new NotImplementedException();
        }
    }

    public class RidiciSystem
    {
        public double GetMostRychlost() // Vytvoří rychlost podle pomocí počasí (zeptá se MeteroStredisko)
        {
            throw new NotImplementedException();
        }
        public void HorsiPorucha() //
        {
            throw new NotImplementedException();
        }
        public void LepsiPorucha()
        {
            throw new NotImplementedException();
        }
    }

    public class MeteroStredisko
    {
        public string GetPocasi() // dostaneme pocasi
        {
            throw new NotImplementedException();
        }
    }

    public class ServisStedisko
    {
        public void OdvezAuto() // Vyžáda nový auto pote dá mu trasu k porouchanému vozidlu a pak ho odveze
        {
            throw new NotImplementedException();
        }
        public void NovyAuto() // vytvoří auto v Servisu
        {
            throw new NotImplementedException();
        }
        public double GetTrasuKServisu(double poloha) // Dostane cestu od aouta k nejbližšímu servisu
        {
            throw new NotImplementedException();
        }
    }
}
