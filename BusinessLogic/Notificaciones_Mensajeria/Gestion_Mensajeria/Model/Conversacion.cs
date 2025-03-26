using API.Controllers;
using APPCORE;
using CAPA_NEGOCIO;
using CAPA_NEGOCIO.Gestion_Mensajeria;
using CAPA_NEGOCIO.MAPEO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
namespace DataBaseModel
{
    public class Conversacion : EntityClass
    {
        [PrimaryKey(Identity = true)]
        public int? Id_conversacion { get; set; }
        public string? Descripcion { get; set; }
        public int? MensajesPendientes { get; set; }
        public DateTime? Fecha_Ultimo_Mensaje { get; set; }

        [OneToMany(TableName = "Conversacion_usuarios", KeyColumn = "Id_conversacion", ForeignKeyColumn = "Id_conversacion")]
        public List<Conversacion_usuarios>? Conversacion_usuarios { get; set; }

        [OneToMany(TableName = "Mensajes", KeyColumn = "Id_conversacion", ForeignKeyColumn = "Id_conversacion")]
        public List<Mensajes>? Mensajes { get; set; }

        public static List<Conversacion> GetConversaciones(string? identity)
        {
            UserModel user = AuthNetCore.User(identity);
            List<Conversacion_usuarios> Conversaciones_usuarios = new Conversacion_usuarios
            { Id_usuario = user.UserId }.Get<Conversacion_usuarios>();
            return [.. Conversaciones_usuarios.Select(u => {
                DateTime? last = u.Conversacion?.Mensajes?.Select(m => m.Created_at).ToList().Max();
                if (u.Conversacion != null)
                {
                    u.Conversacion.Fecha_Ultimo_Mensaje = last;
                    u.Conversacion.MensajesPendientes = u.Conversacion?.Mensajes
                        .SelectMany(m => m.Destinatarios ?? [])
                        .Where(r =>r.Id_User == user.UserId && r.Leido != true).ToList().Count;
                    u.Conversacion!.Mensajes = null;
                }
                return u.Conversacion;
            }).OrderByDescending(c => c?.Fecha_Ultimo_Mensaje)];
        }
        public List<Contacto> GetContactos(string? identity, Contacto contacto)
        {
            UserModel user = AuthNetCore.User(identity);

            return new Tbl_Profile()
                .Where<Tbl_Profile>(
                    FilterData.Distinc("IdUser", user.UserId),
                    FilterData.Limit(50),
                    FilterData.Like("Nombre_Completo", contacto.Nombre_Completo)
                )
                .Select(u =>
                {
                    int count = new Mensajes
                    {
                        Usuario_id = u.IdUser,
                        Leido = false

                    }.Count(
                        FilterData.Like("Destinatarios", $"Id_User : {user.UserId}")//TODO REPARAR ESTE ERROR,FilterData.Equal("Leido", "0") 
                    );
                    return new Contacto
                    {
                        Id_User = u.IdUser,
                        Nombre_Completo = u.GetNombreCompleto() ?? u.Nombres,
                        Foto = u.Foto,
                        Mensajes = count
                    };
                }).ToList();
        }

