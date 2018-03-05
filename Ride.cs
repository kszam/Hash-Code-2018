using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashCode2018
{
    class Ride
    {
        public int ID { get; set; }
        public int StartRow { get; set; }
        public int StartColumn { get; set; }
        public int FinishRow { get; set; }
        public int FinishColumn { get; set; }
        public int EarliestStart { get; set; }
        public int LatestFinish { get; set; }
    }
}
