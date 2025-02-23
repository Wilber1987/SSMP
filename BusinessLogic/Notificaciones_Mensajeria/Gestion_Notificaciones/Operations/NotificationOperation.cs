using API.Controllers;
using CAPA_DATOS;
using CAPA_DATOS.Services;
using CAPA_NEGOCIO.MAPEO;
using DataBaseModel;
using DatabaseModelNotificaciones;
using WhatsAppApi;

namespace CAPA_NEGOCIO.Gestion_Mensajes.Operations
{
	public class NotificationOperation : TransactionalClass
	{

		public ResponseService SaveNotificacion(string identity, NotificationRequest request)
		{
			UserModel user = AuthNetCore.User(identity);

			//1- CREAR UNA TABLA EN BD PARA ALMACENAR NOTIFICACIONES (CREAR EL SCRIPT EN LA CARPETA DE LOS SQL)
			//2- IMPLEMENTAR LOGICA PARA GUARDAR NOTIFICACIONES, ARCHIVOS DE LAS NOTIFICACIONES.
			//3- CREAR CONTROLLADOR PARA INVOCAR ESTE METODO DE ESTA CLASE (NotificationOperation().SaveNotificacion())
			try
			{
				foreach (var file in request.Files ?? [])
				{
					ModelFiles? Response = (ModelFiles?)FileService.upload("Attach\\", file).body;
					file.Value = Response?.Value;
					file.Type = Response?.Type;
				}
				//hacer consultas para obtener el telefono
				List<Security_Users> usuariosSeleccionados = [];

				if (request.NotificationType == NotificationTypeEnum.USUARIOS && request.Usuarios?.Count > 0)
				{
					usuariosSeleccionados = new Security_Users().Where<Security_Users>(FilterData.In("Id_User", request.Usuarios));
				}
				else if (request.NotificationType == NotificationTypeEnum.DEPENDENCIA && request.Dependencias?.Count > 0)
				{
					usuariosSeleccionados = new Security_Users().Where<Security_Users>(FilterData.In("Id_Dependencia", request.Dependencias));
				}
				else if (request.NotificationType != NotificationTypeEnum.LIBRE)
				{
					usuariosSeleccionados = new Security_Users().Get<Security_Users>();
				}
				SaveNotificacion(request, usuariosSeleccionados);

				if (request.Destinatarios != null)
				{
					SaveNotificacionDestinatarios(request);
				}

				LoggerServices.AddMessageInfo($"El usuario con id = {user.UserId} envio una notificación");
				return new ResponseService
				{
					status = 200,
					message = "Notificacion enviada"
				};
			}
			catch (System.Exception)
			{
				//RollBackGlobalTransaction();
				throw;
			}
		}

		private static void SaveNotificacion(NotificationRequest request, List<Security_Users> usuariosSeleccionados)
		{
			foreach (var item in usuariosSeleccionados)
			{
				var newNotificaciones = new Notificaciones
				{
					Id_User = item.Id_User,
					Mensaje = request.Mensaje,
					Titulo = request.Titulo,
					Media = request.Files,
					Enviado = false,
					Leido = false,
					Tipo = request.NotificationType.ToString(),
					Email = item.Mail,
					//Telefono = item.Telefono,
					Fecha = DateTime.Now,
					NotificationsServices = request.NotificationsServices
				};
				newNotificaciones.Save();
			}
		}
		private static void SaveNotificacionDestinatarios(NotificationRequest request)
		{
			foreach (var item in request.Destinatarios ?? [])
			{
				var newNotificaciones = new Notificaciones
				{
					Mensaje = request.Mensaje,
					Titulo = request.Titulo,
					Media = request.Files,
					Enviado = false,
					Leido = false,
					Tipo = request.NotificationType.ToString(),
					Email = item.Correo,
					Telefono = $"{WhatsAppMessage.ObtenerExtensionPorDepartamento(item?.NotificationData?.Departamento)}{item?.Telefono}" ?? "" ,
					Fecha = DateTime.Now,
					NotificationsServices = request.NotificationsServices,
					NotificationData = item.NotificationData
				};
				newNotificaciones.Save();
			}
		}

		public static List<Notificaciones> GetNotificaciones(Notificaciones Inst, string identity)
		{
			UserModel user = AuthNetCore.User(identity);
			Inst.Id_User = user.UserId;
			return Inst.Get<Notificaciones>();
		}
		public static List<Notificaciones> GetNotificaciones(Notificaciones Inst)
		{
			return Inst.Get<Notificaciones>();
		}

		public static ResponseService ReenvioNotificaciones(List<Notificaciones>? notificaciones)
		{
			notificaciones?.ForEach(n => 
			{
				n.Enviado = false;
				n.Leido = false;
				n.Update();
			});
			return new ResponseService
			{
				status= 200,
				message= "Notificaciones Reenviadas"	
			};
		}

		public static List<Notificaciones> GetNotificacionesEnviadas(Notificaciones Inst)
		{
			Inst.orderData = [OrdeData.Asc("Fecha_Envio")];
			return Inst.Where<Notificaciones>(FilterData.Equal("Enviado", true));
		}
	}

	public class NotificactionDestinatarios
	{
		public string? Correo { get; set; }
		public string? Telefono { get; set; }
		public NotificationData? NotificationData { get; set; }
	}

	public class NotificationData
	{
		public string? Departamento { get; set; }
		public string? Direccion { get; set; }
		public string? Destinatario { get; set; }
		public string? Identificacion { get; set; }
		//public string? Numero { get; set; }
		public string? Correlativo { get; set; }
		public string? Fecha { get; set; }
		//public string? NotificaA { get; set; }
		public string? Municipio { get; set; }
		public string? Agencia { get; set; }

		public string? Correo { get; set; }
		public string? Telefono { get; set; }
		public string? Dpi { get;  set; }
		public string? NumeroPaquete { get;  set; }
		public int? Reenvios { get;  set; }
		public List<NotificationsParams>? Params { get; set; }
	}
	
	public class NotificationsParams 
	{
		public string? Name { get; set; }
		public string? Type { get; set; }
		public string? Value { get; set; }
	}
}

/* public bool enviarWhatsapp(){
	 //enviar whatsapp consultado los registros de notificaciones donde el campo enviado sea false 

	 var notificacionesSinLeer = new Notificaciones().Find<Notificaciones>(FilterData.Equal("enviado", false));
	 foreach (var notificacion in notificacionesSinLeer)
	 {
		 //codigo para enviar whatsapp aqui
		 notificacion.Enviado = true;
		 notificacion.Update();
	 }

	 return true;
 }*/
