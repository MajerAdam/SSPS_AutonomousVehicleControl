using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AutonomousVehicleControlSystemCore
{
    public static class WeatherService
    {
        public static double GetRecommendedMaxBridgeSpeed(Location vehiocleLocation)
        {
            throw new NotImplementedException();
        }
    }

    public struct Location
    {
        public Location(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public double DistanceTo(Location loc)
        {
            double dLat = loc.Latitude - Latitude;
            double dLon = loc.Longitude - Longitude;
            return Math.Sqrt(dLat * dLat + dLon * dLon);
        }

        public static Location operator +(Location a, Location b)
            => new Location(a.Latitude + b.Latitude, a.Longitude + b.Longitude);

        public static Location operator -(Location a)
            => new Location(-a.Latitude, -a.Longitude);

        public static Location operator -(Location a, Location b)
            => a + (-b);

        internal Location Multiply(Location a, double x)
            => new Location(a.Latitude * x, a.Longitude * x);
    }

    public interface IRoad
    {
        /// <summary>
        /// Unique name of the road
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Speed limit of the road (in length's units per hour)
        /// </summary>
        public double SpeedLimit { get; }

        /// <summary>
        /// Shape of the road
        /// </summary>
        public IList<Location> Shape { get; }

        public void VehicleEntered(Vehicle vehicle);

        public void VehicleLeft(Vehicle vehicle);
    }

    public static class RoadExtensions
    {
        public static double CalculateDistance(this IRoad road, int shapeIdxStart, int shapeIdxEnd = -1)
        {
            if (shapeIdxEnd < 0)
                shapeIdxEnd = road.Shape.Count - 1;

            double dist = 0;
            for (int i = shapeIdxStart; i < shapeIdxEnd; i++)
                dist += road.Shape[i].DistanceTo(road.Shape[i + 1]);

            return dist;
        }

        public static double CalculateHeadingAt(this IRoad road, int shapeIdx)
        {
            if (shapeIdx >= road.Shape.Count - 1)
                throw new IndexOutOfRangeException();

            Location startLoc = road.Shape[shapeIdx];
            Location endLoc = road.Shape[shapeIdx + 1];
            Location delta = startLoc - endLoc;
            return Math.Atan2(delta.Longitude, delta.Latitude);
        }

        public static Location CalculateLocationFromDistance(this IRoad road, double drivenDist)
        {
            int i = 0;
            while (road.CalculateDistance(0, i + 1) < drivenDist)
            {
                i++;
                if (i >= road.Shape.Count)
                    throw new ArgumentException("Too large driven distance!");
            }

            double fragmentDrivenDistance = drivenDist;
            if (i > 0)
                fragmentDrivenDistance -= road.CalculateDistance(0, i);

            double heading = road.CalculateHeadingAt(i);
            return road.Shape[i] + new Location(Math.Cos(heading) * fragmentDrivenDistance, Math.Sin(heading) * fragmentDrivenDistance);
        }
    }

    public class Road : IRoad
    {
        public Road(string name, double speedLimit, IList<Location> shape)
        {
            Name = name;
            SpeedLimit = speedLimit;
            Shape = shape;
        }

        public string Name { get; }

        public double SpeedLimit { get; }

        public IList<Location> Shape { get; }

        public virtual void VehicleEntered(Vehicle vehicle)
        {
            vehicle.BeginChangeSpeed(SpeedLimit);
        }

        public virtual void VehicleLeft(Vehicle vehicle)
        {
        }
    }

    public class Tunnel : Road
    {
        public Tunnel(string name, double speedLimit, IList<Location> shape) : base(name, speedLimit, shape)
        {
        }

        public override void VehicleEntered(Vehicle vehicle)
        {
            base.VehicleEntered(vehicle);
            vehicle.ChangeHeadlightsState(true);
        }

        public override void VehicleLeft(Vehicle vehicle)
        {
            base.VehicleLeft(vehicle);
            vehicle.ChangeHeadlightsState(false);
        }
    }

    public class Bridge : Road
    {
        public Bridge(string name, double speedLimit, IList<Location> shape) : base(name, speedLimit, shape)
        {
        }

        public override void VehicleEntered(Vehicle vehicle)
        {
            base.VehicleEntered(vehicle);
            // Gets recommended bridge speed form circa middle of the bridge and slows down the vehicle
            vehicle.BeginChangeSpeed(WeatherService.GetRecommendedMaxBridgeSpeed(Shape[Shape.Count / 2]));
        }
    }

    public class RoadRoute
    {
        public IRoad Road { get; set; }
        public double StartDeltaEnter { get; set; }
        public double StartDeltaExit { get; set; }
    }

    public class TravelPath : LinkedList<RoadRoute>
    {
    }

    public class RoadChangedEventArgs : EventArgs
    {
        public RoadRoute Route { get; set; }
        public Location VehicleLocation { get; set; }
    }

    public class Vehicle
    {
        public event EventHandler<RoadChangedEventArgs> RoadTypeChanged;
        public event EventHandler<RoadChangedEventArgs> NewRoadEntered;

        private TravelPath currJourney;
        public TravelPath RemainingJourney { get => currJourney; }
        private CancellationTokenSource journeyCancelTokenSrc;

        public double TimeMultiplier { get; set; }

        private double speed;
        public double Speed { get => speed; }

        public double MaximumSpeed { get; set; }

        private bool headlightsState = false;

        private double roadDrivenDist;

        public Vehicle(double maximumSpeed)
        {
            MaximumSpeed = maximumSpeed;
            journeyCancelTokenSrc = new CancellationTokenSource();
        }

        public bool AreLightsOn { get => headlightsState; }

        public void BeginChangeSpeed(double targetSpeed)
        {
            //TODO implement accel decel
            if (targetSpeed < MaximumSpeed)
                speed = targetSpeed;
            else
                speed = MaximumSpeed;
        }

        public void ChangeHeadlightsState(bool lightsOn)
        {
            headlightsState = lightsOn;
        }

        private void ResetState()
        {
            headlightsState = false;
            speed = 0;
        }

        public Task BeginNewJourney(TravelPath journey)
        {
            // Cancle current journey
            journeyCancelTokenSrc.Cancel();

            // Prepare variables for new journey
            journeyCancelTokenSrc = new CancellationTokenSource();
            currJourney = journey;
            
            return Task.Factory.StartNew(() => Travel(journeyCancelTokenSrc.Token));
        }

        private void Travel(CancellationToken cancellationToken)
        {
            roadDrivenDist = 0;
            Type lastRoadType = null;
            while(!cancellationToken.IsCancellationRequested)
            {
                RoadRoute currRoute = currJourney.First.Value;

                // TODO args
                if (NewRoadEntered != null)
                    NewRoadEntered.Invoke(this, new RoadChangedEventArgs() { Route = currRoute, VehicleLocation = currRoute.Road.CalculateLocationFromDistance(roadDrivenDist) });
                if (lastRoadType.Equals(currRoute.Road.GetType()) && RoadTypeChanged != null)
                    RoadTypeChanged.Invoke(this, new RoadChangedEventArgs() { Route = currRoute, VehicleLocation = currRoute.Road.CalculateLocationFromDistance(roadDrivenDist) });
                lastRoadType = currRoute.Road.GetType();

                double routeLength = Math.Abs(currRoute.StartDeltaExit - currRoute.StartDeltaEnter);

                currRoute.Road.VehicleEntered(this);

                DateTime lastLocUpdate = DateTime.Now;
                while(!cancellationToken.IsCancellationRequested)
                {
                    DateTime now = DateTime.Now;
                    double deltaTime = now.Subtract(lastLocUpdate).TotalSeconds;
                    lastLocUpdate = now;

                    roadDrivenDist += (Speed * deltaTime) / 3600D;

                    if(roadDrivenDist >= routeLength)
                    {
                        roadDrivenDist -= routeLength;
                        currRoute.Road.VehicleLeft(this);
                        currJourney.RemoveFirst();
                        break;
                    }
                }
            }
        }
    }
}
