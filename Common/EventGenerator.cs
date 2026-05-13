using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class EventGenerator
    {
        public delegate void EventHandler(object sender, EventArgs e);

        public event EventHandler OnTransferStarted;
        public event EventHandler OnSampleRecieved;
        public event EventHandler OnTransferCompleted;
        public event EventHandler OnWarningRaised;

        public void StartSession()
        {
            OnTransferStarted(this, EventArgs.Empty);
        }

        public void RecieveSample()
        {
            OnSampleRecieved(this, EventArgs.Empty);
        }

        public void EndSession()
        {
            OnTransferCompleted(this, EventArgs.Empty);
        }

        public void Warning()
        {
            OnWarningRaised(this, EventArgs.Empty);
        }
    }
}
