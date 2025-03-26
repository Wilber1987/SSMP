using API.Controllers;
using APPCORE;
using APPCORE.Security;
using APPCORE.Services;
using CAPA_NEGOCIO.Gestion_Mensajes.Operations;
using DataBaseModel;
using MimeKit.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseModelNotificaciones
{

	public class Notificaciones : EntityClass
	{
		[PrimaryKey(Identity = true)]
		public int? Id { get; set; }
		public int? Id_User { get; set; }
		public string? Titulo { get; set; }
		public string? Mensaje { get; set; }
		public DateTime? Fecha { get; set; }
		public DateTime? Fecha_Envio { get; set; }
		[JsonProp]
		public List<ModelFiles>? Media { get; set; }
		public bool? Enviado { get; set; }
		public bool? Leido { get; set; }
		public string? Tipo { get; set; }
		public string? Telefono { get; set; }
		public string? Estado { get; set; }
		public string? Email { get; set; }

		public string Year
		{
			get { return Fecha_Envio.GetValueOrDefault().Year.ToString(); }
		}

		public string Month
		{
			get
			{
				var month = Fecha_Envio.GetValueOrDefault().Month;
				return month switch
				{
					1 => "Enero",
					2 => "Febrero",
					3 => "Marzo",
					4 => "Abril",
					5 => "Mayo",
					6 => "Junio",
					7 => "Julio",
					8 => "Agosto",
					9 => "Septiembre",
					10 => "Octubre",
					11 => "Noviembre",
					12 => "Diciembre",
					_ => "Desconocido"
				};
			}
		}

		[JsonProp]
		public List<NotificationsServicesEnum>? NotificationsServices { get; set; }
		[JsonProp]
		public NotificationData? NotificationData { get; set; }



		public ResponseService MarcarComoLeido()
		{
			new Notificaciones { Id = Id, Leido = true }.Update();
			return new ResponseService
			{
				status = 200,
				message = "leido"
			};
		}
		public string GetParam(string value) {
			return this.NotificationData?.Params?.Find(p => p.Name?.ToLower() == value.ToLower())?.Value ?? "";
		}
	}
	public enum NotificacionesStates
	{
		ACTIVA, INACTIVA, VENCIDA
	}
}