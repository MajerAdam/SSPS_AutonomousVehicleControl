namespace AutonomousVehicleControl
{
    public static class MeteoStredisko
    {
        private static ThreadSafeRandom rnd = new ThreadSafeRandom();
        public static Pocasi GetPocasi(Lokace lokace) // dostaneme pocasi
        {
            return (Pocasi)rnd.Next(3);
        }
    }
}
