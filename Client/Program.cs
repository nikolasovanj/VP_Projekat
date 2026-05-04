using Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ChannelFactory<IBattery> factory = new ChannelFactory<IBattery>("BatteryService");

            IBattery proxy = factory.CreateChannel();

            EisMeta meta = FileManipulation.CreateMeta("../../../Dataset");
            Console.WriteLine($"Battery id: {meta.BatteryId}\nFile name: {meta.FileName}\nTest id: {meta.TestId}\nSoC: {meta.SoC}\nRows: {meta.TotalRows}");
            string[] lines = File.ReadAllLines($"../../../Dataset/{meta.BatteryId}/EIS measurements/{meta.TestId}/Hioki/{meta.FileName}");
            List<EisSample> samples = new List<EisSample>();
            for (int i = 1; i < meta.TotalRows; i++) 
            {
                samples.Add(FileManipulation.CreateSample(i-1, lines[i]));
            }
            foreach (var sample in samples) 
            { Console.WriteLine(sample.FrequencyHz); }

        }

        private static bool CheckSample(EisSample sample)
        {
            if (sample.FrequencyHz <= 0) return false;
            return true;
        }
    }
}
