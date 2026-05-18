using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    [DataContract]
    public class DataFormatFault : Exception
    {
        string message;

        public DataFormatFault(string message)
        {
            this.Message = "Field " + message + " does not have a valid format.";
        }

        [DataMember]
        public string Message { get => message; set => message = value; }
    }
}
