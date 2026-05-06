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

            EisMeta meta;
            SampleReader sr = new SampleReader();
            int temp = 0;
            while (true)
            {
                temp = PrintMenu();
                if (temp == 2) break;
                else if (temp == -1) continue;
                try
                {
                    meta = EisMeta.CreateMeta();
                    sr = new SampleReader(meta);
                    string path = proxy.StartSession(meta);
                    for (int i = 0; i < meta.TotalRows-1; i++)
                    {
                        EisSample sample = sr.CreateSampleFromMeta(i, path);
                        proxy.PushSample(sample);
                    }

                    proxy.EndSession(path);
                    sr.Dispose();
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
            Console.WriteLine("[ANY]. Send samples\n2. Exit");
            Console.Write("Choose action: ");
            if(int.TryParse(Console.ReadLine(), out int ret))
            {
                return ret;
            }
            return -1;
        }

        
    }
}
