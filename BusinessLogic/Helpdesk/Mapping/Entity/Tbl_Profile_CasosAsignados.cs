using APPCORE;

namespace CAPA_NEGOCIO.MAPEO
{
    public class Tbl_Profile_CasosAsignados : EntityClass
	{
		[PrimaryKey(Identity = false)]
		public int? Id_Perfil { get; set; }

		[PrimaryKey(Identity = false)]
		public int? Id_Case { get; set; }
		public DateTime? Fecha { get; set; }
		public int? Id_Tipo_Participacion { get; set; }

		[ManyToOne(TableName = "Tbl_Profile", KeyColumn = "Id_Perfil", ForeignKeyColumn = "Id_Perfil")]
		public Tbl_Profile? Tbl_Profile { get; set; }

		[ManyToOne(TableName = "Tbl_Case", KeyColumn = "Id_Case", ForeignKeyColumn = "Id_Case")]
		public Tbl_Case? Tbl_Case { get; set; }
		[ManyToOne(TableName = "Cat_Tipo_Participaciones", KeyColumn = "Id_Tipo_Participacion", ForeignKeyColumn = "Id_Tipo_Participacion")]
		public Cat_Tipo_Participaciones? Cat_Tipo_Participaciones { get; set; }
	}
}
