using CAPA_NEGOCIO;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RabbitMQ;
using RabbitMQService;


namespace UI.SSMP_IA.ApiControllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class WebhookSsmpIAController : ControllerBase
	{
		[HttpPost]
		public IActionResult ReceiveMessage([FromBody] dynamic message, [FromHeader(Name = "X-Platform")] string platform)
		{
			try
			{
				// Determinar el origen del mensaje
				UserMessage unifiedMessage = platform?.ToLower() switch
				{
					"whatsapp" => ProcessWhatsAppMessage(message),
					"messenger" => ProcessMessengerMessage(message),
					"webapi" => ProcessWebApiMessage(message),
					_ => throw new InvalidOperationException("Unsupported platform")
				};

				if (unifiedMessage != null)
				{
					switch (unifiedMessage.Source)
					{
						case "webapi":
							//funcion para consultar directamente a la IA y retorna respuesta
							var resp = EnqueueMessageWebApi(unifiedMessage, "webapi");

							ResponseWebApi reply = new ResponseWebApi()
							{
								Reply = resp.MessageIA,
								WithAgentResponse = false,
								ProfileName = "IA",
								Id_Case = resp.Id_case,
							};
							//funcion para encolar mensage para procesar y posterior enviar una respuesta al usuario
							//EnqueueMessage(message, "WebAPI"); //desactualizado no se ve necesario la interaccion encolada

							return  Ok(reply);
							break;
						case "whatsapp":
							// Responder al usuario según la plataforma
							MQClient.PublishToQueue("TSK_Receiver_Message", unifiedMessage);
							break;
						case "messenger":
							// token EAARa3vZCcGMQBOZCMy5ixTSzPHZAA5ZAz8iaNZByAEDRhqgaw3TjtU9OyZCN4QDCsaf00u1ihTNPaC9sIZACpGQI3tisjofB8GaUvQNTkcVtYYZARTYyqMvfAa4xPUZCDbcqBXVIFJH3EMBGezPLNycWwjttisdbtjNCKotQZAZBGtZBlFLQ0nQgddYKG3Prq24kjreLDgZDZD
							// Responder al usuario según la plataforma
							MQClient.PublishToQueue("TSK_Receiver_Message", unifiedMessage);
							break;

						default:
							break;
					}

					return Ok("EVENT_RECEIVED");
				}

				return BadRequest("Message could not be processed.");
			}
			catch (Exception ex)
			{
				// Manejo de errores genérico
				Console.WriteLine($"Error processing message: {ex}");
				return StatusCode(500, "Internal Server Error");
			}
		}

		private UserMessage ProcessWhatsAppMessage(dynamic message)
		{
			try
			{
				var whatsAppMessage = message?.entry[0]?.changes[0]?.value?.messages[0];
				if (whatsAppMessage == null)
					return null;

				return new UserMessage
				{
					Source = "WhatsApp",
					UserId = whatsAppMessage.from,
					Text = whatsAppMessage.text?.body,
					Timestamp = DateTime.UtcNow // O extraer del mensaje
				};
			}
			catch
			{
				return null;
			}
		}

		private UserMessage ProcessMessengerMessage(dynamic message)
		{
			try
			{
				var entry = message?.entry?[0];
				var messagingEvent = entry?.messaging?[0];
				if (messagingEvent == null)
					return null;

				return new UserMessage
				{
					Source = "Messenger",
					UserId = messagingEvent.sender?.id,
					Text = messagingEvent.message?.text,
					Timestamp = DateTime.UtcNow // O extraer del mensaje
				};
			}
			catch
			{
				return null;
			}
		}

		private UserMessage ProcessWebApiMessage(dynamic message)
		{
			try
			{
				string jsonString = System.Text.Json.JsonSerializer.Serialize(message);
				var jsonElement = System.Text.Json.JsonSerializer.Deserialize<UserMessage>(jsonString.ToString());

				if (jsonElement == null)
					return null;

				return jsonElement;

			}
			catch
			{
				return null;
			}
		}

		//Devuelve respuesta al usuario
		private static UserMessage EnqueueMessageWebApi(UserMessage message, string source)
		{
			message.Source = source; // Añadimos la fuente al mensaje

			var instanceIA = new LlamaClient();

			// Ejecutar asincronía sincrónicamente usando GetAwaiter().GetResult()
			var response = instanceIA.GenerateResponse(message).GetAwaiter().GetResult();
		   // var jsonResponse = JsonConvert.DeserializeObject<JsonResponse>(response);
		   // string messageContent = jsonResponse.Message.Content;

			return response;
		}

		[HttpGet]
		public IActionResult VerifyToken()
		{
			string AccessToken = "EAARa3vZCcGMQBO5oiYGZAAF4t";

			var token = Request.Query["hub.verify_token"].ToString();
			var challenge = Request.Query["hub.challenge"].ToString();

			if (challenge != null && token != null && token == AccessToken)
			{
				return Ok(challenge);
			}
			else
			{
				return BadRequest();
			}
		}

	}
}
