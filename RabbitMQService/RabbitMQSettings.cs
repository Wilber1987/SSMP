using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQService
{
    public class RabbitMQSettings
    {

            public string Host { get; set; } = "localhost";
            public string VirtualHost { get; set; } = "/";
            public int Port { get; set; } = 5672;
            public string User { get; set; } = "guest";
            public string Password { get; set; } = "guest";
            public string QueueName { get; set; }
    }
}
