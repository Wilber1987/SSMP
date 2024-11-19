using CAPA_DATOS;

namespace CAPA_NEGOCIO.MAPEO
{
    public class Cat_Tipo_Servicio : EntityClass
	{
		[PrimaryKey(Identity = true)]
		public int? Id_Tipo_Servicio { get; set; }
		public string? Descripcion { get; set; }
		public string? Estado { get; set; }
		public string? Icon { get; set; }
		//[OneToMany(TableName = "Tbl_Servicios", KeyColumn = "Id_Tipo_Servicio", ForeignKeyColumn = "Id_Tipo_Servicio")]
		public List<Tbl_Servicios>? Tbl_Servicios { get; set; }
	}
}
