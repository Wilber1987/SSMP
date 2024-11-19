using CAPA_DATOS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace DataBaseModel {
   public class Cat_Valor_Preguntas : EntityClass {
       [PrimaryKey(Identity = true)]
       public int? Id_valor_pregunta { get; set; }
       public string? Descripcion { get; set; }
       public int? Id_tipo_pregunta { get; set; }
       public string? Estado { get; set; }
       public int? Valor { get; set; }
       public DateTime? Created_at { get; set; }
       public DateTime? Updated_at { get; set; }
       [ManyToOne(TableName = "Cat_Tipo_Preguntas", KeyColumn = "Id_tipo_pregunta", ForeignKeyColumn = "Id_tipo_pregunta")]
       public Cat_Tipo_Preguntas? Cat_Tipo_Preguntas { get; set; }
       [OneToMany(TableName = "Resultados_Pregunta_Tests", KeyColumn = "Id_valor_pregunta", ForeignKeyColumn = "Id_valor_pregunta")]
       public List<Resultados_Pregunta_Tests>? Resultados_Pregunta_Tests { get; set; }
   }
}
