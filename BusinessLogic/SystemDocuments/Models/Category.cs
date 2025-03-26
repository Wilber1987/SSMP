using System.Collections.Generic;
using APPCORE;

namespace BusinessLogic.SystemDocuments.Models
{
	public class Category: EntityClass
	{
		[PrimaryKey(Identity = true)]
		public int? Category_Id { get; set; }
		public string? Descripcion { get; set; }
	}
}