using System;

namespace AutonomousVehicleControl
{
    class Program
    {
        static void Main(string[] args)
        {
            
        }
    }

    class Silnice
    {
        string jmeno
        string Jmeno
        {
            get => return jmeno; //jmeno silnice
            set => jmeno = value;
        }
        double maxrychlost;  // maximální možná rychlost na sinici
        double Maxrychos 
        {
            get => return maxrychlost;
            set => maxrychost = value;
        }
        double delka;   // jak dlouhá je silnice
        double Delka
        {
            get => return delka;
            set => Delka = value;
        }
        public double Left(double d) // dostane novou maxrychlost když opustí část silnice
        {
            double rychlost
            return rychlost;
        }
        public double Entered(double d) // změna při opuštění oblasti
        {
            double rychlost
            return rychlost;
        }
           
    }
    class Most : Silnice
    {
        public override double Entered(double d) // zmení se rychlost podle počasí
        {
            double rychlost
            return rychlost;
        }

    }
    class Tunel: Silnice
    {
        public override double Left(double d) // změní se stav světel a rychlost 
        {
            double rychlost
            return rychlost;
        }
        public override double Entered(double d) // změní se stav světel a rychlost
        {
            double rychlost
            return rychlost;
        }
        

    }
    class BeznaCesta : Silnice
    {

    }
    class Auto
    {
        double rychlost;
        double Rychlost
        {
            get => return rychlost;
            set => rychlost = value;
        }
        double trasa;
        double Trasa
        {
            get => return trasa;
            set => trasa = value;
        }
        double poloha;
        double Poloha
        {
            get => return poloha;
            set => poloha = value;
        }
        bool stav_svetel;
        bool Stav_svetel
        {
            get => return stav_svetel;
            set => stav_svetel = value;
        }
        
        public void porucha() // vyvolá metodu z Ridiciho systemu podle typu poruchy
        {

        }

    }
    class RidiciSystem
    {
        public double GetMostRychlost() // Vytvoří rychlost podle pomocí počasí (zeptá se MeteroStredisko)
        {
            double rychlost
            return rychlost;
        }
        public void HorsiPorucha() //
        {

        }
        public void LepsiPorucha()
        {

        }
    }
    class MeteroStredisko
    {
        public string GetPocasi() // dostaneme pocasi
        {

        }
    }
    class ServisStedisko
    {
        public void OdvezAuto() // Vyžáda nový auto pote dá mu trasu k porouchanému vozidlu a pak ho odveze
        {

        }
        public void NovyAuto() // vytvoří auto v Servisu
        {

        }
        public double GetTrasuKServisu(double poloha) // Dostane cestu od aouta k nejbližšímu servisu
        {

        }
    }
}
