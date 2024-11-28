namespace CAPA_NEGOCIO
{
	public class UserMessage
	{
		public string? Id { get; set; }          // Identificador único del mensaje
		public string Text { get; set; }        // Contenido del mensaje del usuario
		public string Source { get; set; }      // Fuente del mensaje (WebAPI, WhatsApp, Messenger)
		public DateTime Timestamp { get; set; } // Marca de tiempo del mensaje recibido
		public string UserId { get; set; }      // Identificador único del usuario que envió el mensaje
		public string? SessionId { get; set; }   // Identificador de sesión para seguir la conversación
		public string? TypeProcess { get; set; }
		public string? MessageIA { get; set; }
		public bool? WithAgentResponse { get; set; } = false;
		public int? Id_case { get; set; }
	}

}
