using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IA.DtoWhatsApp
{
    public class WhatsappBusinessAccount
    {
        public string Object { get; set; }
        public List<Entry> Entry { get; set; } = [];
    }

    public class Entry
    {
        public string Id { get; set; }
        public List<Change> Changes { get; set; } = [];
    }

    public class Change
    {
        public Value Value { get; set; }
    }

    public class Value
    {
        public string MessagingProduct { get; set; }
        public Metadata Metadata { get; set; }
        public List<Contact> Contacts { get; set; } = [];
        public List<Message> Messages { get; set; } = [];
    }

    public class Metadata
    {
        public string? Display_phone_number { get; set; }
        public string? Phone_number_id { get; set; }
    }

    public class Contact
    {
        public Profile Profile { get; set; }
        public string WaId { get; set; }
    }

    public class Profile
    {
        public string Name { get; set; }
    }

    public class Message
    {
        public string From { get; set; }
        public string Id { get; set; }
        public string Timestamp { get; set; }
        public string Type { get; set; }
        public Text? Text { get; set; }
        public Image? Image { get; set; }
        public Document? Document { get; set; }
    }

    public class Image
    {
        public string caption { get; set; }
        public string mime_type { get; set; }
        public string sha256 { get; set; }
        public string link { get; set; }
        public string Id { get; set; }
    }
    public class Document
    {
        public string caption { get; set; }
        public string mime_type { get; set; }
        public string sha256 { get; set; }
        public string link { get; set; }
        public string Id { get; set; }
    }

    public class Text
    {
        public string Body { get; set; }
    }
}
