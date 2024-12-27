using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using static RabbitMQService.RabbitMQSettings;
using RabbitMQService.Utils;
using RabbitMQ.Client.Events;
using System.Threading.Channels;
using Microsoft.Extensions.Configuration;
using CAPA_DATOS;

namespace RabbitMQService
{
	public class MQClient
	{
		private static IConnection connection;
		private static IModel channel;
		public static RabbitMQSettings settings = new RabbitMQSettings();
		private static IConfiguration Configuration;
		public static ILogger Logger { get; set; }
		public static String LogCnx { get; set; }

		public MQClient(IConfiguration configuration)
		{
			Configuration = configuration; // Asigna la configuración proporcionada en el constructor
			Configuration.GetSection("RabbitMQ").Bind(settings);
		}

		public static void Subscribe(string queueName, Action<string> onMessageReceived)
		{
			try
			{
				channel.QueueDeclare(queue: queueName,
								 durable: false,
								 exclusive: false,
								 autoDelete: false,
			arguments: null);

				var consumer = new EventingBasicConsumer(channel);
				consumer.Received += (model, ea) =>
				{
					var body = ea.Body.ToArray();
					var message = Encoding.UTF8.GetString(body);

					// Llamar a la función onMessageReceived pasando el mensaje recibido
					onMessageReceived(message);
				};

				channel.BasicConsume(queue: queueName,
									 autoAck: true,
									 consumer: consumer);
			}
			catch (System.Exception ex)
			{
				LoggerServices.AddMessageError($"ERROR: procesando Subscribe", ex);
				throw;
			}

		}

		public static bool Publish(string exchangeName, Object obj)
		{
			var result = false;

			try
			{
				if (string.IsNullOrEmpty(settings.Host))
					throw new ArgumentNullException("RabbitMQHost not defined");

				if (!VerifyConnection())
					throw new Exception("No fue posible establecer una conexión con el servidor de Rabbit.");

				channel.ExchangeDeclare(exchangeName, ExchangeType.Fanout, durable: true);

				var options = new JsonSerializerOptions { ReferenceHandler = ReferenceHandler.Preserve };
				var newBody = JsonSerializer.SerializeToUtf8Bytes(obj, options);

				channel.BasicPublish(exchange: exchangeName,
										routingKey: "",
										basicProperties: null,
										body: newBody);

				result = true;
			}
			catch (Exception ex)
			{
				Logger.LogError(ex, ex.Message);
				result = SaveRequest("Exchange", exchangeName, obj);
				if (!result)
					throw;
			}
			return result;
		}

		public static bool PublishToQueue(string queueName, Object obj)
		{
			var result = false;

			try
			{
				if (string.IsNullOrEmpty(settings.Host))
					throw new ArgumentNullException("RabbitMQHost not defined");

				if (!VerifyConnection())
					throw new Exception("No fue posible establecer una conexión con el servidor de Rabbit.");


				channel.QueueDeclare(queue: queueName,
										durable: true,
										exclusive: false,
										autoDelete: false,
										arguments: null);

				var options = new JsonSerializerOptions { ReferenceHandler = ReferenceHandler.Preserve };
				var newBody = JsonSerializer.SerializeToUtf8Bytes(obj, options);

				channel.BasicPublish(exchange: "",
										routingKey: queueName,
										basicProperties: null,
										body: newBody);

				result = true;
			}
			catch (Exception ex)
			{
				Logger.LogError(ex, ex.Message);
				LoggerServices.AddMessageError($"ERROR: procesando mensaje", ex);
				result = SaveRequest("Queue", queueName, obj);
				if (!result)
					throw;
			}
			return result;
		}

		public static bool VerifyConnection()
		{
			if (connection == null)
				return NewConnection();

			return connection.IsOpen;
		}

		private static bool NewConnection()
		{
			var result = false;

			try
			{
				var factory = new ConnectionFactory()
				{
					HostName = settings.Host,
					VirtualHost = string.IsNullOrEmpty(settings.VirtualHost) ? "/" : settings.VirtualHost,
					Port = settings.Port,
					UserName = settings.User,
					Password = settings.Password,
					AutomaticRecoveryEnabled = true
				};

				connection = factory.CreateConnection();
				channel = connection.CreateModel();
				result = true;
			}
			catch (Exception ex)
			{
				LoggerServices.AddMessageError($"ERROR: procesando NewConnection", ex);
				Logger.LogError(ex, ex.Message);
			}

			return result;
		}

		public static IConnection GetConnection()
		{
			if (connection != null) return connection;

			if (NewConnection())
				return connection;

			return null;
		}

		private static bool SaveRequest(string type, string reference, Object obj)
		{
			var result = false;
			try
			{
				var options = new JsonSerializerOptions { ReferenceHandler = ReferenceHandler.Preserve };
				var jsonObj = JsonSerializer.Serialize(obj, options);

				var query = @$"INSERT INTO QueuePendingRequests(
							QprType,
							QprReference,
							QprObject,
							QprCreationDate
							)
					VALUES(
							'{type}',
							'{reference}',
							'{jsonObj}',
							'{DateTime.Now}'                                         
							)";
				SqlTools.ExecNonQuery(query, LogCnx);
				result = true;
			}
			catch (Exception ex)
			{
				LoggerServices.AddMessageError($"ERROR: procesando SaveRequest", ex);
				Logger.LogError(ex, ex.Message);
				result = false;
			}

			return result;
		}
	}
}
