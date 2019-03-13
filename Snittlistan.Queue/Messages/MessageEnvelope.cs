﻿namespace Snittlistan.Queue.Messages
{
    using System;
    using Newtonsoft.Json;

    public class MessageEnvelope
    {
        public MessageEnvelope(object payload, Uri uri)
        {
            Payload = payload;
            Uri = uri;
        }

        public object Payload { get; }
        public Uri Uri { get; }

        public override string ToString()
        {
            return $"{Uri}: {JsonConvert.SerializeObject(Payload)}";
        }
    }
}