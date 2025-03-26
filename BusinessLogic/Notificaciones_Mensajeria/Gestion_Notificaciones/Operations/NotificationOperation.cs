using System.Reflection;
using API.Controllers;
using APPCORE;
using APPCORE.Services;
using CAPA_NEGOCIO.MAPEO;
using DataBaseModel;
using DatabaseModelNotificaciones;
using Org.BouncyCastle.Asn1.Ocsp;
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
			Transactional_Configuraciones defaultMessageTemplate = Transactional_Configuraciones.GetParam(ConfiguracionesThemeEnum.MESSAGE_TEMPLATE, defaultTemplate);

			foreach (var item in request.Destinatarios ?? [])
			{
				var newNotificaciones = new Notificaciones
				{
					Mensaje = GetMensaje(request, item, defaultMessageTemplate),
					Titulo = request.Titulo,
					Media = request.Files,
					Enviado = false,
					Leido = false,
					Tipo = request.NotificationType.ToString(),
					Email = item.Correo,
					Telefono = $"{WhatsAppMessage.ObtenerExtensionPorDepartamento(item?.NotificationData?.Departamento)}{item?.Telefono}" ?? "",
					Fecha = DateTime.Now,
					NotificationsServices = request.NotificationsServices,
					NotificationData = item.NotificationData
				};
				newNotificaciones.Save();
			}
		}

		private static string GetMensaje(NotificationRequest request, NotificactionDestinatarios item, Transactional_Configuraciones defaultMessageTemplate)
		{

			if (request.Mensaje == null && SystemConfig.IsWhatsAppActive() == true)
			{
				string messageTemplate = defaultMessageTemplate.Valor ?? "";
				
				PropertyInfo[] properties = item.NotificationData?.GetType()?.GetProperties() ?? [];
				foreach (PropertyInfo property in properties)
				{
					string propertyName = property.Name;
					object? propertyValue = property?.GetValue(item.NotificationData, null);
					string placeholder = "{{"+propertyName+"}}";
					if (propertyValue != null)
					{
						messageTemplate  = messageTemplate.Replace(placeholder, propertyValue.ToString());
					}
					else
					{
						messageTemplate = messageTemplate.Replace(placeholder, "");
					}
				}
				return messageTemplate;
			}
			return request.Mensaje?.ToString() ?? "";
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
				n.NotificationData!.Reenvios = 0;
				n.Update();
			});
			return new ResponseService
			{
				status = 200,
				message = "Notificaciones Reenviadas"
			};
		}

		public static List<Notificaciones> GetNotificacionesEnviadas(Notificaciones Inst)
		{
			Inst.orderData = [OrdeData.Asc("Fecha_Envio")];
			return Inst.Where<Notificaciones>(FilterData.Equal("Enviado", true));
		}
		static string defaultTemplate = @"¡Hola {{Destinatario}}! En Correos nos encanta servirte. <br> <br> 
Ha llegado un paquete a tu nombre y lo tenemos resguardado en la oficina de Fardos Postales - SAT, ubicada en el Palacio de Correos, zona 1, 2do nivel. Of. 203
<br> <br> 
Recuerda traer tu DPI y brindar el número {{NumeroPaquete}} para el respectivo pago de los impuestos, que apliquen segun las disposiciones de la SAT y así continuar el proceso de entrega.
<br> <br> 
Si en caso se te dificulta venir, te ofrecemos el servicio de desaduanaje remoto el cual consiste en realizar, con tu debida autorización, todos los procesos por ti y entregarte el paquete en la dirección consignada en el mismo. 
<br> <br> 
¿Qué opción has decidido? 
1. Visitarnos en el Palacio de Correos
2. Utilizar nuestro servicio de desaduanaje
<br> <br> 
¡Qué tengas un estupendo dia!";
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
		public string? Email { get; set; }
		public string? Telefono { get; set; }
		public string? Dpi { get; set; }
		public string? Nit { get; set; }
		public string? NumeroPaquete { get; set; }
		public string? Fecha_del_envio_de_notificacion { get; set; }
		public string? NumeroAduana { get; set; }
		public int? Reenvios { get; set; }
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
