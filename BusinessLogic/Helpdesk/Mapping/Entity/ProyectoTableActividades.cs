using APPCORE;
using API.Controllers;

namespace CAPA_NEGOCIO.MAPEO
{

	public enum CommetsState
	{
		Leido, Pendiente
	}

	public enum Case_Estate
	{
		Solicitado, Pendiente, Activo, Finalizado, Espera, Rechazado, Vinculado
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
