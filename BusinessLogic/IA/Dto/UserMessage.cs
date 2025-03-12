using System.Threading.Tasks;
using CAPA_DATOS;
using CAPA_DATOS.Services;
using CAPA_NEGOCIO.IA;
using DocumentFormat.OpenXml.Wordprocessing;
using IA.Dto;
using Newtonsoft.Json;

namespace CAPA_NEGOCIO
{
	public class UserMessage
	{
		public string? ServicesIdentification;

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
		public bool IsMetaApi { get { return Source == "WhatsApp"; } }
		public bool IsWithIaResponse { get; set; }
		public ModelFiles? Attach { get; set; }
		public static async Task<UserMessage?> ProcessWhatsAppMessage(dynamic message)
		{
			try
			{
				//string whatsAppMessagestringg = message.ToString();

				//var whatsAppMessage = message?.entry[0]?.changes[0]?.value?.messages[0];
				LoggerServices.AddAction("nuevo mensaje: \n" + message.ToString(), 1);
				WhatsappBusinessAccount whatsAppMessage = JsonConvert.DeserializeObject<WhatsappBusinessAccount>(message.ToString());
				var messageEv = whatsAppMessage?.Entry?[0]?.Changes?[0]?.Value?.Messages[0];
				if (whatsAppMessage == null || messageEv == null)
					return null;

				string mensaje = CalculeMessage(messageEv);
				ModelFiles? Attach = null;
				if (messageEv.Type == "image")
				{
					string type = GetMediaType(messageEv?.Image?.mime_type);
					Attach = await ResponseAPI.DownloadImageAsync(messageEv?.Image?.Id, type);
				}
				else if (messageEv.Type == "document")
				{
					string type = GetMediaType(messageEv?.Document?.mime_type);
					Attach = await ResponseAPI.DownloadImageAsync(messageEv?.Document?.Id, type);
				} 
				return new UserMessage
				{
					Source = "WhatsApp",
					UserId = messageEv?.From,
					Text = mensaje,
					Attach = Attach,
					Timestamp = DateTime.Now, // O extraer del mensaje
					ServicesIdentification = whatsAppMessage?.Entry[0]?.Changes[0]?.Value?.Metadata?.Phone_number_id
				};
			}
			catch (Exception ex)
			{
				LoggerServices.AddMessageError("error, construyendo mensaje", ex);
				return null;
			}
		}

		private static string GetMediaType(string? mime_type)
		{
			if (mime_type.Contains("jpg"))
			{
				return ".jpg";
			}
			else if (mime_type.Contains("jpeg"))
			{
				return ".jpeg";
			}
			else if (mime_type.Contains("png"))
			{
				return ".png";
			}
			else if (mime_type.Contains("pdf"))
			{
				return ".pdf";
			}
			else if (mime_type.Contains("xls"))
			{
				return ".xls";
			}
			else if (mime_type.Contains("xlsx"))
			{
				return ".xlsx";
			}
			else if (mime_type.Contains("doc"))
			{
				return ".doc";
			}
			else if (mime_type.Contains("docx"))
			{
				return ".docx";
			}
			else
			{
				return ".png";
			}
		}

		private static string CalculeMessage(Message messageEv)
		{
			if (messageEv.Type == "image")
			{
				return messageEv?.Image?.caption ?? "";
			}
			else if (messageEv.Type == "document")
			{
				return messageEv?.Document?.caption ?? "";
			}
			return messageEv?.Text?.Body ?? "";
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
