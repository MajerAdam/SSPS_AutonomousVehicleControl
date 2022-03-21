using System;

namespace AutonomousVehicleControl
{
    public class SilniceChangedEventArgs : EventArgs
    {
        public Silnice Silnice { get; set; }
        public Lokace VehicleLocation { get; set; }
    }
}
