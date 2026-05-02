using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    [DataContract]
    public class EisSample
    {

        /*RowIndex, FrequencyHz, R_ohm, X_ohm, T_degC,
Range_ohm, TimestampLocal.*/
        int rowIndex = 1;
        float frequencyHz;
        float r_ohm;
        float x_ohm;
        int t_degC;
        int range_ohm;
        DateTime timestampLocal;

        public EisSample(int rowIndex, float frequencyHz, float r_ohm, float x_ohm, int degC, int range_ohm, DateTime timestampLocal)
        {
            RowIndex = rowIndex;
            FrequencyHz = frequencyHz;
            R_ohm = r_ohm;
            X_ohm = x_ohm;
            T_degC = degC;
            Range_ohm = range_ohm;
            TimestampLocal = timestampLocal;
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
        public int T_degC { get => t_degC; set => t_degC = value; }
        [DataMember]
        public int Range_ohm { get => range_ohm; set => range_ohm = value; }
        [DataMember]
        public DateTime TimestampLocal { get => timestampLocal; set => timestampLocal = value; }
    }
}
