using Common.Events;
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

        public void StartSession(string message)
        {
            OnTransferStarted(this, new SessionStartEventArgs(message));
        }

        public void RecieveSample(string message)
        {
            OnSampleRecieved(this, new SampleRecievedEventArgs(message));
        }

        public void TransferComplete(string message)
        {
            OnTransferCompleted(this, new TransferCompleteEventArgs(message));
        }

        public void Warning(string message)
        {
            OnWarningRaised(this, new WarningEventArgs(message));
        }
    }
}
