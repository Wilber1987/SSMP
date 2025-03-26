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
	public class ApiNotificacionesManageController : ControllerBase
	{
		[HttpPost]
		[AuthController(Permissions.NOTIFICACIONES_MANAGER)]
		public List<Notificaciones> getNotificaciones(Notificaciones Inst)
		{
			return NotificationOperation.GetNotificaciones(Inst);
		}
		[HttpPost]
		[AuthController(Permissions.NOTIFICACIONES_MANAGER)]
		public List<Notificaciones> GetNotificacionesEnviadas(Notificaciones Inst)
		{
			return NotificationOperation.GetNotificacionesEnviadas(Inst);
		}
		
		[HttpPost]
		[AuthController(Permissions.NOTIFICACIONES_MANAGER)]
		public ResponseService ReenvioNotificaciones(AdminNotificacionesRequest notificationRequest)
		{
			return NotificationOperation.ReenvioNotificaciones(notificationRequest.Notificaciones);
		}	
		
		public class AdminNotificacionesRequest 
		{
			public List<Notificaciones>? Notificaciones { get; set; }
		}	

	}
}