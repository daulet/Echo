﻿using System.IO;

namespace Echo.IntegrationTests
{
    internal class EchoWriter : IEchoWriter
    {
        private readonly StreamWriter _streamWriter;

        public EchoWriter(StreamWriter streamWriter)
        {
            _streamWriter = streamWriter;
        }

        public void WriteLine(string echo)
        {
            _streamWriter.WriteLine(echo);
        }
    }
}
