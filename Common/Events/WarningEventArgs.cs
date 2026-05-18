using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Events
{
    public class WarningEventArgs: EventArgs
    {
        private string _message;

        public WarningEventArgs(string message)
        {
            _message = message;
        }

        public override string ToString()
        {
            return _message;
        }
    }
}
