using System;
using APPCORE;

namespace BusinessLogic.SystemDocuments.Models
{
	public class Article : EntityClass
	{
		[PrimaryKey(Identity = true)]
		public int? Article_Id { get; set; }
		public string? Title { get; set; }
		public string? Author { get; set; }		
		public int? Id_User { get; set; }
		public string? Body { get; set; }
		public DateTime? Publish_Date { get; set; }
		public DateTime? Update_Date { get; set; }
		public bool? Status { get; set; }
		// Foreign key
		public int? Category_Id { get; set; }
		// Navigation property
		[ManyToOne(TableName = "Category", KeyColumn = "Category_Id", ForeignKeyColumn = "Category_Id")]
		public Category? Category { get; set; }
	}
}