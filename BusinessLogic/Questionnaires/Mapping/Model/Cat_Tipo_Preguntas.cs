using CAPA_DATOS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace DataBaseModel {
   public class Cat_Tipo_Preguntas : EntityClass {
       [PrimaryKey(Identity = true)]
       public int? Id_tipo_pregunta { get; set; }
       public string? Tipo_pregunta { get; set; }
       public string? Descripcion { get; set; }
       public string? Estado { get; set; }
       public DateTime? Fecha_crea { get; set; }
       public DateTime? Created_at { get; set; }
       public DateTime? Updated_at { get; set; }
       public bool? Editable { get; set; }
       public string? Descripcion_general { get; set; }
       [OneToMany(TableName = "Cat_Valor_Preguntas", KeyColumn = "Id_tipo_pregunta", ForeignKeyColumn = "Id_tipo_pregunta")]
       public List<Cat_Valor_Preguntas>? Cat_Valor_Preguntas { get; set; }
       [OneToMany(TableName = "Pregunta_Tests", KeyColumn = "Id_tipo_pregunta", ForeignKeyColumn = "Id_tipo_pregunta")]
       public List<Pregunta_Tests>? Pregunta_Tests { get; set; }
       
   }
}
