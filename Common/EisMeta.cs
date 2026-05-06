using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    [DataContract]
    public enum Test
    {
        [EnumMember] Test_1,
        [EnumMember] Test_2
    }
    [DataContract]
    public class EisMeta
    {
        string batteryId;
        Test testId;
        int soC;
        string fileName;
        int totalRows;
        public EisMeta() { }
        public EisMeta(string batteryId, Test testId, int soC, string fileName, int totalRows)
        {
            BatteryId = batteryId;
            TestId = testId;
            SoC = soC;
            FileName = fileName;
            TotalRows = totalRows;
        }

        private EisMeta(bool create)
        {
            if (create)
            { 
                string path = "../../../Dataset";
                string[] dirs = Directory.GetDirectories(path);
                Random rand = new Random(DateTime.UtcNow.Millisecond);
                path = dirs[rand.Next(dirs.Length)];
                BatteryId = path.Split('\\')[1];

                dirs = Directory.GetDirectories(path + "/EIS measurements");
                path = dirs[rand.Next(dirs.Length)];
                TestId = path.Split('_')[1].Equals("1") ? Test.Test_1 : Test.Test_2;

                string[] files = Directory.GetFiles(path + "/Hioki");
                path = files[rand.Next(files.Length)];
                TotalRows = File.ReadLines(path).Count();

                FileName = path.Split('\\')[3];

                SoC = int.Parse(FileName.Split('_')[3]);
            }
        }

        [DataMember]
        public string BatteryId { get => batteryId; set => batteryId = value; }
        [DataMember]
        public Test TestId { get => testId; set => testId = value; }
        [DataMember]
        public int SoC { get => soC; set => soC = value; }
        [DataMember]
        public string FileName { get => fileName; set => fileName = value; }
        [DataMember]
        public int TotalRows { get => totalRows; set => totalRows = value; }

        public static EisMeta CreateMeta()
        {
            try
            {
                return new EisMeta(true);
            }
            catch (CustomException ex)
            {
                Console.WriteLine(ex.Message);
                return new EisMeta(false);
            }
        }
    }
}
