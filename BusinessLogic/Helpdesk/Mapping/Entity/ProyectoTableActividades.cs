using CAPA_DATOS;
using API.Controllers;
using CAPA_DATOS.Services;
using MimeKit;

namespace CAPA_NEGOCIO.MAPEO
{

	public enum CommetsState
	{
		Leido, Pendiente
	}
	public class Tbl_Comments : EntityClass
	{
		[PrimaryKey(Identity = true)]
		public int? Id_Comentario { get; set; }
		public string? Estado { get; set; }
		public string? NickName { get; set; }
		public string? Mail { get; set; }
		public string? Foto { get; set; }

		public string? Body { get; set; }
		[JsonProp]
		public List<ModelFiles>? Attach_Files { get; set; }
		[JsonProp]
		public List<string>? Mails { get; set; }
		public int? Id_Case { get; set; }
		public int? Id_User { get; set; }
		public DateTime? Fecha { get; set; }

		public object? SaveComment(string identity, Boolean withMail = true)
		{
			try
			{
				BeginGlobalTransaction();
				UserModel user = AuthNetCore.User(identity);
				var profile = Tbl_Profile.Get_Profile(user);
				Fecha = DateTime.Now;
				Id_User = user.UserId;
				NickName = user.UserData?.Nombres;
				Mail = user.mail;
				Foto = profile.Foto;
				Tbl_Case? Tbl_Case = new Tbl_Case() { Id_Case = Id_Case }.Find<Tbl_Case>();
				foreach (var file in Attach_Files ?? new List<ModelFiles>())
				{
					ModelFiles Response = (ModelFiles)FileService.upload("Attach\\", file).body;
					file.Value = Response.Value;
					file.Type = Response.Type;
				}
				Save();
				if (withMail)
				{
					CreateMailForComment(user, Tbl_Case);
				}
				CommitGlobalTransaction();
				return this;
			}
			catch (System.Exception)
			{
				RollBackGlobalTransaction();
				throw;
			}

		}

		public void CreateMailForComment(UserModel user, Tbl_Case Tbl_Case)
		{
			List<String?>? toMails = new List<string?>();
			Tbl_Case? Tbl_CaseWithComments = new Tbl_Case() { Id_Case = Id_Case }.Find<Tbl_Case>();
			if (Mails?.Count == 0 && Tbl_CaseWithComments?.Tbl_Comments != null)
			{
				Mails = new List<string>();
				Mails?.AddRange(Tbl_CaseWithComments?.Tbl_Comments?.Select(c => c.Mail?.ToString()).ToList());

			}
			if (Mails != null)
			{
				toMails.AddRange(Mails);
			}
			toMails.Add(Tbl_Case?.Mail);
			new Tbl_Mails()
			{
				Id_Case = Tbl_Case?.Id_Case,
				Subject = $"RE: " + Tbl_Case?.Titulo?.ToUpper(),
				Body = Body,
				FromAdress = user.mail,
				Estado = MailState.PENDIENTE.ToString(),
				Date = DateTime.Now,
				Attach_Files = Attach_Files,
				Uid = Tbl_Case?.MimeMessageCaseData?.MessageId,
				ToAdress = toMails.Where(m => m != null && m != user.mail).ToList().Distinct().ToList()
			}.Save();
		}

		public List<Tbl_Comments> GetComments()
		{
			Tbl_Case Tbl_Case = new Tbl_Case() { Id_Case = Id_Case }.Find<Tbl_Case>();
			if (Tbl_Case?.Id_Vinculate != null)
			{
				return new Tbl_Comments().Where<Tbl_Comments>(
					FilterData.In(
						"Id_Case",
						new Tbl_Case() { Id_Vinculate = Tbl_Case.Id_Vinculate }
					.Get<Tbl_Case>().Select(c => c.Id_Case.ToString()).ToArray()
					));
			}
			else
			{
				return Get<Tbl_Comments>();
			}

		}
		public List<Tbl_Comments> GetOwComments(List<Tbl_Case> caseTables)
		{
			return new Tbl_Comments().Where<Tbl_Comments>(
			 FilterData.In(	"Id_Case", caseTables.Select(c => c.Id_Case.ToString()).ToArray()));

		}
	}

