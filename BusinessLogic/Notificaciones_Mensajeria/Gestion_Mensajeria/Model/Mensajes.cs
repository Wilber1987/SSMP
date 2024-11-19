using API.Controllers;
using CAPA_DATOS;
using CAPA_DATOS.Security;
using CAPA_NEGOCIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
namespace DataBaseModel
{
    public class Mensajes : EntityClass
    {
        [PrimaryKey(Identity = true)]
        public int? Id_mensaje { get; set; }
        public string? Foto { get; set; }
        public string? Remitente { get; set; }
        public int? Usuario_id { get; set; }
        [JsonProp]
        public List<Destinatario>? Destinatarios { get; set; }
        public string? Asunto { get; set; }
        public string? Body { get; set; }
        public int? Id_conversacion { get; set; }
        public DateTime? Created_at { get; set; }
        public DateTime? Updated_at { get; set; }
        public bool? Enviado { get; set; }
        public bool? Leido { get; set; }


        /* [ManyToOne(TableName = "Conversacion", KeyColumn = "Id_conversacion", ForeignKeyColumn = "Id_conversacion")]
        public Conversacion? Conversacion { get; set; }
        [ManyToOne(TableName = "Security_Users", KeyColumn = "Id_User", ForeignKeyColumn = "Id_User")]
        public Security_Users? Security_Users { get; set; } */
        public List<Mensajes> GetMessage(string? identity, Conversacion inst)
        {

            try
            {
                BeginGlobalTransaction();
                if (inst.Id_conversacion == null)
                {
                    throw new Exception("error al recuperar la conversación");
                }
                Id_conversacion = inst.Id_conversacion;
                filterData = inst.filterData;
                var Messages = Get<Mensajes>();
                UserModel user = AuthNetCore.User(identity);
                //MyConnection?.Open();

                Messages.ForEach(m =>
                {
                    if (m.IsMensajeNoLeido(user))
                    {
                        m.MarcarComoLeido(user);
                    }
                });
                CommitGlobalTransaction();
                return Messages;
            }
            catch (Exception ex)
            {                
                RollBackGlobalTransaction();
                LoggerServices.AddMessageError("error al recuperar la conversación", ex);
                throw new Exception("error al recuperar la conversación");
            }

        }
        public bool IsMensajeNoLeido(UserModel user)
        {
            return Destinatarios?.Find(d => d.Id_User == user.UserId && d.Leido != true) != null;
        }
        public void MarcarComoLeido(UserModel user)
        {
            var destinatario = Destinatarios?.Find(d => d.Id_User == user.UserId && d.Leido != true);
            if (destinatario != null)
            {
                destinatario.Leido = true;
                Update();
            }
        }
        public ResponseService SaveMessage(string? identity)
        {
            try
            {
                BeginGlobalTransaction();
                UserModel user = AuthNetCore.User(identity);
                CAPA_NEGOCIO.MAPEO.Tbl_Profile profile = CAPA_NEGOCIO.MAPEO.Tbl_Profile.Get_Profile(user);

                Usuario_id = user.UserId;
                Remitente = profile.GetNombreCompleto();
                Foto = profile.Foto;
                Conversacion? conversacion = new Conversacion { Id_conversacion = Id_conversacion }
                   .Find<Conversacion>();

                Destinatarios = conversacion?.Conversacion_usuarios.Where(C => C.Id_usuario != Usuario_id)
                .Select(C => new Destinatario
                {
                    Correo = C.Security_Users?.Mail,
                    Id_User = C.Id_usuario,
                    Leido = false,
                    Enviado = false,
                    Nombre = $"{C.Security_Users?.Get_Profile().GetNombreCompleto()}"
                }).ToList();

                Created_at = DateTime.Now;
                Updated_at = DateTime.Now;
                Save();
                CommitGlobalTransaction();

                return new ResponseService { status = 200, message = "Mensaje guardado" };
            }
            catch (Exception)
            {
                RollBackGlobalTransaction();
                throw;
            }
        }
    }


    public class Destinatario
    {
        public int? Id_User { get; set; }
        public string? Correo { get; set; }
        public string? Nombre { get; set; }
        public bool Enviado { get; set; }
        public bool Leido { get; set; }
    }
}
