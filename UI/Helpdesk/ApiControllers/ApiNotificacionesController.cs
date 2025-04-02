using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Controllers;
using APPCORE;
using APPCORE.Security;
using CAPA_NEGOCIO.Gestion_Mensajeria;
using CAPA_NEGOCIO.Gestion_Mensajes.Operations;
using DataBaseModel;
using DatabaseModelNotificaciones;
using Microsoft.AspNetCore.Mvc;

namespace UI.Controllers
{
	[ApiController]
	[Route("api/[controller]/[action]")]
	public class ApiNotificacionesController : ControllerBase
	{
		[HttpPost]
		[AuthController(Permissions.NOTIFICACIONES_READER)]
		public List<Notificaciones> getNotificaciones(Notificaciones Inst)
		{
			return NotificationOperation.GetNotificaciones(Inst, HttpContext.Session.GetString("sessionKey"));
		}

		[HttpPost]
		[AuthController(Permissions.SEND_MESSAGE)]
		public ResponseService SaveNotificationRequest(NotificationRequest notificationRequest)
		{
			return new NotificationOperation().SaveNotificacion(HttpContext.Session.GetString("sessionKey"), notificationRequest);
		}

		[HttpPost]
		[AuthController(Permissions.NOTIFICACIONES_READER)]
		public ResponseService MarcarComoLeido(Notificaciones notificationRequest)
		{
			return notificationRequest.MarcarComoLeido();
		}
		#region  CHAT
		//Conversaciones
		[HttpPost]
		[AuthController(Permissions.SEND_MESSAGE)]
		public List<Contacto> getContactos(Contacto Inst)
		{
			return new Conversacion().GetContactos(HttpContext.Session.GetString("sessionKey"), Inst);
		}
		[HttpPost]
		[AuthController(Permissions.SEND_MESSAGE)]
		public List<Conversacion> getConversacion(Contacto Inst)
		{
			return Conversacion.GetConversaciones(HttpContext.Session.GetString("sessionKey"), Inst);
		}
		[HttpPost]
		[AuthController(Permissions.SEND_MESSAGE)]
		public Conversacion? findConversacion(Conversacion Inst)
		{
			return Inst.Find<Conversacion>();
		}
		[HttpPost]
		[AuthController(Permissions.SEND_MESSAGE)]
		public object? saveConversacion(Conversacion Inst)
		{
			return Inst.SaveConversacion(HttpContext.Session.GetString("sessionKey"));
		}
		#endregion
		//Notificaciones

	}
}