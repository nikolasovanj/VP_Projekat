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
        private bool disposed = false;
        public string Path { get; private set; }
        public SampleReader(EisMeta meta)
        {
            Path = $"../../../Dataset/{meta.BatteryId}/EIS measurements/{meta.TestId}/Hioki/{meta.FileName}";
        }
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
        public IEnumerable<EisSample> CreateSampleFromMeta()
        {
            textReader = File.OpenText(Path);
            List<EisSample> samples = new List<EisSample>();
            string line;
            int row = 0;
            textReader.ReadLine();
            while ((line = textReader.ReadLine()) != null)
            {
                row++;
                samples.Add(EisSample.CreateSample(row, line));
            }
            return samples;
        }

    }
}
