using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Events
{
    public class SampleRecievedEventArgs : EventArgs
    {
        private string _message;
        public SampleRecievedEventArgs(string message)
        {
            _message = message;
        }

        public override string ToString()
        {
            return _message;
        }
    }
}
