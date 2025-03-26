using APPCORE;

namespace CAPA_NEGOCIO.MAPEO
{
    public class Cat_Paises : EntityClass
	{
		[PrimaryKey(Identity = true)]
		public int? Id_Pais { get; set; }
		public string? Estado { get; set; }
		public string? Descripcion { get; set; }
	}
}
