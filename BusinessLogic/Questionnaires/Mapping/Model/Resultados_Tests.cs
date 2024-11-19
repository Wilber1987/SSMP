using CAPA_DATOS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace DataBaseModel {
   public class Resultados_Tests : EntityClass {
       [PrimaryKey(Identity = true)]
       public int? Id_Resultado { get; set; }
       public int? Id_Perfil { get; set; }
       public int? Id_test { get; set; }
       public DateTime? Fecha { get; set; }
       public string? Seccion { get; set; }
       public DateTime? Created_at { get; set; }
       public DateTime? Updated_at { get; set; }
       public string? Valor { get; set; }
       public string? Categoria_valor { get; set; }
       public string? Tipo { get; set; }
       [ManyToOne(TableName = "Tests", KeyColumn = "Id_test", ForeignKeyColumn = "Id_test")]
       public Tests? Tests { get; set; }
       [OneToMany(TableName = "Resultados_Pregunta_Tests", KeyColumn = "Id_Resultado", ForeignKeyColumn = "Id_Resultado")]
       public List<Resultados_Pregunta_Tests>? Resultados_Pregunta_Tests { get; set; }
   }
}