	public class Tbl_Comments_Tasks : Tbl_Comments
	{
		public int? Id_Tarea { get; set; }
		public List<Tbl_Comments_Tasks> GetOwComments(List<Tbl_Tareas> caseTables)
		{
			return new Tbl_Comments_Tasks().Where<Tbl_Comments_Tasks>(
				FilterData.In("Id_Tarea", caseTables.Select(c => c.Id_Tarea.ToString()).ToArray()));
		}
		public new List<Tbl_Comments_Tasks> GetComments()
		{
			return Get<Tbl_Comments_Tasks>();
		}
		public new object? SaveComment(string identity, Boolean withMail = true)
		{
			try
			{
				BeginGlobalTransaction();
				UserModel user = AuthNetCore.User(identity);
				var profile = Tbl_Profile.Get_Profile(user);
				Fecha = DateTime.Now;
				Id_User = user.UserId;
				NickName = user.UserData?.Nombres;
				Foto = profile.Foto;
				Mail = user.mail;
				Tbl_Tareas? Tbl_Tareas = new Tbl_Tareas() { Id_Tarea = Id_Tarea }.Find<Tbl_Tareas>();
				Tbl_Case? Tbl_Case = new Tbl_Case() { Id_Case = Tbl_Tareas.Id_Case }.Find<Tbl_Case>();
				foreach (var file in Attach_Files ?? new List<ModelFiles>())
				{
					ModelFiles Response = (ModelFiles)FileService.upload("Attach\\", file).body;
					file.Value = Response.Value;
					file.Type = Response.Type;
				}
				Save();
				if (withMail)
				{
					CreateMailForComment(user, Tbl_Case);
				}
				CommitGlobalTransaction();
				return this;
			}
			catch (System.Exception)
			{
				RollBackGlobalTransaction();
				throw;
			}

		}
	}

	public enum Case_Estate
	{
		Solicitado, Pendiente, Activo, Finalizado, Espera, Rechazado, Vinculado
	}

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
	public class Cat_Cargos_Dependencias : EntityClass
	{
		[PrimaryKey(Identity = true)]
		public int? Id_Cargo { get; set; }
		public string? Descripcion { get; set; }
		[OneToMany(TableName = "Tbl_Dependencias_Usuarios", KeyColumn = "Id_Cargo", ForeignKeyColumn = "Id_Cargo")]
		public List<Tbl_Dependencias_Usuarios>? Tbl_Dependencias_Usuarios { get; set; }
	}
	public class Cat_Tipo_Participaciones : EntityClass
	{
		[PrimaryKey(Identity = true)]
		public int? Id_Tipo_Participacion { get; set; }
		public string? Descripcion { get; set; }
		[OneToMany(TableName = "Tbl_Participantes", KeyColumn = "Id_Tipo_Participacion", ForeignKeyColumn = "Id_Tipo_Participacion")]
		public List<Tbl_Participantes>? Tbl_Participantes { get; set; }
	}
	public class Tbl_Agenda : EntityClass
	{
		[PrimaryKey(Identity = true)]
		public int? IdAgenda { get; set; }
		public int? Id_Perfil { get; set; }
		public int? Id_Dependencia { get; set; }
		public string? Dia { get; set; }
		public string? Hora_Inicial { get; set; }
		public string? Hora_Final { get; set; }
		public DateTime? Fecha_Caducidad { get; set; }		
	}
	public class Tbl_Dependencias_Usuarios : EntityClass
	{
		[PrimaryKey(Identity = false)]
		public int? Id_Perfil { get; set; }
		[PrimaryKey(Identity = false)]
		public int? Id_Dependencia { get; set; }
		public int? Id_Cargo { get; set; }
		[ManyToOne(TableName = "Tbl_Profile", KeyColumn = "Id_Perfil", ForeignKeyColumn = "Id_Perfil")]
		public Tbl_Profile? Tbl_Profile { get; set; }
		[ManyToOne(TableName = "Cat_Dependencias", KeyColumn = "Id_Dependencia", ForeignKeyColumn = "Id_Dependencia")]
		public Cat_Dependencias? Cat_Dependencias { get; set; }
		[ManyToOne(TableName = "Cat_Cargos_Dependencias", KeyColumn = "Id_Cargo", ForeignKeyColumn = "Id_Cargo")]
		public Cat_Cargos_Dependencias? Cat_Cargos_Dependencias { get; set; }
	}
	public class Tbl_Evidencias : EntityClass
	{
		[PrimaryKey(Identity = true)]
		public int? IdEvidencia { get; set; }
		public int? IdTipo { get; set; }
		public string? Data { get; set; }
		public int? Id_Tarea { get; set; }
		[ManyToOne(TableName = "Tbl_Tareas", KeyColumn = "Id_Tarea", ForeignKeyColumn = "Id_Tarea")]
		public Tbl_Tareas? Tbl_Tareas { get; set; }
		[ManyToOne(TableName = "Cat_Tipo_Evidencia", KeyColumn = "IdTipo", ForeignKeyColumn = "IdTipo")]
		public Cat_Tipo_Evidencia? Cat_Tipo_Evidencia { get; set; }

	}


