using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Controllers;
using APPCORE;
using APPCORE.Security;
using DataBaseModel;

namespace CAPA_NEGOCIO.Gestion_Mensajeria
{
	public class NotificacionesMensajes
	{
		public DateTime? Date { get; set; }
		public string? Content { get; set; }
		public NotificacionType? Type { get; set; }
		public List<NotificacionesMensajes> Get(string? identity)
		{
			UserModel user = AuthNetCore.User(identity);
			var conversaciones = Conversacion.GetConversaciones(identity);
			var mensajesNoLeidos = conversaciones.SelectMany(x => x?.Mensajes ?? [])
			.ToList().Where(m => m.IsMensajeNoLeido(user)).ToList();

			List<NotificacionesMensajes> notificaciones = [];

			mensajesNoLeidos.ForEach(m => notificaciones.Add(new NotificacionesMensajes
			{
				Type = NotificacionType.MENSAJE,
				Date = m.Created_at,
				Content = $"{m.Remitente}: {m.Asunto}"
			}));
			
			return notificaciones;
		}
		
	}
	public enum NotificacionType { MENSAJE, ALERTA, NOTICIA }
}