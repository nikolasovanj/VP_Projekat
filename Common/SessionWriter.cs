using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class SessionWriter : IDisposable
    {
        public readonly Dictionary<string, Tuple<FileStream, FileStream>> files = new Dictionary<string, Tuple<FileStream, FileStream>> ();
        private readonly string _session = "/session.csv";
        private readonly string _reject = "/reject.csv";
        private readonly string _default = "../../../Data";

        public Dictionary<string, Tuple<FileStream, FileStream>> Files { get { return files; } }
        public SessionWriter()
        {
            if (!Directory.Exists(_default)) Directory.CreateDirectory(_default); 
        }
        private bool disposed = false;
        ~SessionWriter()
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
                    if (files != null)
                    {
                        foreach (var file in files)
                        { 
                            file.Value.Item1.Dispose();
                            file.Value.Item1.Close();
                            file.Value.Item2.Dispose();
                            file.Value.Item2.Close();
                        }
                        files.Clear();
                    }
                }
                disposed = true;
            }
        }
        public string RegisterMeta(EisMeta meta)
        {
            string Path = _default;
            Path += $"/{meta.BatteryId}";
            if (!Directory.Exists(Path)) Directory.CreateDirectory(Path);
            Path += $"/{meta.TestId}";
            if (!Directory.Exists(Path)) Directory.CreateDirectory(Path);
            Path += $"/{meta.SoC}%";
            if (!Directory.Exists(Path)) Directory.CreateDirectory(Path);
            files[Path] = new Tuple<FileStream, FileStream>(
                new FileStream(Path + _session, FileMode.Create),
                new FileStream(Path + _reject, FileMode.Create)
                );
            return Path;
        }
        public void Write(EisSample sample)
        {
            if (true) // TODO Checks
            {
                FileStream fs = files[sample.File].Item1;
                if (fs.Position == 0) 
                {
                    string header = sample.ToCSVHeader();
                    fs.Write(new UTF8Encoding(true).GetBytes(header), 0, header.Length);
                }
                string csv = sample.ToCSV();
                fs.Write(new UTF8Encoding(true).GetBytes(csv), 0, csv.Length);
            }
            else
            {
                FileStream fs = files[sample.File].Item2;
                if (fs.Position == 0)
                {
                    string header = "Timestamp,reason,RowIndex";
                    fs.Write(new UTF8Encoding(true).GetBytes(header), 0, header.Length);
                }
                string reason = "";
                string text = $"{sample.TimestampLocal},rejected: {reason},{sample.RowIndex}";
                fs.Write(new UTF8Encoding(true).GetBytes(text), 0, text.Length);
            }
        }

    }
}
