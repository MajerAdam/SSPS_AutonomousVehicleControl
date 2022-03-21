using System;
using System.Collections.Generic;

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
            string GetJmenoObecnehoAuta(object _auto) => ((Auto)_auto).Jmeno;
        }
    }
}
