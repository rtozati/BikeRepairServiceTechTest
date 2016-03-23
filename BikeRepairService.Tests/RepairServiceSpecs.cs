using BikeRepairService.ExternalLibraries;
using FluentAssertions;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace BikeRepairService.Tests
{
    public class RepairServiceSpecs
    {
        [Fact]
        public void When_an_engineer_is_requested_an_engineer_is_chosen_and_marked_as_unavailable()
        {
            IList<Engineer> engineers = new List<Engineer>();
            Engineer engineer = new Engineer("Engineer1", true, 51.510897, -0.223887, TravelMode.Cycling, "123456789");
            engineers.Add(engineer);
            Engineer engineer2 = new Engineer("Engineer2", true, 51.510897, -0.223887, TravelMode.Cycling, "123456789");
            engineers.Add(engineer2);
            Engineer engineer3 = new Engineer("Engineer2", false, 51.510897, -0.223887, TravelMode.Cycling, "123456789");
            engineers.Add(engineer3);

            TravelDurationApi travelDurationApi = MockRepository.GenerateStub<TravelDurationApi>();
            TextMessageSender textMessengerSender = MockRepository.GenerateStub<TextMessageSender>();

            RepairService repairService = new ConsoleReportingRepairService(engineers, travelDurationApi, textMessengerSender);

            repairService.RequestEngineer(51.510897, -0.223887, DateTime.Now, "Fred");

            engineer.Available.Should().Be(false);
            engineer2.Available.Should().Be(true);
            engineer3.Available.Should().Be(false);
            
        }

        [Fact]
        public void When_an_engineer_is_requested_and_no_engineer_is_available_should_throw_exception()
        {
            IList<Engineer> engineers = new List<Engineer>();

            TravelDurationApi travelDurationApi = MockRepository.GenerateStub<TravelDurationApi>();
            TextMessageSender textMessengerSender = MockRepository.GenerateStub<TextMessageSender>();

            RepairService repairService = new ConsoleReportingRepairService(engineers, travelDurationApi, textMessengerSender);

            Action action = () => repairService.RequestEngineer(51.510897, -0.223887, DateTime.Now, "Fred");

            action.ShouldThrow<ApplicationException>();
        }

        [Fact]
        public void When_an_engineer_is_requested_and_no_engineer_is_available_should_throw_exception_two()
        {
            IList<Engineer> engineers = new List<Engineer>();

            engineers.Add(new Engineer("Engineer1", false, 51.510897, -0.223887, TravelMode.Cycling, "123456789"));
            engineers.Add(new Engineer("Engineer2", false, 51.510897, -0.223887, TravelMode.Cycling, "123456789"));

            TravelDurationApi travelDurationApi = MockRepository.GenerateStub<TravelDurationApi>();
            TextMessageSender textMessengerSender = MockRepository.GenerateStub<TextMessageSender>();

            RepairService repairService = new ConsoleReportingRepairService(engineers, travelDurationApi, textMessengerSender);


            Action action = () => repairService.RequestEngineer(51.510897, -0.223887, DateTime.Now, "Fred");

            action.ShouldThrow<ApplicationException>();
        }

        [Fact]
        public void Retrieve_engineer_available_in_the_shortest_distance()
        {
            DateTime date = DateTime.Now;

            IList<Engineer> engineers = new List<Engineer>();
            
            Engineer engineer = new Engineer("Engineer1", true, 10.510897, -10.223887, TravelMode.Driving, "123456789");
            Engineer engineer2 = new Engineer("Engineer2", true, 20.510897, -20.223887, TravelMode.Driving, "123456789");
            Engineer engineer3 = new Engineer("Engineer3", true, 30.510897, -30.223887, TravelMode.Driving, "123456789");

            engineers.Add(engineer);
            engineers.Add(engineer2);
            engineers.Add(engineer3);

            TravelDurationApi travelDurationApi = MockRepository.GenerateStub<TravelDurationApi>();
            travelDurationApi.Stub(m => m.GetTravelDuration(engineer.Latitude, engineer.Longitude, 51.510897, -0.223887, date, engineer.TravelMode)).Return(new TimeSpan(2, 0, 0));
            travelDurationApi.Stub(m => m.GetTravelDuration(engineer2.Latitude, engineer2.Longitude, 51.510897, -0.223887, date, engineer2.TravelMode)).Return(new TimeSpan(1, 0, 0));
            travelDurationApi.Stub(m => m.GetTravelDuration(engineer3.Latitude, engineer3.Longitude, 51.510897, -0.223887, date, engineer3.TravelMode)).Return(new TimeSpan(3, 0, 0));

            TextMessageSender textMessengerSender = MockRepository.GenerateStub<TextMessageSender>();


            RepairService repairService = new ConsoleReportingRepairService(engineers, travelDurationApi, textMessengerSender);

            repairService.RequestEngineer(51.510897, -0.223887, date, "Fred");

            engineer.Available.Should().Be(true);
            engineer2.Available.Should().Be(false);
            engineer3.Available.Should().Be(true);
        }

        [Fact]
        public void Make_sure_a_message_is_sent_to_engineer_with_customer_details()
        {
            IList<Engineer> engineers = new List<Engineer>();
            Engineer engineer = new Engineer("Engineer1", true, 51.510897, -0.223887, TravelMode.Cycling, "123456789");
            engineers.Add(engineer);

            TravelDurationApi travelDurationApi = MockRepository.GenerateStub<TravelDurationApi>();
            TextMessageSender textMessengerSender = MockRepository.GenerateMock<TextMessageSender>();

            RepairService repairService = new ConsoleReportingRepairService(engineers, travelDurationApi, textMessengerSender);
            
            repairService.RequestEngineer(51.510897, -0.223887, DateTime.Now, "Fred");

            textMessengerSender.AssertWasCalled(x => x.Send("123456789", string.Format("Engineer requested at {0}°N {1}°W. Customer name: {2}", 51.510897, -0.223887, "Fred")));            
        }

        [Fact]
        public void Should_throw_excpetion_as_no_engineer_is_available_because_distance_is_more_than_40_minutes_for_engineer_with_bicycle()
        {
            DateTime date = DateTime.Now;

            IList<Engineer> engineers = new List<Engineer>();

            Engineer engineer = new Engineer("Engineer1", true, 10.510897, -10.223887, TravelMode.Cycling, "123456789");
            
            engineers.Add(engineer);            

            TravelDurationApi travelDurationApi = MockRepository.GenerateStub<TravelDurationApi>();
            travelDurationApi.Stub(m => m.GetTravelDuration(engineer.Latitude, engineer.Longitude, 51.510897, -0.223887, date, engineer.TravelMode)).Return(new TimeSpan(1, 0, 0));
            
            TextMessageSender textMessengerSender = MockRepository.GenerateStub<TextMessageSender>();

            RepairService repairService = new ConsoleReportingRepairService(engineers, travelDurationApi, textMessengerSender);

            Action action = () => repairService.RequestEngineer(51.510897, -0.223887, date, "Fred");

            action.ShouldThrow<ApplicationException>();
            
        }

        [Fact]
        public void Request_engineer_with_bicycle_available_if_distance_less_than_40_minutes()
        {
            DateTime date = DateTime.Now;

            IList<Engineer> engineers = new List<Engineer>();

            Engineer engineer = new Engineer("Engineer1", true, 10.510897, -10.223887, TravelMode.Cycling, "123456789");

            engineers.Add(engineer);

            TravelDurationApi travelDurationApi = MockRepository.GenerateStub<TravelDurationApi>();
            travelDurationApi.Stub(m => m.GetTravelDuration(engineer.Latitude, engineer.Longitude, 51.510897, -0.223887, date, engineer.TravelMode)).Return(new TimeSpan(0, 40, 0));

            TextMessageSender textMessengerSender = MockRepository.GenerateStub<TextMessageSender>();

            RepairService repairService = new ConsoleReportingRepairService(engineers, travelDurationApi, textMessengerSender);

            repairService.RequestEngineer(51.510897, -0.223887, date, "Fred");

            engineer.Available.Should().Be(false);

        }

        [Fact]
        public void Driving_engineer_should_be_available_only_between_defined_localization()
        {
            //Range where destination should be. 51.45 < latitude < 51.60 and - 0.30 < longitude < 0.05
            
            DateTime date = DateTime.Now;

            IList<Engineer> engineers = new List<Engineer>();

            Engineer engineer = new Engineer("Engineer1", true, 51.50, 0, TravelMode.Driving, "123456789");

            engineers.Add(engineer);

            TravelDurationApi travelDurationApi = MockRepository.GenerateStub<TravelDurationApi>();
            travelDurationApi.Stub(m => m.GetTravelDuration(engineer.Latitude, engineer.Longitude, 51.510897, -0.223887, date, engineer.TravelMode)).Return(new TimeSpan(0, 40, 0));

            TextMessageSender textMessengerSender = MockRepository.GenerateStub<TextMessageSender>();

            RepairService repairService = new ConsoleReportingRepairService(engineers, travelDurationApi, textMessengerSender);

            repairService.RequestEngineer(51.510897, -0.223887, date, "Fred");

            engineer.Available.Should().Be(false);
        }

        [Fact]
        public void Driving_engineer_should_not_be_available_if_outside_defined_localization_range()
        {
            //Range where destination should be. 51.45 < latitude < 51.60 and - 0.30 < longitude < 0.05

            DateTime date = DateTime.Now;

            IList<Engineer> engineers = new List<Engineer>();

            Engineer engineer = new Engineer("Engineer1", true, 51.50, 0, TravelMode.Driving, "123456789");

            engineers.Add(engineer);

            TravelDurationApi travelDurationApi = MockRepository.GenerateStub<TravelDurationApi>();
            travelDurationApi.Stub(m => m.GetTravelDuration(engineer.Latitude, engineer.Longitude, 52.50, -2, date, engineer.TravelMode)).Return(new TimeSpan(0, 40, 0));

            TextMessageSender textMessengerSender = MockRepository.GenerateStub<TextMessageSender>();

            RepairService repairService = new ConsoleReportingRepairService(engineers, travelDurationApi, textMessengerSender);

            Action action = () => repairService.RequestEngineer(52.50, -2, date, "Fred");

            action.ShouldThrow<ApplicationException>();
        }

    }
}
