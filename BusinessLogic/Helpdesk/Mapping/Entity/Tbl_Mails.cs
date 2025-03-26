using APPCORE;
using APPCORE.Services;
using MimeKit;

namespace CAPA_NEGOCIO.MAPEO
{
	public class Tbl_Mails : EntityClass
	{
		public Tbl_Mails() { }
		public Tbl_Mails(MimeMessage mail)
		{
			Subject = mail.Subject;
			MessageID = mail.MessageId;
			Sender = mail.Sender?.Address;
			FromAdress = mail.From.ToString();
			ReplyTo = mail.ReplyTo?.Select(r => r.ToString()).ToList();
			Bcc = mail.Bcc?.Select(r => r.ToString()).ToList();
			Cc = mail.Cc?.Select(r => r.ToString()).ToList();
			ToAdress = mail.To?.Select(r => r.ToString()).ToList();
			Date = mail.Date.DateTime;
			Uid = mail.MessageId;
			Body = this.Body ?? mail.HtmlBody;
			Estado = MailState.RECIBIDO.ToString();
			Flags = Flags?.ToString();
		}
		[PrimaryKey(Identity = true)]
		public int? Id_Mail { get; set; }
		public string? Subject { get; set; }
		public int? Id_Case { get; set; }
		public string? MessageID { get; set; }
		public string? Estado { get; set; }
		public string? Sender { get; set; }
		public string? Body { get; set; }
		public string? Type { get; set; }//NOTIFICATION SERVICES ENUM
		public string? FromAdress { get; set; }
		[JsonProp]
		public List<String>? ReplyTo { get; set; }
		[JsonProp]
		public List<String>? Bcc { get; set; }
		[JsonProp]
		public List<String>? Cc { get; set; }
		[JsonProp]
		public List<String>? ToAdress { get; set; }
		[JsonProp]
		public List<ModelFiles>? Attach_Files { get; set; }
		//public int? Size { get; set; }
		public String? Flags { get; set; }
		//public string[] RawFlags { get; set; }
		public DateTime? Date { get; set; }
		public string? Uid { get; set; }
		// [OneToOne(TableName = "Tbl_Comments", KeyColumn = "Id_Mail", ForeignKeyColumn = "Id_Mail")]
		// public Tbl_Comments? Tbl_Comments  { get; set; }
		
	}	
	public enum MailState
	{
		ENVIADO, PENDIENTE, RECIBIDO
	}
}
