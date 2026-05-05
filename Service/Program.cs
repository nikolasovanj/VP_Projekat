using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class Program
    {
        static void Main(string[] args)
        {
            using (ServiceHost host = new ServiceHost(typeof(BatteryService)))
            {
                host.Open();
                Console.WriteLine("Service is open, press any key to close it.");
                Console.ReadKey();
                host.Close();
            }
            Console.WriteLine("Service is closed");
            Console.ReadKey();
        }
    }
}
