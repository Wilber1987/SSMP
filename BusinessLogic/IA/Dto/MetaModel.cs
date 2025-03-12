using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;


namespace IAMeta.Dto
{
    public class MetaModel
    {
        public string Field { get; set; }
        public MessageValue Value { get; set; }
    }

    public class MessageValue
    {
        [JsonPropertyName("sender")]
        public User Sender { get; set; }

        [JsonPropertyName("recipient")]
        public User Recipient { get; set; }

        [JsonPropertyName("timestamp")]
        public string Timestamp { get; set; }

        [JsonPropertyName("message")]
        public Message Message { get; set; }
    }

    public class User
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
    }

    public class Message
    {
        [JsonPropertyName("mid")]
        public string Mid { get; set; }

        [JsonPropertyName("text")]
        public string Text { get; set; }

        [JsonPropertyName("commands")]
        public List<Command> Commands { get; set; }
    }

    public class Command
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}