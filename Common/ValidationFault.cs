using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    [DataContract]
    public class ValidationFault : Exception
    {
        string message;
        public ValidationFault(string message)
        {
            this.Message = "Field " + message + " does not have a valid value.";
        }

        [DataMember]
        public string Message { get => message; set => message = value; }
    }
}
