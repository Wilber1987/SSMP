using CAPA_DATOS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace DataBaseModel {
   public class Pregunta_Tests : EntityClass {
       [PrimaryKey(Identity = true)]
       public int? Id_pregunta_test { get; set; }
       public string? Estado { get; set; }
       public string? Descripcion_pregunta { get; set; }
       public int? Id_test { get; set; }
       public int? Id_tipo_pregunta { get; set; }
       public DateTime? Fecha { get; set; }
       public DateTime? Created_at { get; set; }
       public DateTime? Updated_at { get; set; }
       public string? Seccion { get; set; }
       public string? Descripcion_general { get; set; }
       [ManyToOne(TableName = "Cat_Tipo_Preguntas", KeyColumn = "Id_tipo_pregunta", ForeignKeyColumn = "Id_tipo_pregunta")]
       public Cat_Tipo_Preguntas? Cat_Tipo_Preguntas { get; set; }
       [ManyToOne(TableName = "Tests", KeyColumn = "Id_test", ForeignKeyColumn = "Id_test")]
       public Tests? Tests { get; set; }
       [OneToMany(TableName = "Resultados_Pregunta_Tests", KeyColumn = "Id_pregunta_test", ForeignKeyColumn = "Id_pregunta_test")]
       public List<Resultados_Pregunta_Tests>? Resultados_Pregunta_Tests { get; set; }
   }
}
