using System;
using System.Collections.Generic;
using System.IO;

namespace Alpha.Toolkit.IO
{
    class CsvLogger : IDisposable
    {
        public String FileName { get; set; }
        public int Sampling { get; set; }
        public char Separator { get; set; }
        private readonly StreamWriter _streamWriter;
        public readonly List<ICsvLoggable> _items;
        private int _counter;
        public CsvLogger(string fileName, int sampling, char separator = ',')
        {
            if (fileName == null) throw new ArgumentNullException("fileName");
            FileName = fileName;
            Sampling = sampling;
            Separator = separator;
            Directory.CreateDirectory(Path.GetDirectoryName(fileName));
            _streamWriter = new StreamWriter(fileName);
            _items = new List<ICsvLoggable>();
            _counter = 0;
        }

        public void Register(params ICsvLoggable[] items)
        {
            foreach (ICsvLoggable item in items)
                _items.Add(item);
        }

        public void Log()
        {
            if(((_counter++) % Sampling) != 0)
                return;
            foreach (ICsvLoggable item in _items)
                _streamWriter.Write(item.ToCsv()+Separator);
            _streamWriter.Write(Environment.NewLine);
        }

        public void Dispose()
        {
            _streamWriter.Flush();
            _streamWriter.Close();
        }
    }
}
