using BikeRepairService.ExternalLibraries;
using System;
using System.Collections.Generic;

namespace BikeRepairService
{
    public class ConsoleReportingRepairService : RepairService
    {
        private IList<Engineer> _engineers = new List<Engineer>();
        private TravelDurationApi _travelDurationApi;
        private TextMessageSender _textMessageSender;

        public ConsoleReportingRepairService(IList<Engineer> engineers, TravelDurationApi travelDurationApi, TextMessageSender textMessageSender)
        {
            _engineers = engineers;
            _travelDurationApi = travelDurationApi;
            _textMessageSender = textMessageSender;

        }

        public virtual void RequestEngineer(double latitudeRequired, double longitudeRequired, DateTime date, string customerName)
        {
            Engineer engineer = ObtainAvailableEngineer(latitudeRequired,longitudeRequired, date);            

            if (engineer == null)
            {
                NoEngineerAvailable();                
            }
            else
            {
                AllocateEngineer(engineer, latitudeRequired, longitudeRequired, date, customerName);                
            }

            LogToConsole(string.Format("Engineer requested at {0}°N {1}°W. Customer name: {2}", latitudeRequired, longitudeRequired, customerName));
        }

        public virtual TimeSpan ObtainDistanceFromCustomer(double latitudeRequired, double longitudeRequired, DateTime date, Engineer engineer)
        {
            return _travelDurationApi.GetTravelDuration(engineer.Latitude, engineer.Longitude, latitudeRequired, longitudeRequired, date, engineer.TravelMode);
        }

        public virtual void SendMessageWithCustomerDetailsToEngineer(double latitudeRequired, double longitudeRequired, string customerName, Engineer engineer)
        {
            _textMessageSender.Send(engineer.PhoneNumber, string.Format("Engineer requested at {0}°N {1}°W. Customer name: {2}", latitudeRequired, longitudeRequired, customerName));
        }

        protected void LogToConsole(string message)
        {
            Console.WriteLine(message);
        }

        private Engineer ObtainAvailableEngineer(double latitudeRequired, double longitudeRequired, DateTime date)
        {
            Engineer engineer = null;
            foreach (Engineer engineerAux in _engineers)
            {
                TimeSpan engineerAuxDistance = ObtainDistanceFromCustomer(latitudeRequired, longitudeRequired, date, engineerAux);
                bool engineerAuxAvailable = engineerAux.IsAvailable(engineerAuxDistance, latitudeRequired, longitudeRequired);
                if (engineerAuxAvailable
                    && (engineer == null || engineerAuxDistance < ObtainDistanceFromCustomer(latitudeRequired, longitudeRequired, date, engineer)))
                {
                    engineer = engineerAux;
                }
            }
            return engineer;
        }

        private void AllocateEngineer(Engineer engineer, double latitudeRequired, double longitudeRequired, DateTime date, string customerName)
        {
            engineer.Available = false;
            SendMessageWithCustomerDetailsToEngineer(latitudeRequired, longitudeRequired, customerName, engineer);
        }

        private void NoEngineerAvailable()
        {
            LogToConsole("No engineer available");
            throw new ApplicationException("No Engineer Available");
        }
    }
}