﻿using Echo.Serialization;
using Echo.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Script.Serialization;

namespace Echo.Core
{
    internal class InvocationReader : IInvocationReader
    {
        private readonly JavaScriptSerializer _serializer = new JavaScriptSerializer();
        private readonly IInvocationEntryReader _reader;

        internal InvocationReader(IInvocationEntryReader reader)
        {
            _reader = reader;
        }

        public InvocationResult FindReturnValue(MethodInfo methodInfo, object[] arguments)
        {
            var serializedEntries = _reader.ReadAllInvocationEntries();
            var entries = new HashSet<InvocationEntry>(
                serializedEntries.Select(x => _serializer.Deserialize<InvocationEntry>(x)));

            foreach (var entry in entries)
            {
                if (string.Equals(entry.Method, methodInfo.Name, StringComparison.OrdinalIgnoreCase))
                {
                    if (InvocationUtility.IsArgumentListMatch(entry.Arguments, arguments))
                    {
                        // TODO can't remove while enumerating
                        entries.Remove(entry);

                        return entry.InvocationResult;
                    }
                }
            }
            throw new NoRecordingFoundException();
        }
    }
}