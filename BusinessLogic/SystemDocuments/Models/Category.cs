using System.Collections.Generic;
using CAPA_DATOS;

namespace BusinessLogic.SystemDocuments.Models
{
	public class Category: EntityClass
	{
		[PrimaryKey(Identity = true)]
		public int? Category_Id { get; set; }
		public string? Descripcion { get; set; }
	}
}