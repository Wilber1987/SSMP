using IA.Dto;
using Newtonsoft.Json;

namespace CAPA_NEGOCIO
{
	public class UserMessage
	{
		public string? Id { get; set; } // Identificador único del mensaje
		public string? Text { get; set; } // Contenido del mensaje del usuario
		public string? Source { get; set; } // Fuente del mensaje (WebAPI, WhatsApp, Messenger)
		public DateTime? Timestamp { get; set; } // Marca de tiempo del mensaje recibido
		public string? UserId { get; set; } // Identificador único del usuario que envió el mensaje
		public string? SessionId { get; set; }// Identificador de sesión para seguir la conversación
		public string? TypeProcess { get; set; }
		public string? MessageIA { get; set; }
		public bool? WithAgentResponse { get; set; } = false;
		public int? Id_case { get; set; }

		public static UserMessage ProcessWhatsAppMessage(dynamic message)
		{
			try
			{
				//string whatsAppMessagestringg = message.ToString();

				//var whatsAppMessage = message?.entry[0]?.changes[0]?.value?.messages[0];
				WhatsappBusinessAccount whatsAppMessage = JsonConvert.DeserializeObject<WhatsappBusinessAccount>(message.ToString());
				var messageEv = whatsAppMessage.Entry[0].Changes[0].Value.Messages[0];
				if (whatsAppMessage == null)
					return null;

				return new UserMessage
				{
					Source = "WhatsApp",
					UserId = messageEv?.From,
					Text = messageEv?.Text?.Body,
					Timestamp = DateTime.Now // O extraer del mensaje
				};
			}
			catch (Exception ex)
			{
				return null;
			}
		}

		public static UserMessage ProcessMessengerMessage(dynamic message)
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

		public static UserMessage ProcessWebApiMessage(dynamic message)
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


	}

}
