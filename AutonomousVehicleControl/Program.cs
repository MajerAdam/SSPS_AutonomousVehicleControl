using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutonomousVehicleControl
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Silnice> trasa = new List<Silnice>()
            {
                new Silnice(200, "prvni silnice", 1),
                new Most("prvni most", .2),
                new Tunel("prvni tunel", .4)
            };
            RidiciSystem ridiciSystem = new RidiciSystem();
            Auto auto = ridiciSystem.CreateAuto(trasa, "auto");
            auto.OnPorucha += (sender, e) =>            Console.WriteLine($"{GetJmenoObecnehoAuta(sender)}: Chyba! Chybovy kód: {e.ChybovyKod}");
            auto.DruhSilniceChanged += (sender, e) =>   Console.WriteLine($"{GetJmenoObecnehoAuta(sender)}: Zmena typu silnice na: " + e.Silnice.GetType().Name);
            auto.NewSilniceEntered += (sender, e) =>    Console.WriteLine($"{GetJmenoObecnehoAuta(sender)}: Auto prejelo na novou silnici: {e.Silnice}");
            auto.OnTrasaDokoncena += (sender, e) =>     Console.WriteLine($"{GetJmenoObecnehoAuta(sender)}: Trasa dokoncena");
            auto.RychlostChanged += (sender, e) =>      Console.WriteLine($"{GetJmenoObecnehoAuta(sender)}: Zmena rychlosti na: {(sender as Auto).Rychlost}");
            auto.StavSvetelChanged += (sender, e) =>    Console.WriteLine($"{GetJmenoObecnehoAuta(sender)}: Zmena stavu svetel: {(sender as Auto).StavSvetel}");

            ridiciSystem.SpustTrasuVAute(auto);

            Console.ReadLine();
            string GetJmenoObecnehoAuta(object auto) => ((Auto)auto).Jmeno;
        }
    }

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

    public class Lokace
    {
        public double UjetaVzdalenost { get; set; }
        public Silnice AktualniSilnice { get; set; }
    }

    public class SilniceChangedEventArgs : EventArgs
    {
        public Silnice Silnice { get; set; }
        public Lokace VehicleLocation { get; set; }
    }

    public class PoruchaEventArgs : EventArgs
    {
        public ChybovyKod ChybovyKod { get; set; }
    }

    public enum ChybovyKod
    {
        HodneSpatny,
        MaloSpatny
    }

    public class Auto
    {
        public event EventHandler<SilniceChangedEventArgs> NewSilniceEntered;
        public event EventHandler<SilniceChangedEventArgs> DruhSilniceChanged;
        public event EventHandler<EventArgs> RychlostChanged;
        public event EventHandler<EventArgs> StavSvetelChanged;
        public event EventHandler<PoruchaEventArgs> OnPorucha;
        public event EventHandler<EventArgs> OnTrasaDokoncena;

        public string Jmeno { get; }
        public List<Silnice> Trasa = new List<Silnice>();
        public Silnice CurrSilnice { get; private set; }

        internal Auto(RidiciSystem ridiciSystem, List<Silnice> trasa, string jmeno)
        {
            RidiciSystem = ridiciSystem;
            Trasa = trasa;
            Jmeno = jmeno;
        }

        private double rychlost;
        public double Rychlost
        {
            get => rychlost;
            set { rychlost = value; RychlostChanged?.Invoke(this, EventArgs.Empty); }
        }

        public Lokace Poloha
        {
            get;
            set;
        }

        private bool stavSvetel;
        public bool StavSvetel
        {
            get => stavSvetel;
            set { stavSvetel = value; StavSvetelChanged?.Invoke(this, EventArgs.Empty); }
        }

        public RidiciSystem RidiciSystem
        {
            get;
            private set;
        }

        private ThreadSafeRandom rnd = new ThreadSafeRandom();
        public ChybovyKod VyvolatPoruchu() // vyvolá metodu z Ridiciho systemu podle typu poruchy
        {
            ChybovyKod chyba = (ChybovyKod)rnd.Next(2);
            RidiciSystem.VyresPoruchu(this, chyba);
            OnPorucha?.Invoke(this, new PoruchaEventArgs() { ChybovyKod = chyba });
            return chyba;
        }

        internal async Task ProvedTrasu()
        {
            Silnice lastSilnice = null;
            for (int i = 0; i < Trasa.Count; i++)
            {
                CurrSilnice = Trasa[i];

                NewSilniceEntered?.Invoke(this, new SilniceChangedEventArgs() { Silnice = CurrSilnice, VehicleLocation = Poloha });
                if (lastSilnice != null && !lastSilnice.GetType().Equals(CurrSilnice.GetType()))
                    DruhSilniceChanged?.Invoke(this, new SilniceChangedEventArgs() { Silnice = CurrSilnice, VehicleLocation = Poloha });
                lastSilnice = CurrSilnice;

                CurrSilnice.Entered(this);

                //TODO implementovat dynamicky vyvolavani chyb
                bool staneSeChyba = false;// rnd.Next(100) == 0;

                double cilovaVzdalenost = CurrSilnice.Delka;

                if (staneSeChyba)
                    cilovaVzdalenost *= rnd.NextDouble();

                double dobaJizdy = (cilovaVzdalenost / Rychlost) * 3600 * 1000 * RidiciSystem.TimeScale;

                await Task.Delay((int)dobaJizdy);

                if (staneSeChyba)
                    if (VyvolatPoruchu() == ChybovyKod.HodneSpatny)
                        break;

                CurrSilnice.Left(this);
            }

            OnTrasaDokoncena?.Invoke(this, EventArgs.Empty);
        }
    }

    public class RidiciSystem
    {
        public static double TimeScale = 1;

        public List<Task> AutoTasks = new List<Task>();

        public double GetMostRychlost(Auto auto) // Vytvoří rychlost podle pomocí počasí (zeptá se MeteroStredisko)
        {
            switch (MeteoStredisko.GetPocasi(auto.Poloha))
            {
                case Pocasi.Dobry:
                    return 70;
                case Pocasi.Stredni:
                    return 50;
                case Pocasi.Spatny:
                    return 30;
            }
            return 50;
        }

        public void VyresPoruchu(Auto auto, ChybovyKod chybovyKod)
        {
            switch (chybovyKod)
            {
                case ChybovyKod.HodneSpatny:
                    VyresTezkouPoruchu(auto);
                    break;
                case ChybovyKod.MaloSpatny:
                    VyresLehkouPoruchu(auto);
                    break;
            }
        }

        private void VyresTezkouPoruchu(Auto auto) //
        {
            ServisStredisko.ChciNahradniAuto(new List<Silnice>(), auto);
        }

        private void VyresLehkouPoruchu(Auto auto)
        {
            ServisStredisko.ChciNahradniAuto(ServisStredisko.GetTrasaKServisu(auto.Poloha), auto);

        }

        public Auto CreateAuto(List<Silnice> trasa, string jmeno)
        {
            return new Auto(this, trasa, jmeno);
        }

        public void SpustTrasuVAute(Auto auto)
        {
            AutoTasks.Add(auto.ProvedTrasu());
        }
    }

    public enum Pocasi
    {
        Dobry,
        Stredni,
        Spatny
    }

    public static class MeteoStredisko
    {
        private static ThreadSafeRandom rnd = new ThreadSafeRandom();
        public static Pocasi GetPocasi(Lokace lokace) // dostaneme pocasi
        {
            return (Pocasi)rnd.Next(3);
        }
    }

    public static class ServisStredisko
    {
        private static ThreadSafeRandom rnd = new ThreadSafeRandom();

        public static void ChciNahradniAuto(List<Silnice> trasaDoMistaSetkani, Auto auto)
        {
            bool predChybou = true;
            List<Silnice> zbyvajiciSilnice = new List<Silnice>();
            zbyvajiciSilnice.AddRange(trasaDoMistaSetkani.AsEnumerable().Reverse());

            foreach (var ii in auto.Trasa)
            {
                if (!predChybou)
                    zbyvajiciSilnice.Add(ii);
                if (ii == auto.CurrSilnice)
                    predChybou = false;
                    continue;
            }


            Auto nahrada = auto.RidiciSystem.CreateAuto(zbyvajiciSilnice, "nahrada za " + auto.Jmeno);
            auto.OnTrasaDokoncena += (sender, e) => nahrada.RidiciSystem.SpustTrasuVAute(nahrada);
        }

        public static List<Silnice> GetTrasaKServisu(Lokace poloha) // Dostane cestu od aouta k nejbližšímu servisu
        {
            return new List<Silnice>() { new Silnice(30, "Cesta do servisu", rnd.NextDouble() * 5 + 1) }; 
        }
    }

    public class ThreadSafeRandom
    {
        private static readonly Random _global = new Random();
        [ThreadStatic] private static Random _local;

        private void CheckLocal()
        {
            if (_local == null)
            {
                int seed;
                lock (_global)
                {
                    seed = _global.Next();
                }
                _local = new Random(seed);
            }
        }

        public int Next(int max)
        {
            CheckLocal();
            return _local.Next(max);
        }

        public double NextDouble()
        {
            CheckLocal();
            return _local.NextDouble();
        }
    }
}
