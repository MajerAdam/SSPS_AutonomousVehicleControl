using System;

namespace AutonomousVehicleControl
{
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
