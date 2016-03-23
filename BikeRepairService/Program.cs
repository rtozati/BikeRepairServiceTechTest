using BikeRepairService.ExternalLibraries;
using System;
using System.Collections.Generic;

namespace BikeRepairService
{
    class Program
    {
        static void Main(string[] args)
        {
            IList<Engineer> engineers = new List<Engineer>();
            engineers.Add(new Engineer("Engineer1", true, 51.510897, -0.223887, TravelMode.PublicTransport, "123456789"));
            TextMessageSender textMessageSender = new DummyTextMessageSender();
            TravelDurationApi travelDurationApi = new DummyTravelDurationApi();

            RepairService repairService = new ConsoleReportingRepairService(engineers, travelDurationApi, textMessageSender);

            repairService.RequestEngineer(51.510897, -0.223887, DateTime.Now, "Fred");

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }
    }
}
