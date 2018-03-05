using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashCode2018
{
    class StartUp
    {
        static void Main(string[] args)
        {
            Simulation s1 = new Simulation();
            s1.startSim(@"a_example");
            s1 = new Simulation();
            s1.startSim(@"b_should_be_easy");
            s1 = new Simulation();
            s1.startSim(@"c_no_hurry");
            s1 = new Simulation();
            s1.startSim(@"d_metropolis");
            s1 = new Simulation();
            s1.startSim(@"e_high_bonus");
        }
    }
}