	public class Tbl_Calendario : EntityClass
	{
		[PrimaryKey(Identity = true)]
		public int? IdCalendario { get; set; }
		public int? Id_Tarea { get; set; }
		public int? Id_Dependencia { get; set; }
		public string? Estado { get; set; }
		public DateTime? Fecha_Inicio { get; set; }
		public DateTime? Fecha_Final { get; set; }
		[ManyToOne(TableName = "Tbl_Tareas", KeyColumn = "Id_Tarea", ForeignKeyColumn = "Id_Tarea")]
		public Tbl_Tareas? Tbl_Tareas { get; set; }
		[ManyToOne(TableName = "Cat_Dependencias", KeyColumn = "Id_Dependencia", ForeignKeyColumn = "Id_Dependencia")]
		public Cat_Dependencias? Cat_Dependencias { get; set; }

	}
	public class Tbl_Tareas : EntityClass
	{
		[PrimaryKey(Identity = true)]
		public int? Id_Tarea { get; set; }
		public string? Titulo { get; set; }
		public int? Id_TareaPadre { get; set; }
		public int? Id_Case { get; set; }
		public string? Descripcion { get; set; }
		public DateTime? Fecha_Inicio { get; set; }
		public DateTime? Fecha_Finalizacion { get; set; }
		public string? Estado { get; set; }
		[ManyToOne(TableName = "Tbl_Tareas", KeyColumn = "Id_Tarea", ForeignKeyColumn = "Id_TareaPadre")]
		public Tbl_Tareas? Tbl_Tarea { get; set; }
		[ManyToOne(TableName = "Tbl_Case", KeyColumn = "Id_Case", ForeignKeyColumn = "Id_Case")]
		public Tbl_Case? Tbl_Case { get; set; }
		[OneToMany(TableName = "Tbl_Calendario", KeyColumn = "Id_Tarea", ForeignKeyColumn = "Id_Tarea")]
		public List<Tbl_Calendario>? Tbl_Calendario { get; set; }
		[OneToMany(TableName = "Tbl_Evidencias", KeyColumn = "Id_Tarea", ForeignKeyColumn = "Id_Tarea")]
		public List<Tbl_Evidencias>? Tbl_Evidencias { get; set; }
		[OneToMany(TableName = "Tbl_Participantes", KeyColumn = "Id_Tarea", ForeignKeyColumn = "Id_Tarea")]
		public List<Tbl_Participantes>? Tbl_Participantes { get; set; }
		[OneToMany(TableName = "Tbl_Tareas", KeyColumn = "Id_Tarea", ForeignKeyColumn = "Id_TareaPadre")]
		public List<Tbl_Tareas>? Tbl_TareasHijas { get; set; }
		public DateTime? Fecha_Finalizacion_Proceso { get; private set; }
		public DateTime? Fecha_Inicio_Proceso { get; private set; }

		public List<Tbl_Tareas> GetOwParticipations(string identity)
		{
			Tbl_Profile? profile = new Tbl_Profile() { IdUser = AuthNetCore.User(identity).UserId }.Find<Tbl_Profile>();
			Tbl_Participantes Inst = new Tbl_Participantes() { Id_Perfil = profile?.Id_Perfil };
			return Where<Tbl_Tareas>(
				FilterData.In("Id_Tarea", Inst.Get<Tbl_Participantes>().Select(p => p.Id_Tarea.ToString()).ToArray())
			);
		}
		public List<Tbl_Tareas> GetOwActiveParticipations(string identity)
		{
			Tbl_Profile? profile = new Tbl_Profile() { IdUser = AuthNetCore.User(identity).UserId }.Find<Tbl_Profile>();
			Tbl_Participantes Inst = new Tbl_Participantes { Id_Perfil = profile?.Id_Perfil };
			return Where<Tbl_Tareas>(
				FilterData.NotIn("Estado", TareasState.Finalizado.ToString(), TareasState.Inactivo.ToString()),
				FilterData.In("Id_Tarea", Inst.Get<Tbl_Participantes>().Select(p => p.Id_Tarea.ToString()).ToArray())
			);
		}

