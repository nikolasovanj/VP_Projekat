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
            BatteryService service = new BatteryService();
            using (ServiceHost host = new ServiceHost(service))
            {
                host.Open();
                service.InitializeEvents();
                Console.WriteLine("Service is open, press any key to close it.");
                Console.ReadKey();
                service.Close();
                host.Close();
            }
            Console.WriteLine("Service is closed");
            Console.ReadKey();
        }
    }
}
