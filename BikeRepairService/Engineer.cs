using BikeRepairService.ExternalLibraries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeRepairService
{
    public class Engineer
    {
        public virtual string Name { get; private set; }
        public virtual bool Available { get; set; }
        public virtual double Latitude { get; private set; }
        public virtual double Longitude { get; private set; }
        public virtual string PhoneNumber { get; private set; }
        public virtual TravelMode TravelMode { get; private set; }

        public Engineer(string name, bool available, double latitude, double longitude, TravelMode travelMode, string phoneNumber)
        {
            Name = name;
            Available = available;
            Latitude = latitude;
            Longitude = longitude;
            TravelMode = travelMode;
            PhoneNumber = phoneNumber;
           
        }        

        public virtual bool IsAvailable(TimeSpan distance, double destinationLatitude, double destinationLongitude)
        {
            if (Available && distance <= DistanceLimit() && ValidRange(destinationLatitude, destinationLongitude))
                return true;
            else
                return false;

        }

        private TimeSpan DistanceLimit()
        {
            if (TravelMode == TravelMode.Cycling)
                return new TimeSpan(0, 40, 0);
            else
                return TimeSpan.MaxValue;
        }

        private bool ValidRange(double destinationLatitude, double destinationLongitude)
        {
            //Range where destination should be. 51.45 < latitude < 51.60 and - 0.30 < longitude < 0.05
            double minLatitude = 51.45;
            double maxLatitude = 51.60;
            double minLongitude = -0.30;
            double maxLongitude = 0.05;
            if (TravelMode == TravelMode.Driving)
            {
                if (minLatitude < destinationLatitude &&
                    destinationLatitude < maxLatitude
                   && minLongitude < destinationLongitude
                   && destinationLongitude < maxLongitude)
                    return true;
                else
                    return false;
            }
            else
                return true;
        }
    }
}
