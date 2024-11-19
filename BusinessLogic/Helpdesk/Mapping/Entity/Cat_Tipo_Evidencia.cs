using CAPA_DATOS;
using CAPA_NEGOCIO.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAPA_NEGOCIO.MAPEO
{

    public class Cat_Tipo_Evidencia : EntityClass
	{
		[PrimaryKey(Identity = true)]
		public int? IdTipo { get; set; }
		public string? Descripcion { get; set; }
		public string? Estado { get; set; }

	}
}
