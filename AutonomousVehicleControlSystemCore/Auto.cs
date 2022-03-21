using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AutonomousVehicleControl
{
    public class Auto
    {
        public event EventHandler<SilniceChangedEventArgs> NewSilniceEntered;
        public event EventHandler<SilniceChangedEventArgs> DruhSilniceChanged;
        public event EventHandler<EventArgs> RychlostChanged;
        public event EventHandler<EventArgs> StavSvetelChanged;
        public event EventHandler<PoruchaEventArgs> OnPorucha;
        public event EventHandler<EventArgs> OnTrasaDokoncena;
        public event EventHandler<EventArgs> PolohaChanged;
        public event EventHandler<EventArgs> JmenoChanged;

        private string jmeno;
        public string Jmeno
        {
            get => jmeno;
            set { jmeno = value; JmenoChanged?.Invoke(this, EventArgs.Empty); }
        }
        public List<Silnice> Trasa = new List<Silnice>();
        public Silnice CurrSilnice { get; private set; }

        internal protected Auto(RidiciSystem ridiciSystem, List<Silnice> trasa, string jmeno)
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

        private Lokace poloha;
        public Lokace Poloha
        {
            get => poloha;
            set { poloha = value; PolohaChanged?.Invoke(this, EventArgs.Empty); }
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
}
