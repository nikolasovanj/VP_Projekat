using Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;

namespace Service
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class BatteryService : IBattery
    {
        private static SessionWriter _session = new SessionWriter();
        private static EventGenerator _eventGenerator = new EventGenerator();
        private static EventListener _listener = new EventListener();
        public void EndSession(string path)
        {
            _session.Files[path].Item1.Close();
            _session.Files[path].Item2.Close();
            _eventGenerator.EndSession();
            Console.WriteLine($"End Session:\t{path.Substring(14)}");
        }

        public void PushSample(EisSample eisSample)
        {
            Console.Write($"Session: {eisSample.File.Substring(14)}\t\t");
            Console.WriteLine($"Tranfer initialized for sample no.{eisSample.RowIndex}...");
            Thread.Sleep(1000);
            _session.Write(eisSample);
            Console.Write($"Session: {eisSample.File.Substring(14)}\t\t");
            Console.WriteLine($"Transfer for sample no.{eisSample.RowIndex} complete!");
            Thread.Sleep(500);
        }

        public string StartSession(EisMeta eisMeta)
        {
            Console.Write("Starting Session:\t");
            string path = _session.RegisterMeta(eisMeta);
            Console.WriteLine(path.Substring(14));
            
            return path;
        }
        public void InitializeEvents()
        {
            _eventGenerator.OnTransferStarted += _listener.HandleEvent;
            _eventGenerator.OnSampleRecieved += _listener.HandleEvent;
            _eventGenerator.OnTransferCompleted += _listener.HandleEvent;
            _eventGenerator.OnWarningRaised += _listener.HandleEvent;
        }
        public void Close()
        {
            _session.Dispose();
        }
    }
}
