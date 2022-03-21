using AutonomousVehicleControl;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Visualiser
{
    public partial class MainWindow : Window
    {
        private AutoFactoryDataContext dataContext;

        private Auto currAuto;
        private Silnice currSilnice;
        public List<Silnice> currTrasa = new List<Silnice>();

        private RidiciSystem RidiciSystem;
        public List<Auto> auta = new List<Auto>();

        public void Init()
        {
            RidiciSystem = new RidiciSystem();
            ClearAuto();
        }

        private void CreateNewSilnice()
        {
            currSilnice = new SilniceBinding(50, "Nepojmenovana silnice", 1);
        }

        public void CreateNewAuto()
        {
            currAuto = new AutoBinding(RidiciSystem, currTrasa, "Nepojmenovane auto");
        }

        public void AddSilniceToTrasa()
        {
            currTrasa.Add(currSilnice);
        }

        public void StartCurrAuto()
        {
            RidiciSystem.SpustTrasuVAute(currAuto);
            ClearAuto();
        }

        private void ClearAuto()
        {
            CreateNewAuto();
            CreateNewSilnice();
            currTrasa.Clear();
            dataContext = new AutoFactoryDataContext() { Auto = currAuto, CurrSilnice = currSilnice, Trasa = currTrasa };
            DataContext = dataContext;
        }
    }

    public class AutoFactoryDataContext : INotifyPropertyChanged
    {
        private Auto auto;
        private Silnice currSilnice;
        private List<Silnice> trasa;

        public List<Silnice> Trasa
        {
            get { return trasa; }
            set { trasa = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Trasa")); }
        }

        public Silnice CurrSilnice
        {
            get { return currSilnice; }
            set { currSilnice = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CurrSilnice")); }
        }

        public Auto Auto
        {
            get { return auto; }
            set { auto = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Auto")); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class AutoBinding : Auto, INotifyPropertyChanged
    {
        protected internal AutoBinding(RidiciSystem ridiciSystem, List<Silnice> trasa, string jmeno) : base(ridiciSystem, trasa, jmeno)
        {
            JmenoChanged += (sender, e) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Jmeno"));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class SilniceBinding : Silnice, INotifyPropertyChanged
    {
        public SilniceBinding(double maxRychlost, string jmeno, double delka) : base(maxRychlost, jmeno, delka)
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected override void SetDelka(double value)
        {
            base.SetDelka(value);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Delka"));
        }

        protected override void SetJmeno(string value)
        {
            base.SetJmeno(value);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Jmeno"));
        }

        protected override void SetMaxRychlost(double value)
        {
            base.SetMaxRychlost(value);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MaxRychlost"));
        }
    }
}
