using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;


namespace IAMeta.Dto
{
    public class MetaModel
    {
        public string Object { get; set; }
        public List<Entry> Entry { get; set; }
    }

    public class Entry
    {
        public long Time { get; set; }
        public string Id { get; set; }
        public List<Messaging> Messaging { get; set; }
    }

    public class Messaging
    {
        public Sender Sender { get; set; }
        public Recipient Recipient { get; set; }
        public long Timestamp { get; set; }
        public Message Message { get; set; }
    }

    public class Sender
    {
        public string Id { get; set; }
    }

    public class Recipient
    {
        public string Id { get; set; }
    }

    public class Message
    {
        public string Mid { get; set; }
        public string Text { get; set; }
    }

}