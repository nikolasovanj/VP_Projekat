using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Events
{
    public class TransferCompleteEventArgs: EventArgs
    {
        private string _message;

        public TransferCompleteEventArgs(string message)
        {
            _message = message;
        }

        public override string ToString()
        {
            return _message;
        }
    }
}