        public static List<Conversacion> GetConversaciones(string? identity, Contacto contacto)
        {
            UserModel user = AuthNetCore.User(identity);
            List<Conversacion_usuarios> Conversaciones_usuarios = new Conversacion_usuarios
            {
                Id_usuario = user.UserId,
                filterData = [
                    FilterData.Limit(30),
                    FilterData.Like("Name", contacto.Nombre_Completo)
                ]
            }.Get<Conversacion_usuarios>();

            //recuperar Conversaciones
            List<Conversacion> conversaciones = [.. Conversaciones_usuarios.Select(u => {
                DateTime? last = u.Conversacion?.Mensajes?.Select(m => m.Created_at).ToList().Max();
                if (u.Conversacion != null)
                {
                    u.Conversacion.Fecha_Ultimo_Mensaje = last;
                    u.Conversacion.MensajesPendientes = u.Conversacion?.Mensajes?
                        .SelectMany(m => m.Destinatarios ?? [])
                        .Where(r =>r.Id_User == user.UserId && r.Leido != true).ToList().Count;
                    u.Conversacion!.Mensajes = null;
                }
                return u.Conversacion;
            }).OrderByDescending(c => c?.Fecha_Ultimo_Mensaje)];

            //Recuperar contactos si hay pocas conversaciones
            if (conversaciones.Count < 30)
            {
                Security_Users entity = new Security_Users
                {
                    filterData = conversaciones.Count == 0 ? [] : [FilterData.NotIn("Id_User", conversaciones
                            .SelectMany(c => c.Conversacion_usuarios ?? [])
                            .Select(c => c.Id_usuario).ToArray())]
                };
                List<Security_Users> contactos = entity.Where<Security_Users>(
                        FilterData.Distinc("Id_User", user.UserId),
                        FilterData.Limit(30 - conversaciones.Count),
                        FilterData.Like("Nombres", contacto.Nombre_Completo)
                    );
                conversaciones.AddRange(contactos?.Select(c =>
                {
                    CAPA_NEGOCIO.MAPEO.Tbl_Profile tbl_Profile = c.Get_Profile();
                    return new Conversacion
                    {
                        Conversacion_usuarios = [new Conversacion_usuarios {
                            Name = tbl_Profile.GetNombreCompleto(),
                            Id_usuario = c.Id_User,
                            Avatar = tbl_Profile.Foto
                        }]
                    };
                }).ToList() ?? []);
            }
            return conversaciones;
        }

        public Conversacion? FindConversacion(string? identity)
        {
            UserModel user = AuthNetCore.User(identity);
            List<Conversacion_usuarios> conversaciones_usuarios = new Conversacion_usuarios()
                .Where<Conversacion_usuarios>( FilterData.In("Id_usuario", Conversacion_usuarios.Select(cu => cu.Id_usuario).ToArray()));

            List<Conversacion> conversaciones = new Conversacion { }.Where<Conversacion>(
                FilterData.In("id_conversacion", conversaciones_usuarios.Select(cu => cu.Id_conversacion).ToArray())
            );
            Conversacion? conversacion = conversaciones.Find(c => c.Conversacion_usuarios?.Find(cu => cu.Id_usuario == user.UserId) != null);
            if (conversacion != null)
            {
                return conversacion;
            }

            //var profile = new CAPA_NEGOCIO.Security_Users {Id_User = Conversacion_usuarios?[0].Id_usuario}.Get_Profile();


            Conversacion_usuarios?.Add(new Conversacion_usuarios
            {
                Id_usuario = user.UserId
            });

            Conversacion_usuarios?.ForEach(c =>
            {
                var profile = new Security_Users { Id_User = c.Id_usuario }.Find<Security_Users>()?.Get_Profile();
                c.Avatar = profile?.Foto;
                c.Name = profile?.GetNombreCompleto();
            });
            Descripcion = $"Conversacion-{Guid.NewGuid()}";

            return (Conversacion?)Save() ?? this;
        }

        public object? SaveConversacion(string? identity)
        {
            UserModel user = AuthNetCore.User(identity);
            if (Conversacion_usuarios == null || Conversacion_usuarios.Count == 0)
            {
                throw new Exception("no se puede crear una conversacion sin participantes");
            }
            //EN ESTE CASO SE REALIZA DE ESTA FORMA PARA MANEJAR UNA CONVERSACION POR PAREJA DE USUARIOS
            var conversacionesExistentes = GetConversaciones(identity);
            var conversacion = conversacionesExistentes
                .Where(c => UsuarioIncluidoEnConversacion(c?.Conversacion_usuarios))
                .FirstOrDefault();
            if (conversacion != null)
            {
                return conversacion;
            }
            Conversacion_usuarios.Add(new Conversacion_usuarios { Id_usuario = user.UserId });
            return Save();
        }

        private bool UsuarioIncluidoEnConversacion(List<Conversacion_usuarios>? Conversacion_usuarios)
        {
            return Conversacion_usuarios?.Find(cu => cu.Id_usuario == Conversacion_usuarios.First().Id_usuario) != null;
        }
    }
}
