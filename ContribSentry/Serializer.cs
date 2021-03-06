﻿using Newtonsoft.Json;
using Sentry.Protocol;
using ContribSentry.Internals;
using System;
using System.IO;
using System.Text;
using Sentry;
using Newtonsoft.Json.Converters;
using ContribSentry.Extensions;
using Newtonsoft.Json.Serialization;

namespace ContribSentry
{
    public class Serializer
    {
        private static readonly StringEnumConverter StringEnumConverter = new StringEnumConverter();

        internal Encoding utf8 = Encoding.UTF8;

        internal JsonSerializerSettings jsonSettings;
        
        public Serializer()
        {
            jsonSettings = new JsonSerializerSettings()
            {
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                Formatting = Formatting.None,
                Converters = new[] { StringEnumConverter },
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore
            };
        }

        public void Serialize(ISession session, Stream writer)
        {
            var json = JsonConvert.SerializeObject(session, jsonSettings);
            CopyBytesByKb(utf8.GetBytes(json), writer);
            writer.Flush();
        }

        public void Serialize(SentryEvent @event, Stream writer)
        {
            var json = JsonConvert.SerializeObject(@event, jsonSettings);
            CopyBytesByKb(utf8.GetBytes(json), writer);
            writer.Flush();
        }

        public void Serialize(SentryTracingEvent tracing, Stream writer)
        {
            var json = JsonConvert.SerializeObject(tracing, jsonSettings);
            CopyBytesByKb(utf8.GetBytes(json), writer);
            writer.Flush();
        }

        public void Serialize(SentryEnvelope envelope, Stream writer)
        {
            var nextLineArray = new Byte[1] { 10 };
            var header = (SentryId.Empty.Equals(envelope.Header.EventId) ? "{}" : JsonConvert.SerializeObject(envelope.Header, jsonSettings));
            CopyBytesByKb(utf8.GetBytes(header), writer);
            foreach (var item in envelope.Items)
            {
                writer.Write(nextLineArray, 0, 1);

                var itemTypeJson = "{\"type\":\"" + item.Type.Type.ConvertString() + "\"}";
                writer.Write(utf8.GetBytes(itemTypeJson), 0, itemTypeJson.Length);

                writer.Write(nextLineArray, 0, 1);
                CopyBytesByKb(item.Data, writer);
            }
            writer.Flush();
        }


        private void CopyBytesByKb(byte[] data, Stream writer)
        {
            long size = data.Length - 1024;
            var offset = 0;
            for (; offset < size; offset += 1024)
            {
                writer.Write(data, offset, 1024);
            }
            writer.Write(data, offset, data.Length - offset);
        }
    }
}
