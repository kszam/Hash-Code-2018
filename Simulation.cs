using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashCode2018
{
    class Simulation
    {
        // Next block from input file
        public int RowCount { get; set; }
        public int ColumnCount { get; set; }
        public int VehicleCount { get; set; }
        public int RideCount { get; set; }
        public int Bonus { get; set; }
        public int MaxStepCount { get; set; }

        // Live step at this moment in sim 
        public int StepNo { get; set; } = 0;

        public LinkedList<Ride> Rides { get; set; } = new LinkedList<Ride>();
        public List<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
        public SortedDictionary<int, HashSet<Vehicle>> VehicleEvents { get; set; } = new SortedDictionary<int, HashSet<Vehicle>>();

        public void startSim(string filename)
        {
            ReadInputFile(filename);
            createVehicles();

            FindRides();
            // while (Rides.Count > 0 && StepNo < MaxStepCount)
            while ((VehicleEvents.Count > 0 || Rides.Count > 0 ) && StepNo < MaxStepCount)
            {
                FindRides();
                if (VehicleEvents.Count == 0)
                {
                    break;
                }
                else
                {
                    ProceedToNextEvent();
                }
            }

            WriteOutputFile(filename);
        }

        // Process the list of waiting events
        private void ProceedToNextEvent()
        {
            //Get next event
            KeyValuePair<int, HashSet<Vehicle>> kvp = VehicleEvents.First();
            VehicleEvents.Remove(kvp.Key);
            StepNo = kvp.Key; // the key is the actual step number at this moment

            HashSet<Vehicle> hsv= kvp.Value;

            foreach (var v in hsv) // At this stepno we can have more than one events 
            {
                // set the new position
                v.ActualPosRow = v.PassengerRideToRow;
                v.ActualPosColumn = v.PassengerRideToColumn;

                // no ride at the moment
                v.WithoutRide = true;
                v.ActiveRide = null;
            }
        }

        // find vehicle without ride, find "best" ride for this vehicle
        // Go through rides
        // find ride that we can reach early enough 
        // calculate event times 
        private void FindRides()
        {
            foreach (var v in Vehicles)
            {
                if (v.WithoutRide)
                {
                    Ride r = FindRide(v);
                    if (r != null)
                    {
                        // The vehicle is going to do this ride.
                        v.ActiveRide = r;
                        Rides.Remove(r); // Remove from list of all.
                        v.Rides.Add(r); // Add to vehicle list.
                        v.EmptyRideToRow = r.StartRow;
                        v.EmptyRideToColumn = r.StartColumn;
                        v.PassengerRideToRow = r.FinishRow;
                        v.PassengerRideToColumn = r.FinishColumn;

                        // NextEvent is the step, in which we delivered the passenger.
                        int emptyRide = Distance(v.ActualPosRow, v.ActualPosColumn, r.StartRow, r.StartColumn);
                        int passengerRide = Distance(r.StartRow, r.StartColumn, r.FinishRow, r.FinishColumn);

                        if (StepNo + emptyRide < r.EarliestStart)
                        {
                            v.NextEvent = r.EarliestStart + passengerRide;
                        }
                        else
                        {
                            v.NextEvent = StepNo + emptyRide + passengerRide;
                        }

                        HashSet<Vehicle> value;
                        if (VehicleEvents.TryGetValue(v.NextEvent, out value))
                        {
                            value.Add(v);
                        }
                        else
                        {
                            HashSet<Vehicle> hsv = new HashSet<Vehicle>();
                            hsv.Add(v);
                            VehicleEvents.Add(v.NextEvent, hsv);
                        }
                    }
                }
            }
        }

        Ride FindRide(Vehicle v)
        {
            long bestScore = int.MinValue;
            long score;
            Ride bestRide = null;

            foreach (var ride in Rides)
            {
                // Can I reach the passenger and ride on time ? if not take next ride.
                int emptyRide = Distance(v.ActualPosRow, v.ActualPosColumn, ride.StartRow, ride.StartColumn);
                int passengerRide = Distance(ride.StartRow, ride.StartColumn, ride.FinishRow, ride.FinishColumn);
                if (StepNo + emptyRide + passengerRide >= ride.LatestFinish)
                {
                    continue; // won't make it on time!
                }
                // This ride could be good, score it
                // score = int.MaxValue - (ride.EarliestStart - StepNo) + passengerRide; // can optimize here if time left >>>> 35.783.811 
                // score = int.MaxValue - (ride.EarliestStart - StepNo); // + passengerRide; // can optimize here if time left >>>> 36.414.077
               score = ((long ) int.MaxValue) - (ride.EarliestStart - StepNo) + passengerRide - emptyRide; 
               

                if (score > bestScore)
                {
                    bestScore = score;
                    bestRide = ride;
                }
            }
            return bestRide;
        }

        int Distance(int fromRow, int fromColumn, int toRow, int toColumn)
        {
            return Math.Abs(toRow - fromRow) + Math.Abs(toColumn - fromColumn);
        }

        private void createVehicles()
        {
            for (int i = 0; i < VehicleCount; i++)
            {
                Vehicle v = new Vehicle();
                v.ActualPosColumn = 0;
                v.ActualPosRow = 0;
                Vehicles.Add(v);
            }
        }

        public void WriteOutputFile(string filename)
        {
            using (StreamWriter writer = new StreamWriter(filename + ".out"))
            {
                foreach (var vehicle in Vehicles)
                {
                    if (vehicle.Rides.Count > 0)
                    {
                        writer.Write(vehicle.Rides.Count);

                        foreach (var ride in vehicle.Rides)
                        {
                            writer.Write(" " + ride.ID);
                        }
                        writer.WriteLine();
                    }
                }
            }
        }

        public void ReadInputFile(string filename)
        {
            using (StreamReader reader = File.OpenText(filename + ".in"))
            {
                char[] splitOnThese = { ' ' };

                // First Line
                string readInputLine = reader.ReadLine();
                string[] headerInf = readInputLine.Split(splitOnThese);
                RowCount = int.Parse(headerInf[0]); // R
                ColumnCount = int.Parse(headerInf[1]); // C
                VehicleCount = int.Parse(headerInf[2]); // F
                RideCount = int.Parse(headerInf[3]); // N
                Bonus = int.Parse(headerInf[4]); // B
                MaxStepCount = int.Parse(headerInf[5]); // T

                // Then N lines of individual rides
                for (int i = 0; i < RideCount; i++)
                {
                    readInputLine = reader.ReadLine();
                    headerInf = readInputLine.Split(splitOnThese);

                    Ride r = new Ride();
                    r.ID = i;
                    r.StartRow = int.Parse(headerInf[0]); // a
                    r.StartColumn = int.Parse(headerInf[1]); // b
                    r.FinishRow = int.Parse(headerInf[2]); // x
                    r.FinishColumn = int.Parse(headerInf[3]); // y
                    r.EarliestStart = int.Parse(headerInf[4]); // s
                    r.LatestFinish = int.Parse(headerInf[5]); // f
                    Rides.AddLast(r);
                }
            }
        }
    }
}
