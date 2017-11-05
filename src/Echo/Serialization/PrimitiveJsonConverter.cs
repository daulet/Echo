﻿using Newtonsoft.Json;
using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Echo.Serialization
{
    [ComVisible(false)]
    public sealed class PrimitiveJsonConverter : JsonConverter
    {
        public override bool CanRead => false;

        public override bool CanConvert(Type objectType)
        {
            return objectType.GetTypeInfo().IsPrimitive;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            switch (serializer.TypeNameHandling)
            {
                case TypeNameHandling.All:
                    writer.WriteStartObject();

                    writer.WritePropertyName("$type", false);
                    writer.WriteValue(value.GetType().FullName);
                    
                    writer.WritePropertyName("$value", false);
                    writer.WriteValue(value);

                    writer.WriteEndObject();
                    break;
                default:
                    writer.WriteValue(value);
                    break;
            }
        }
    }
}
