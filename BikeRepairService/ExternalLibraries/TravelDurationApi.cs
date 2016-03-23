using System;

namespace BikeRepairService.ExternalLibraries
{
    public interface TravelDurationApi
    {
        /// <summary>
        /// Estimates how long a journey will take. Takes into account factors such as estimated traffic congestion for the time of day. 
        /// </summary>
        /// <param name="startLatitude">The latitude of the start of the journey in degrees</param>
        /// <param name="startLongitude">The longitude of the start of the journey in degrees</param>
        /// <param name="endLatitude">The latitude of the end of the journey in degrees</param>
        /// <param name="endLongitude">The longitude of the end of the journey in degrees</param>
        /// <param name="startTime">What time the journey should start in degrees</param>
        /// <param name="travelMode">The form of transport</param>
        /// <returns>How long the journey is estimated to take. The estimated arrival time will be this duration after the startTime</returns>
        TimeSpan GetTravelDuration(double startLatitude, double startLongitude, double endLatitude, double endLongitude, DateTime startTime, TravelMode travelMode);
    }

    public enum TravelMode
    {
        Driving,
        Walking,
        Cycling,
        PublicTransport
    }


    internal class DummyTravelDurationApi : TravelDurationApi
    {
        public TimeSpan GetTravelDuration(double startLatitude, double startLongitude, double endLatitude, double endLongitude, DateTime startTime, TravelMode travelMode)
        {
            var random = new Random();
            return TimeSpan.FromMinutes(random.Next(5, 120));
        }
    }
}