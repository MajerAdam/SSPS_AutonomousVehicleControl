using System.Collections.Generic;
using System.Threading.Tasks;

namespace AutonomousVehicleControl
{
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
}
