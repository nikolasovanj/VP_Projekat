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
        [EnumMember] Test_1=1,
        [EnumMember] Test_2=2
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
    }
}
