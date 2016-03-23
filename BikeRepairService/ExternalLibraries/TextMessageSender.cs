using System;

namespace BikeRepairService.ExternalLibraries
{
    public interface TextMessageSender
    {
        void Send(string phoneNumber, string message);
    }


    internal class DummyTextMessageSender : TextMessageSender
    {
        public void Send(string phoneNumber, string message)
        {
            Console.WriteLine("Sending message to {0} : {1}", phoneNumber, message);
        }
    }
}
