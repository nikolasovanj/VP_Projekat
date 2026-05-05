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
    public class EisSample
    {
        int rowIndex = 1;
        float frequencyHz;
        float r_ohm;
        float x_ohm;
        float v;
        int t_degC;
        float range_ohm;
        DateTime timestampLocal;

        public EisSample() { }

        public EisSample(int rowIndex, float frequencyHz, float r_ohm, float x_ohm, float v, int degC, float range_ohm, DateTime timestampLocal)
        {
            RowIndex = rowIndex;
            FrequencyHz = frequencyHz;
            R_ohm = r_ohm;
            X_ohm = x_ohm;
            V = v;
            T_degC = degC;
            Range_ohm = range_ohm;
            TimestampLocal = timestampLocal;
        }

        private EisSample(int row, string line)
        {

            string[] parts = line.Split(',');
            RowIndex = row;
            FrequencyHz = float.Parse(parts[0]);
            R_ohm = float.Parse(parts[1]);
            X_ohm = float.Parse(parts[2]);
            V = float.Parse(parts[3]);
            T_degC = int.Parse(parts[4]);
            Range_ohm = float.Parse(parts[5]);
        }

        [DataMember]
        public int RowIndex { get => rowIndex; set => rowIndex = value; }
        [DataMember]
        public float FrequencyHz { get => frequencyHz; set => frequencyHz = value; }
        [DataMember]
        public float R_ohm { get => r_ohm; set => r_ohm = value; }
        [DataMember]
        public float X_ohm { get => x_ohm; set => x_ohm = value; }
        [DataMember]
        public float V { get =>  v; set => v = value; }
        [DataMember]
        public int T_degC { get => t_degC; set => t_degC = value; }
        [DataMember]
        public float Range_ohm { get => range_ohm; set => range_ohm = value; }
        [DataMember]
        public DateTime TimestampLocal { get => timestampLocal; set => timestampLocal = value; }
        public static EisSample CreateSample(int row, string line)
        {
            EisSample sample = new EisSample();
            try
            {
                sample = new EisSample(row, line);
            }
            catch (CustomException ex)
            { 
                Console.WriteLine(ex.Message);
            }
            return sample;
        }
    }
}
