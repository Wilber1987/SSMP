using CAPA_DATOS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace DataBaseModel {
   public class Resultados_Pregunta_Tests : EntityClass {
       [PrimaryKey(Identity = false)]
       public int? Id_Perfil { get; set; }
       [PrimaryKey(Identity = false)]
       public int? Id_pregunta_test { get; set; }
       public int? Id_Resultado { get; set; }
       public int? Resultado { get; set; }
       public string? Respuesta { get; set; }
       public string? Estado { get; set; }
       public int? Id_valor_pregunta { get; set; }
       public DateTime? Created_at { get; set; }
       public DateTime? Updated_at { get; set; }
       [PrimaryKey(Identity = false)]
       public DateTime? Fecha { get; set; }
       [ManyToOne(TableName = "Cat_Valor_Preguntas", KeyColumn = "Id_valor_pregunta", ForeignKeyColumn = "Id_valor_pregunta")]
       public Cat_Valor_Preguntas? Cat_Valor_Preguntas { get; set; }
       [ManyToOne(TableName = "Pregunta_Tests", KeyColumn = "Id_pregunta_test", ForeignKeyColumn = "Id_pregunta_test")]
       public Pregunta_Tests? Pregunta_Tests { get; set; }
   }
}
