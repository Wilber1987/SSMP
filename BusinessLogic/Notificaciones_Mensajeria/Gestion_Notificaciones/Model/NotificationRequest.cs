using API.Controllers;
using APPCORE;
using APPCORE.Security;
using APPCORE.Services;
using CAPA_NEGOCIO.Gestion_Mensajes.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace DataBaseModel
{
	public class NotificationRequest
	{
		public string? MediaUrl { get; set; }//creo que no se va utilizar
		public string? Titulo { get; set; }
		public string? Mensaje { get; set; }
		public List<ModelFiles>? Files { get; set; }
		public NotificationTypeEnum? NotificationType { get; set; }
		public bool? EsResponsable { get; set; }
		public List<NotificactionDestinatarios>? Destinatarios { get; set; }
		public List<int?>? Usuarios { get; set; }
		public List<int?>? Dependencias { get; set; }
		public List<string>? ToAdress { get; set; }//para mensajes directos sin usuarios correos o numeros de telefono
		public List<NotificationsServicesEnum>? NotificationsServices { get; set; }
	}
	public enum NotificationsServicesEnum
	{
		WHATSAPP, MAIL
	} 

	public enum NotificationTypeEnum
	{
		USUARIOS, DEPENDENCIA, LIBRE
	}
}