using System;
using System.IO;

namespace ExtractGenBank.GenBank
{
    public class ReaderData : IDisposable
    {
        private readonly StreamReader _reader;
        public string CurrentLine;

        public ReaderData(StreamReader reader) => _reader = reader;

        public void GetNextLine() => CurrentLine = _reader.ReadLine();
        public void Dispose()     => _reader.Dispose();
    }
}