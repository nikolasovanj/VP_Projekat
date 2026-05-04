using Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    static class FileManipulation
    {
        public static EisMeta CreateMeta(string path)
        { 
            string[] dirs = Directory.GetDirectories(path);
            Random rand = new Random();
            path = dirs[rand.Next(dirs.Length)];
            EisMeta meta = new EisMeta();
            meta.BatteryId = path.Split('\\')[1];

            dirs = Directory.GetDirectories(path+ "/EIS measurements");
            path = dirs[rand.Next(dirs.Length)];
            meta.TestId = path.Split('_')[1].Equals("1") ? Test.Test_1 : Test.Test_2;

            string[] files = Directory.GetFiles(path+"/Hioki");
            path = files[rand.Next(files.Length)];
            meta.TotalRows = File.ReadLines(path).Count();

            meta.FileName = path.Split('\\')[3];

            meta.SoC = int.Parse(meta.FileName.Split('_')[3]);

            return meta;
        }

        public static EisSample CreateSample(int row, string line)
        {
            string[] parts = line.Split(',');
            EisSample sample = new EisSample();
            sample.RowIndex = row;
            sample.FrequencyHz = float.Parse(parts[0]);
            sample.R_ohm = float.Parse(parts[1]);
            sample.X_ohm = float.Parse(parts[2]);
            sample.V = float.Parse(parts[3]);
            sample.T_degC = int.Parse(parts[4]);
            sample.Range_ohm = float.Parse(parts[5]);

            return sample;
        }
    }
}
