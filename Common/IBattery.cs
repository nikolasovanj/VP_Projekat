using System;
using System.Collections.Generic;
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
        void StartSession(EisMeta eisMeta);
        [OperationContract]
        void PushSample(EisSample eisSample);
        [OperationContract]
        void EndSession();
    }
}
