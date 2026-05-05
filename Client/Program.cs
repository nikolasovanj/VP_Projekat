using Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ChannelFactory<IBattery> factory = new ChannelFactory<IBattery>("BatteryService");
            IBattery proxy = factory.CreateChannel();

            string datasetPath = ConfigurationManager.AppSettings["dataset"];
            EisMeta meta = EisMeta.CreateMeta(datasetPath);
            SampleReader sr = new SampleReader(meta);
            int temp = 0;
            while (true)
            {
                temp = PrintMenu();
                if (temp == 2) break;
                if (temp == -1) continue;
                try
                {
                    proxy.StartSession(meta);
                    List<EisSample> samples = sr.CreateSampleFromMeta() as List<EisSample>;
                    sr.Dispose();
                    foreach (var sample in samples) 
                    { proxy.PushSample(sample); }

                    proxy.EndSession();
                }
                catch
                {
                    sr.Dispose();
                    Console.WriteLine("Server closed");
                }

            }
        }

        private static int PrintMenu()
        {
            Console.WriteLine("1. Send samples\n2. Exit");
            Console.Write("Choose action: ");
            if(int.TryParse(Console.ReadLine(), out int ret))
            {
                return ret;
            }
            return -1;
        }

        
    }
}
