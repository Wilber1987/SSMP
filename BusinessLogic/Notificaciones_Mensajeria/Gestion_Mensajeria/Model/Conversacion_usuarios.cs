using API.Controllers;
using APPCORE;
using CAPA_NEGOCIO;
using CAPA_NEGOCIO.MAPEO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace DataBaseModel
{
    public class Conversacion_usuarios : EntityClass
    {
        [PrimaryKey(Identity = false)]
        public int? Id_conversacion { get; set; }
        [PrimaryKey(Identity = false)]
        public int? Id_usuario { get; set; }
        public string? Name { get; set; }
        public string? Avatar { get; set; }

        [ManyToOne(TableName = "Conversacion", KeyColumn = "Id_conversacion", ForeignKeyColumn = "Id_conversacion")]
        public Conversacion? Conversacion { get; set; }
        [ManyToOne(TableName = "Security_Users", KeyColumn = "Id_User", ForeignKeyColumn = "id_usuario")]
        public Security_Users? Security_Users { get; set; }
    }
}
