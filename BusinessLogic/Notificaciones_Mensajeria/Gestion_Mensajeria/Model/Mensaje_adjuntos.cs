using APPCORE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace DataBaseModel {
   public class Mensaje_adjuntos : EntityClass {
       [PrimaryKey(Identity = false)]
       public int? Id { get; set; }
       public int? Mensaje_id { get; set; }
       public string? Archivo { get; set; }
       public DateTime? Created_at { get; set; }
       public DateTime? Updated_at { get; set; }
   }
}
