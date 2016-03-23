using System;

namespace BikeRepairService
{
    public interface RepairService
    {
        void RequestEngineer(double latitudeRequired, double longitudeRequired, DateTime date, string customerName);        
    }
}