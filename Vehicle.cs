using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashCode2018
{
    class Vehicle
    {
        public bool WithoutRide { get; set; } = true;
        public int ActualPosRow { get; set; }
        public int ActualPosColumn { get; set; }
        public int EmptyRideToRow { get; set; }
        public int EmptyRideToColumn { get; set; }
        public int PassengerRideToRow { get; set; }
        public int PassengerRideToColumn { get; set; }
        public Ride ActiveRide { get; set; }
        public int NextEvent { get; set; }
        public List<Ride> Rides { get; set; } = new List<Ride>();
    }
}
