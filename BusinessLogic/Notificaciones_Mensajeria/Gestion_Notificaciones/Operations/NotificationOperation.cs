using API.Controllers;
using CAPA_DATOS;
using CAPA_DATOS.Services;
using CAPA_NEGOCIO.MAPEO;
using DataBaseModel;
using DatabaseModelNotificaciones;

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
                else
                {
                    usuariosSeleccionados = new Security_Users().Get<Security_Users>();
                }
                SendNotificacion(request, usuariosSeleccionados);

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

        private static void SendNotificacion(NotificationRequest request, List<Security_Users> usuariosSeleccionados)
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
                    Fecha = DateTime.Now
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
}