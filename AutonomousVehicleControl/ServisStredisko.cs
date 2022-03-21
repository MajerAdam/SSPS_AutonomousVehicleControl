using System;
using System.Collections.Generic;
using System.Linq;

namespace AutonomousVehicleControl
{
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
}
