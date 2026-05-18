using Common;
using System;
using System.Collections.Generic;
using System.Configuration;
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

        private readonly double _temperature_difference_max = 3;
        private double _temperature_previous = -999;

        public void EndSession(string path)
        {
            _session.Files[path].Item1.Close();
            _session.Files[path].Item2.Close();
            Console.WriteLine($"End Session:\t{path.Substring(14)}");
        }

        public void PushSample(EisSample eisSample)
        {
            _eventGenerator.RecieveSample(ConfigurationManager.AppSettings["SampleRecieved"] + eisSample.RowIndex);
            if(_temperature_previous == -999)
            {
                _temperature_previous = eisSample.T_degC;
            }
            else
            {
                if(eisSample.T_degC - _temperature_previous > _temperature_difference_max)
                {
                    _eventGenerator.TemperatureSpike("raising");
                }
                else if(eisSample.T_degC - _temperature_previous < -1 * _temperature_difference_max){
                    _eventGenerator.TemperatureSpike("falling");
                }
            }
            Thread.Sleep(1000);
            _session.Write(eisSample);
           _eventGenerator.TransferComplete(ConfigurationManager.AppSettings["SampleComplete"] + eisSample.RowIndex);
            Thread.Sleep(500);
            _temperature_previous = eisSample.T_degC;
        }

        public string StartSession(EisMeta eisMeta)
        {
            _eventGenerator.StartSession(ConfigurationManager.AppSettings["StartSession"]);
            string path = _session.RegisterMeta(eisMeta);
            Console.WriteLine(path.Substring(14));
            
            return path;
        }
        public void InitializeEvents()
        {
            _eventGenerator.OnTransferStarted += _listener.HandleEvent;
            _eventGenerator.OnSampleRecieved += _listener.HandleEvent;
            _eventGenerator.OnTransferCompleted += _listener.HandleEvent;
            _eventGenerator.OnTemperatureSpike += _listener.HandleEvent;
        }
        public void Close()
        {
            _eventGenerator.OnTransferStarted -= _listener.HandleEvent;
            _eventGenerator.OnSampleRecieved -= _listener.HandleEvent;
            _eventGenerator.OnTransferCompleted -= _listener.HandleEvent;
            _eventGenerator.OnWarningRaised -= _listener.HandleEvent;
            _session.Dispose();
        }
    }
}
