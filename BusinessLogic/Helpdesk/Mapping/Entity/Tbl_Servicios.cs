using CAPA_DATOS;

namespace CAPA_NEGOCIO.MAPEO
{
    public class Tbl_Servicios : EntityClass
	{
		[PrimaryKey(Identity = true)]
		public int? Id_Servicio { get; set; }
		public string? Nombre_Proyecto { get; set; }
		public string? Descripcion_Servicio { get; set; }
		public int? Id_Tipo_Servicio { get; set; }
		public string? Visibilidad { get; set; }
		public string? Estado { get; set; }
		public DateTime? Fecha_Inicio { get; set; }
		public int? Id_Dependencia { get; set; }
		public DateTime? Fecha_Finalizacion { get; set; }
		[ManyToOne(TableName = "Cat_Tipo_Servicio", KeyColumn = "Id_Tipo_Servicio", ForeignKeyColumn = "Id_Tipo_Servicio")]
		public Cat_Tipo_Servicio? Cat_Tipo_Servicio { get; set; }		

	}
}
