using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQService.Utils
{
    public class MessageContent { public string Role { get; set; } public string Content { get; set; } }

    public class JsonResponse
    {
        public string Model { get; set; }
        public DateTime CreatedAt { get; set; }
        public MessageContent Message { get; set; }
        public string DoneReason { get; set; }
        public bool Done { get; set; }
        public long TotalDuration { get; set; }
        public long LoadDuration { get; set; }
        public int PromptEvalCount { get; set; }
        public long PromptEvalDuration { get; set; }
        public int EvalCount { get; set; }
        public long EvalDuration { get; set; }
    }
}