		public object? UpdateTarea()
		{
			//"Activo", "Proceso", "Finalizado", "Espera", "Inactivo"
			if (Estado == TareasState.Proceso.ToString() && Fecha_Inicio == null)
			{
				Fecha_Inicio_Proceso = DateTime.Now;
			}
			if (Estado == TareasState.Finalizado.ToString())
			{
				if (Fecha_Inicio_Proceso == null)
				{
					Fecha_Inicio_Proceso = DateTime.Now;
				}
				Fecha_Finalizacion_Proceso = DateTime.Now;
			}
			return Update();
		}

		public void NotificarTecnicos(Tbl_Case Tbl_Case, UserModel user)
		{
			Tbl_Participantes?.ForEach(participante =>
			{
				List<String?>? toMails = new()
				{
					participante?.Tbl_Profile?.Correo_institucional
				};
				new Tbl_Mails()
				{
					Id_Case = Tbl_Case?.Id_Case,
					Subject = $"TAREA ASIGNADA: - {Titulo} ",
					Body = $"TAREA ASIGNADA: {Titulo} - ROL: {participante.Cat_Tipo_Participaciones.Descripcion} - CASO:  {Tbl_Case?.Titulo?.ToUpper()}" + Descripcion,
					FromAdress = user.mail,
					Estado = MailState.PENDIENTE.ToString(),
					Date = DateTime.Now,
					Attach_Files = null,
					Uid = Tbl_Case?.MimeMessageCaseData?.MessageId,
					ToAdress = toMails?.Where(m => m != null && m != user.mail).ToList()?.Distinct()?.ToList()
				}.Save();
			});
		}
		public object? SaveTareaWithTransaction(string identity)
		{
			try
			{
				BeginGlobalTransaction();
				var response = SaveTarea(identity);
				CommitGlobalTransaction();
				return response;
			}
			catch (System.Exception)
			{
				RollBackGlobalTransaction();
				throw;
			}
		}


		public object? SaveTarea(string identity)
		{
			try
			{
				UserModel user = AuthNetCore.User(identity) ?? new();
				//BeginGlobalTransaction();
				List<DateTime?>? fechasIniciales = this.Tbl_Calendario?.Select(c => c.Fecha_Inicio).ToList();
				List<DateTime?>? fechasFinales = this.Tbl_Calendario?.Select(c => c.Fecha_Inicio).ToList();
				Fecha_Inicio = fechasIniciales?.Min() ?? DateTime.Now;
				Fecha_Finalizacion = fechasFinales?.Max() ?? DateTime.Now;
				var comment = new Tbl_Comments()
				{
					Id_Case = this.Id_Case,
					Body = $"Se a creado una nueva tarea: {this.Descripcion}",
					NickName = $"{user.UserData?.Nombres} ({user?.mail})",
					Fecha = DateTime.Now,
					Estado = CommetsState.Pendiente.ToString(),
					Mail = user?.mail
				};
				comment.Save();
				Tbl_Case = new Tbl_Case() { Id_Case = this.Id_Case }.Find<Tbl_Case>();
				comment.CreateMailForComment(user, Tbl_Case);
				var response = this.Save();
				//CommitGlobalTransaction();
				return response;
			}
			catch (System.Exception)
			{
				//RollBackGlobalTransaction();
				throw;
			}

		}
	}
	enum TareasState
	{
		Activo, Proceso, Finalizado, Espera, Inactivo
	}
	public class Tbl_Participantes : EntityClass
	{
		[PrimaryKey(Identity = false)]
		public int? Id_Perfil { get; set; }
		[PrimaryKey(Identity = false)]
		public int? Id_Tarea { get; set; }
		public int? Id_Tipo_Participacion { get; set; }
		[ManyToOne(TableName = "Tbl_Profile", KeyColumn = "Id_Perfil", ForeignKeyColumn = "Id_Perfil")]
		public Tbl_Profile? Tbl_Profile { get; set; }
		[ManyToOne(TableName = "Tbl_Tareas", KeyColumn = "Id_Tarea", ForeignKeyColumn = "Id_Tarea")]
		public Tbl_Tareas? Tbl_Tareas { get; set; }
		[ManyToOne(TableName = "Cat_Tipo_Participaciones", KeyColumn = "Id_Tipo_Participacion", ForeignKeyColumn = "Id_Tipo_Participacion")]
		public Cat_Tipo_Participaciones? Cat_Tipo_Participaciones { get; set; }
	}
}
