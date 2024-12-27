using System.Text.Json;
using API.Controllers;
using CAPA_DATOS;
using CAPA_NEGOCIO;
using CAPA_NEGOCIO.IA;
using DataBaseModel;
using IA.Dto;
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
		[AuthController]//TODO QUITAR
		public IActionResult ReceiveMessage([FromBody] dynamic message, [FromHeader(Name = "X-Platform")] string platform = null)
		{
			try
			{
				if (string.IsNullOrEmpty(platform))
				{
					// Si el encabezado no está presente, maneja el caso.
					platform = "whatsapp"; // Valor predeterminado o manejo alternativo.
				}
				try
				{
					LoggerServices.AddMessageInfo($"Info: nuevoMensaje: " +  message.toString());
				}
				catch (System.Exception)
				{}
				
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
								WithAgentResponse = resp.WithAgentResponse ?? false,
								ProfileName = "IA",
								Id_Case = resp.Id_case,
							};
							//funcion para encolar mensage para procesar y posterior enviar una respuesta al usuario
							//EnqueueMessage(message, "WebAPI"); //desactualizado no se ve necesario la interaccion encolada

							return  Ok(reply);
						case "WhatsApp": case "messenger":
						 	//var options = new JsonSerializerOptions { ReferenceHandler = ReferenceHandler.Preserve };
							//var message = System.Text.Json.JsonSerializer.Deserialize<UserMessage>(e, options);

							//result.TransactionNumber = message.SessionId;
							var instanceIA = new LlamaClient();
							var responseIA = new ResponseAPI();
							// Ejecutar asincronía sincrónicamente usando GetAwaiter().GetResult()
							var response = instanceIA.GenerateResponse(unifiedMessage).GetAwaiter().GetResult();
						  // var jsonResponse = JsonConvert.DeserializeObject<JsonResponse>(response); 
							//string messageContent = jsonResponse.Message.Content;
							var response2 = responseIA.SendResponseToUser(unifiedMessage, response?.MessageIA).GetAwaiter().GetResult();
							// Responder al usuario según la plataforma
							//MQClient.PublishToQueue("TSK_Receiver_Message", unifiedMessage);
							break;
						/*case "messenger":
							// token EAARa3vZCcGMQBOZCMy5ixTSzPHZAA5ZAz8iaNZByAEDRhqgaw3TjtU9OyZCN4QDCsaf00u1ihTNPaC9sIZACpGQI3tisjofB8GaUvQNTkcVtYYZARTYyqMvfAa4xPUZCDbcqBXVIFJH3EMBGezPLNycWwjttisdbtjNCKotQZAZBGtZBlFLQ0nQgddYKG3Prq24kjreLDgZDZD
							// Responder al usuario según la plataforma
							//MQClient.PublishToQueue("TSK_Receiver_Message", unifiedMessage);
							LlamaClient.
							break;*/

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
				LoggerServices.AddMessageError($"ERROR: procesando mensaje", ex);
				return StatusCode(500, "Internal Server Error");
			}
		}

		private UserMessage ProcessWhatsAppMessage(dynamic message)
		{
			try
			{
				//string whatsAppMessagestringg = message.ToString();
				
				//var whatsAppMessage = message?.entry[0]?.changes[0]?.value?.messages[0];
				var whatsAppMessage = JsonConvert.DeserializeObject<WhatsappBusinessAccount>(message.ToString());
				
				if (whatsAppMessage == null)
					return null;

				return new UserMessage
				{
					Source = "WhatsApp",
					UserId =  whatsAppMessage?.Entry[0]?.Changes[0]?.Value?.Messages[0].From,
					Text =  whatsAppMessage?.Entry[0]?.Changes[0]?.Value?.Messages[0].Text?.Body,
					Timestamp = DateTime.Now // O extraer del mensaje
				};
			}
			catch (Exception ex)
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
					Timestamp = DateTime.Now // O extraer del mensaje
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
			try
			{
				string? AccessToken = SystemConfig.AppConfigurationValue(AppConfigurationList.MettaApi, "AppToken");

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
			catch (System.Exception ex)
			{
				LoggerServices.AddMessageError($"ERROR: Verificando token", ex);

				throw;
			}

		}

	}
}
