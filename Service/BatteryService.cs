using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class BatteryService : IBattery
    {
        public void EndSession()
        {
            Console.WriteLine("End");
        }

        public void PushSample(EisSample eisSample)
        {
            Console.WriteLine(eisSample.FrequencyHz);
        }

        public void StartSession(EisMeta eisMeta)
        {
            Console.WriteLine("Start");
        }
    }
}
