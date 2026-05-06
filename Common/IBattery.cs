using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    [ServiceContract]
    public interface IBattery
    {
        [OperationContract]
        string StartSession(EisMeta eisMeta);
        [OperationContract]
        void PushSample(EisSample eisSample);
        [OperationContract]
        void EndSession(string path);
    }
}
