using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;


namespace RabbitMQService
{
    public abstract class BaseConsumer
    {
        private readonly IConnection connection;
        private readonly IModel channel;
        private readonly IConfiguration configuration;
       
        private string _queueName;

        public BaseConsumer(string exchangeName, string queueName, IServiceProvider provider)
        {
            _queueName = queueName;
           
            configuration = (IConfiguration)provider.GetService(typeof(IConfiguration));
 
            connection = MQClient.GetConnection();

            channel = connection.CreateModel();

            channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Fanout, durable: true);

            channel.QueueDeclare(queue: queueName,
                                         durable: true,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);

            channel.QueueBind(queue: queueName,
                                exchange: exchangeName,
                                routingKey: "");


        }

        public BaseConsumer(string queueName, IServiceProvider provider)
        {
            _queueName = queueName;
          
            configuration = (IConfiguration)provider.GetService(typeof(IConfiguration));
            
            connection = MQClient.GetConnection();

            channel = connection.CreateModel();

            channel.QueueDeclare(queue: queueName,
                                         durable: true,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);

        }

        public void StartListener()
        {
            var consumer = new EventingBasicConsumer(channel);
            
            consumer.Received += (model, ea) =>
            {                
                var body = ea.Body.ToArray();
                var result = ReceivedMessage?.Invoke(this, body);
                
                var message = result.Message;

                if (result.Messages.Any())
                {
                    message = $"{message}:{Environment.NewLine}{string.Join(Environment.NewLine, result.Messages)}";
                }
 
                System.Threading.Thread.Sleep(100);
            };
            channel.BasicConsume(queue: _queueName,
                                    autoAck: true,
                                    consumer: consumer);
        }

        ~BaseConsumer()
        {
            channel.Dispose();
            connection.Dispose();
        }

        private string GetReference(byte[] args)
        {
            string result = "";
            try
            {
                JsonElement obj = JsonSerializer.Deserialize<JsonElement>(args);
                switch (obj.ValueKind)
                {
                    case JsonValueKind.Array:
                        var val = obj.EnumerateArray();
                        foreach (var det in val)
                        {
                            var resp = GetReference(JsonSerializer.SerializeToUtf8Bytes(det));
                            if (!string.IsNullOrEmpty(resp))
                            {
                                result = resp;
                                break;
                            }
                        }
                        break;
                    case JsonValueKind.Object:
                        var val2 = obj.EnumerateObject();
                        var lst = val2.Where(p => (p.Name.ToLower().Contains("number")
                                                    || p.Name.ToLower().Contains("id")
                                                    || p.Name.ToLower().Contains("code")
                                                  ) && p.Name.Length <= 10).Select(p => (p.Name, p.Value)).OrderByDescending(p => p.Name.Length).FirstOrDefault();
                        if (!string.IsNullOrEmpty(lst.Name))
                        {
                            result = lst.Value.ToString();
                        }
                        else
                        {
                            foreach (var det in val2)
                            {
                                var resp = GetReference(JsonSerializer.SerializeToUtf8Bytes(det.Value));
                                if (!string.IsNullOrEmpty(resp))
                                {
                                    result = resp;
                                    break;
                                }
                            }
                        }
                        break;
                    case JsonValueKind.String:
                        result = obj.ToString();
                        break;
                    default:
                        result = obj.ToString();
                        break;
                }
            }
            catch(Exception ex)
            {
                result = "";
            }            
            return result;
        }

        public delegate MessageDto BaseEventHandler(object sender, byte[] args);

        //public event EventHandler<byte[]> ReceivedMessage;                
        public event BaseEventHandler ReceivedMessage;
    }

    public static class ConsumerSettings
    {
        public static List<String> ConsumerList;
    }
}
