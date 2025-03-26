using APPCORE;
using APPCORE.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace DataBaseModel {
   public class Cat_Categorias_Test : EntityClass {
       [PrimaryKey(Identity = true)]
       public int? Id_categoria { get; set; }
       public string? Descripcion { get; set; }
       public string? Imagen { get; set; }
       public string? Estado { get; set; }
       public DateTime? Created_at { get; set; }
       public DateTime? Updated_at { get; set; }
       [OneToMany(TableName = "Tests", KeyColumn = "Id_categoria", ForeignKeyColumn = "Id_categoria")]
       public List<Tests>? Tests { get; set; }
       
        public object? SaveCategoria()
        {
            Imagen = FileService.setImage(Imagen);
            return Save();
        }

        public object? UpdateCategoria()
        {
            Imagen = FileService.setImage(Imagen);
            return Update();
        }
   }
}
