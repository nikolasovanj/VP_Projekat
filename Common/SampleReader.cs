using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class SampleReader: IDisposable
    {
        private TextReader textReader;
        private int row = 0;
        public string Path { get; private set; }

        public SampleReader(EisMeta meta)
        {
            Path = $"../../../Dataset/{meta.BatteryId}/EIS measurements/{meta.TestId}/Hioki/{meta.FileName}";
            textReader = File.OpenText(Path);
            textReader.ReadLine();
        }

        public SampleReader()
        {
        }

        private bool disposed = false;
        ~SampleReader()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    if (textReader != null)
                    {
                        textReader.Dispose();
                        textReader.Close();
                        textReader = null;
                    }
                }
                disposed = true;
            }
        }
        public EisSample CreateSampleFromMeta(int row, string path)
        {
            string line = textReader.ReadLine();
            EisSample sample = EisSample.CreateSample(row, line); // TODO Checks
            sample.File = path;
            return sample;
            
        }

    }
}
