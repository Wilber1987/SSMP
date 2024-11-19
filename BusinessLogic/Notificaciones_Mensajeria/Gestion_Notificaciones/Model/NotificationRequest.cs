using API.Controllers;
using CAPA_DATOS;
using CAPA_DATOS.Security;
using CAPA_DATOS.Services;
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
        public List<int?>? Usuarios { get; set; }
        public List<int?>? Dependencias { get; set; }
        public List<NotificationsServicesEnum>? NotificationsServices { get; set; }
    }
    public enum NotificationsServicesEnum
    {
        WHATSAPP, MAIL
    } 

    public enum NotificationTypeEnum
    {
        USUARIOS, DEPENDENCIA
    }
}