using CAPA_DATOS;
using CAPA_DATOS.Security;
using API.Controllers;
using CAPA_DATOS.Services;
using MimeKit;

namespace CAPA_NEGOCIO.MAPEO
{
	enum Case_Priority
	{
		Alta, Media, Baja
	}
	public class Tbl_Case : EntityClass
	{
		[PrimaryKey(Identity = true)]
		public int? Id_Case { get; set; }
		public string? Titulo { get; set; }
		public string? Descripcion { get; set; }
		public string? Case_Priority { get; set; }
		public string? Mail { get; set; }
		public int? Id_Perfil { get; set; }
		public string? Estado { get; set; }
		public int? Id_Dependencia { get; set; }
		public DateTime? Fecha_Inicio { get; set; }
		public DateTime? Fecha_Final { get; set; }
		public int? Id_Servicio { get; set; }
		public int? Id_Vinculate { get; set; }
		[JsonProp]
		public MimeMessageCaseData? MimeMessageCaseData { get; set; }
		// [ManyToOne(TableName = "Tbl_Profile", KeyColumn = "Id_Perfil", ForeignKeyColumn = "Id_Perfil")]
		public Tbl_Profile? Tbl_Profile { get; set; }
		[ManyToOne(TableName = "Cat_Dependencias", KeyColumn = "Id_Dependencia", ForeignKeyColumn = "Id_Dependencia")]
		public Cat_Dependencias? Cat_Dependencias { get; set; }
		[ManyToOne(TableName = "Tbl_Servicios", KeyColumn = "Id_Servicio", ForeignKeyColumn = "Id_Servicio")]
		public Tbl_Servicios? Tbl_Servicios { get; set; }
		[OneToMany(TableName = "Tbl_Tareas", KeyColumn = "Id_Case", ForeignKeyColumn = "Id_Case")]
		public List<Tbl_Tareas>? Tbl_Tareas { get; set; }
		//[OneToMany(TableName = "Tbl_Comments", KeyColumn = "Id_Case", ForeignKeyColumn = "Id_Case")]
		public List<Tbl_Comments>? Tbl_Comments { get; set; }
		public async Task<bool> CreateAutomaticCase(MimeMessage mail, Cat_Dependencias dependencia)
		{
			try
			{
				List<ModelFiles> Attach = new List<ModelFiles>();
				var remitente = (MailboxAddress?)mail.From.FirstOrDefault();


				Descripcion = mail?.HtmlBody;
				Titulo = mail?.Subject.ToUpper();
				Estado = Case_Estate.Solicitado.ToString();
				Fecha_Inicio = mail?.Date.DateTime;
				Id_Dependencia = dependencia.Id_Dependencia;
				Mail = remitente?.Address;

				Tbl_Profile? tbl_Profile = new Tbl_Profile { Correo_institucional = Mail }.Find<Tbl_Profile>();

				if (tbl_Profile == null)
				{
					tbl_Profile = new Tbl_Profile
					{
						Correo_institucional = remitente?.Address,
						Nombres = remitente?.Name,
						Apellidos = remitente?.Name,
						Estado = "ACTIVO",
						Foto = "\\Media\\profiles\\avatar.png",
						Sexo = "Masculino"
					};
					tbl_Profile.Save();
				}
				Tbl_Profile = tbl_Profile;

				MimeMessageCaseData = new MimeMessageCaseData
				{
					MessageId = mail.MessageId,
					InReplyTo = mail.References?.LastOrDefault()
				};
				RecoveryEmbebedCidImages(mail);
				BeginGlobalTransaction();
				if (mail.Subject.ToUpper().Contains("RE:"))
				{
					char[] MyChar = { 'R', 'E', ':', ' ' };
					Tbl_Case? findCase = new Tbl_Case()
					{
						Titulo = mail.Subject.ToUpper().TrimStart(MyChar)
					}.Find<Tbl_Case>();
					if (findCase != null)
					{
						if (mail?.Attachments != null)
						{
							foreach (MimeEntity attach in mail.Attachments)
							{
								ModelFiles Response = FileService.ReceiveFiles("Upload\\", attach);
								Attach.Add(Response);
							}
						}

						new Tbl_Mails(mail) { Id_Case = findCase.Id_Case, Attach_Files = Attach }.Save();
						saveMessage(mail, Attach, findCase);
					}
				}
				else if (mail.Subject.ToUpper().Contains("TAREA ASIGNADA:")) { }
				else
				{
					Save();
					new Tbl_Profile_CasosAsignados
					{
						Tbl_Profile = Tbl_Profile,
						Tbl_Case = this,
						Cat_Tipo_Participaciones = new Cat_Tipo_Participaciones { Descripcion = "Autor" }.Find<Cat_Tipo_Participaciones>()
					}.Save();
					if (mail?.Attachments != null)
					{
						foreach (MimeEntity attach in mail.Attachments)
						{
							ModelFiles Response = FileService.ReceiveFiles("Upload\\", attach);
							Attach.Add(Response);
						}
						new Tbl_Mails(mail) { Id_Case = this.Id_Case, Attach_Files = Attach }.Save();
						saveMessage(mail, Attach, this);
					}
					else
					{
						new Tbl_Mails(mail) { Id_Case = this.Id_Case, Body = Descripcion }.Save();
					}
				}
				CommitGlobalTransaction();
				return true;
			}
			catch (Exception ex)
			{
				Console.Write("error al guardar");
				RollBackGlobalTransaction();
				LoggerServices.AddMessageError($"error al crear el caso de: {mail.From}, {mail.Subject}", ex);
				return false;
			}

		}
		
		/*Automatico caso con IA*/
		public async Task<bool> CreateAutomaticCaseIA(UserMessage chat)
		{
			try
			{
                BeginGlobalTransaction();

                List<ModelFiles> Attach = new List<ModelFiles>();
 
				Tbl_Profile? tbl_Profile = new Tbl_Profile { Correo_institucional = chat.UserId }.Find<Tbl_Profile>();

			    if (tbl_Profile == null)
				{
					tbl_Profile = new Tbl_Profile
					{
						Correo_institucional = chat?.UserId,
						Nombres = chat?.UserId,
						Apellidos = chat?.UserId,
						Estado = "ACTIVO",
						Foto = "\\Media\\profiles\\avatar.png",
						Sexo = "Masculino"
					};
					tbl_Profile.Save();
				}
				Tbl_Profile = tbl_Profile; 

				//RecoveryEmbebedCidImages(chat);

				//guarda en base de dato el caso
                Save();
               
                CommitGlobalTransaction();
                return true;
			}
			catch (Exception ex)
			{
				Console.Write("error al guardar");
				RollBackGlobalTransaction();
				LoggerServices.AddMessageError($"error al crear el caso de: {chat.UserId}, {chat.Text}", ex);
				return false;
			}

		}


		private void RecoveryEmbebedCidImages(MimeMessage mail)
		{
			foreach (var part in mail.BodyParts)
			{
				if (part is MimePart mimePart)
				{
					if (mimePart.ContentId != null && mimePart.ContentType.MediaType.Equals("image"))
					{
						ModelFiles Response = FileService.ReceiveFiles("Upload\\", part);
						// Es una imagen embebida con formato CID
						var cid = mimePart.ContentId.TrimStart('<').TrimEnd('>');
						var src = $"cid:{cid}";
						Descripcion = Descripcion?.Replace(src, Response?.Value?.Replace("wwwroot", ""));
					}
				}
			}
		}
		private void saveMessage(MimeMessage mail, List<ModelFiles> Attach, Tbl_Case? findCase)
		{
			new Tbl_Comments()
			{
				Id_Case = findCase?.Id_Case,
				Body = Descripcion ?? mail.HtmlBody,
				NickName = $"{mail.From}",
				Fecha = mail.Date.DateTime,
				Estado = CommetsState.Pendiente.ToString(),
				Mail = mail.From.ToString(),
				Attach_Files = Attach

			}.Save();
		}
		public bool SolicitarActividades(string identity)
		{
			UserModel user = AuthNetCore.User(identity);
			Tbl_Profile? profile = new Tbl_Profile { IdUser = user.UserId }.Find<Tbl_Profile>();
			this.Id_Perfil = profile?.Id_Perfil;
			this.Estado = Case_Estate.Pendiente.ToString();
			this.Id_Case = (Int32?)SqlADOConexion.SQLM?.InsertObject(this);
			foreach (Tbl_Tareas obj in this.Tbl_Tareas ?? new List<Tbl_Tareas>())
			{
				obj.Id_Case = this.Id_Case;
				obj.Save();
			}
			return true;
		}
		public Tbl_Case GetActividad()
		{
			this.Tbl_Tareas = new Tbl_Tareas().Get<Tbl_Tareas>("Id_Case = " + this.Id_Case.ToString());
			foreach (Tbl_Tareas tarea in this.Tbl_Tareas ?? new List<Tbl_Tareas>())
			{
				tarea.Tbl_Calendario = new Tbl_Calendario().Get<Tbl_Calendario>("Id_Tarea = " + tarea.Id_Tarea.ToString());
			}
			return this;
		}
		public List<Tbl_Case> GetOwCase(string identity)
		{
			if (AuthNetCore.HavePermission(Permissions.ADMIN_ACCESS.ToString(), identity))
			{
				return Where<Tbl_Case>(FilterData.In("Estado", Case_Estate.Activo.ToString()));
			}
			if (AuthNetCore.HavePermission(Permissions.ADMINISTRAR_CASOS_DEPENDENCIA.ToString(), identity))
			{
				return getCaseByDependencia(identity, null)
				.Where(c => c.Estado != Case_Estate.Rechazado.ToString()
				&& c.Estado != Case_Estate.Solicitado.ToString()
				&& c.Estado != Case_Estate.Finalizado.ToString()).ToList();
			}
			if (AuthNetCore.HavePermission(Permissions.TECNICO_CASOS_DEPENDENCIA.ToString(), identity))
			{
				return getCaseByAsignacion(identity, null)
				.Where(c => c.Estado != Case_Estate.Rechazado.ToString()
				&& c.Estado != Case_Estate.Solicitado.ToString()
				&& c.Estado != Case_Estate.Finalizado.ToString()).ToList();
			}
			if (AuthNetCore.HavePermission(Permissions.ADMINISTRAR_CASOS_PROPIOS.ToString(), identity))
			{
				FilterData orFilter = GetCasosPropiosFilter(identity);
				return Where<Tbl_Case>(FilterData.In("Estado", Case_Estate.Activo.ToString()), orFilter);
			}

			throw new Exception("no tienes permisos para gestionar casos");
		}

		private static FilterData GetCasosPropiosFilter(string identity)
		{
			UserModel user = AuthNetCore.User(identity);
			Tbl_Profile? profile = new Tbl_Profile { IdUser = user.UserId }.Find<Tbl_Profile>();
			FilterData orFilter = FilterData.Or(
				FilterData.Equal("Id_Perfil", profile?.Id_Perfil)
			);

			if (profile?.Tbl_Grupos_Profiles?.Count > 0)
			{
				int?[] perfilesId = Tbl_Profile.GetProfilesByGroup(identity).Select(p => p.Id_Perfil).ToArray();
				orFilter.Filters?.Add(FilterData.In("Id_Perfil", perfilesId));
			}
			return orFilter;
		}

		public List<Tbl_Case> GetOwParticipantCase(string identity)
		{
			if (AuthNetCore.HavePermission(Permissions.ADMIN_ACCESS.ToString(), identity))
			{
				return GetOwCase(identity)
				.Where(c => c.Estado != Case_Estate.Rechazado.ToString()).ToList();
			}
			if (AuthNetCore.HavePermission(Permissions.ADMINISTRAR_CASOS_DEPENDENCIA.ToString(), identity))
			{
				return getCaseByDependencia(identity, null)
				.Where(c => c.Estado != Case_Estate.Rechazado.ToString()).ToList();
			}
			if (AuthNetCore.HavePermission(Permissions.ADMINISTRAR_CASOS_PROPIOS.ToString(), identity))
			{
				return GetOwCase(identity)
				.Where(c => c.Estado != Case_Estate.Rechazado.ToString()).ToList();
			}
			if (AuthNetCore.HavePermission(Permissions.TECNICO_CASOS_DEPENDENCIA.ToString(), identity))
			{
				return getCaseByAsignacion(identity, null)
				.Where(c => c.Estado != Case_Estate.Rechazado.ToString()).ToList();
			}
			throw new Exception("no tienes permisos para gestionar casos");
		}
		public List<Tbl_Case> GetOwCloseCase(string identity)
		{
			if (AuthNetCore.HavePermission(Permissions.ADMINISTRAR_CASOS_DEPENDENCIA.ToString(), identity))
			{
				return getCaseByDependencia(identity, null)
				.Where(c => c.Estado == Case_Estate.Finalizado.ToString()).ToList();
			}
			if (AuthNetCore.HavePermission(Permissions.TECNICO_CASOS_DEPENDENCIA.ToString(), identity))
			{
				return getCaseByAsignacion(identity, null)
				.Where(c => c.Estado == Case_Estate.Finalizado.ToString()).ToList();
			}
			throw new Exception("no tienes permisos para gestionar casos");
		}
		public List<Tbl_Case> GetOwSolicitudes(string? identity, Case_Estate case_Estate)
		{

			Id_Perfil = Tbl_Profile.GetUserProfile(identity)?.Id_Perfil;
			Estado = case_Estate.ToString();
			return Get<Tbl_Case>();
		}
		public List<Tbl_Case> GetSolicitudesPendientesAprobar(string? identity, Case_Estate case_Estate)
		{
			if (AuthNetCore.HavePermission(Permissions.ADMIN_ACCESS.ToString(), identity))
			{
				Estado = case_Estate.ToString();
				return Get<Tbl_Case>();
			}
			if (AuthNetCore.HavePermission(Permissions.ADMINISTRAR_CASOS_DEPENDENCIA.ToString(), identity))
			{
				return getCaseByDependencia(identity, case_Estate);
			}
			throw new Exception("no tienes permisos para aprobar casos");
		}
		private List<Tbl_Case> getCaseByDependencia(string? identity, Case_Estate? case_Estate)
		{
			Estado = case_Estate?.ToString();
			return Where<Tbl_Case>(
				FilterData.NotIn("Estado", Case_Estate.Vinculado),
				FilterData.In("Id_Dependencia",
					new Tbl_Dependencias_Usuarios() { Id_Perfil = Tbl_Profile.GetUserProfile(identity)?.Id_Perfil }
						.Get<Tbl_Dependencias_Usuarios>().Select(p => p.Id_Dependencia.ToString()).ToArray()
				)
			);
		}
		private List<Tbl_Case> getCaseByAsignacion(string? identity, Case_Estate? case_Estate)
		{
			UserModel user = AuthNetCore.User(identity);
			Tbl_Profile? profile = new Tbl_Profile { IdUser = user.UserId }.Find<Tbl_Profile>();
			var asignaciones = new Tbl_Profile_CasosAsignados() { Id_Perfil = profile?.Id_Perfil }.Get<Tbl_Profile_CasosAsignados>();
			Estado = case_Estate?.ToString();
			FilterData andFilter = FilterData.And(
				FilterData.NotIn("Estado", Case_Estate.Vinculado),
				FilterData.In("Id_Case", asignaciones.Select(p => p.Id_Case.ToString()).ToArray())
			);
			FilterData orFilter = GetCasosPropiosFilter(identity);
			return Where<Tbl_Case>(
				FilterData.Or(andFilter, orFilter)
			);
		}
		public object AprobarSolicitud(string identity)
		{

			var user = AuthNetCore.User(identity);
			if (!AuthNetCore.HavePermission(Permissions.ADMINISTRAR_CASOS_DEPENDENCIA.ToString(), identity))
			{
				throw new Exception("no tienes permisos para rechazar casos");
			}
			if (Estado.Equals(Case_Estate.Rechazado.ToString()))
			{
				throw new Exception("no puedes rechazar este caso en este proceso caso");
			}
			//BeginGlobalTransaction();
			Estado = Case_Estate.Activo.ToString();

			var response = Update();


			var comment = new Tbl_Comments()
			{
				Id_Case = this.Id_Case,
				Body = $"El caso esta en estado: {this.Estado}, para mayor información consulte con nuestro equipo",
				NickName = $"{user.UserData.Nombres} ({user.mail})",
				Fecha = DateTime.Now,
				Estado = CommetsState.Pendiente.ToString(),
				Mail = user.mail
			};
			Tbl_Tareas nuevaTarea = new Tbl_Tareas()
			{
				Titulo = "Ejecución y resolución del caso",
				Descripcion = $"Ejecución y resolución del caso: #{this.Id_Case}",
				Id_Case = this.Id_Case,
				Estado = TareasState.Proceso.ToString(),
				Tbl_Case = this
			};
			nuevaTarea.SaveTarea(identity);
			if (Tbl_Tareas != null)
			{
				foreach (var task in Tbl_Tareas)
				{
					task.NotificarTecnicos(this, user);
				}
			}
			CreateAsignationsByService(nuevaTarea);

			comment.Save();
			comment.CreateMailForComment(user, this);
			//CommitGlobalTransaction();
			return response;
		}
		public void CreateAsignationsByService(Tbl_Tareas? nuevaTarea)
		{
			if (this.Tbl_Servicios != null)
			{
				new Tbl_Profile_CasosAsignados { Id_Case = Id_Case }.Delete();
				new Tbl_Servicios_Profile { Id_Servicio = this.Tbl_Servicios?.Id_Servicio }.Get<Tbl_Servicios_Profile>().ForEach(sp =>
				{
					new Tbl_Profile_CasosAsignados
					{
						Id_Case = this.Id_Case,
						Id_Perfil = sp.Id_Perfil,
						Fecha = DateTime.Now
					}.Save();
					if (nuevaTarea != null)
					{
						new Tbl_Participantes
						{
							Id_Tarea = nuevaTarea.Id_Tarea,
							Id_Perfil = sp.Id_Perfil
						}.Save();
					}

				});
			}

		}
		public object RechazarSolicitud(string identity)
		{
			if (AuthNetCore.HavePermission(Permissions.ADMINISTRAR_CASOS_DEPENDENCIA.ToString(), identity))
			{
				Estado = Case_Estate.Rechazado.ToString();
				return Update() ?? new ResponseService()
				{
					status = 500,
					message = "error desconocido"
				};
			}
			throw new Exception("no tienes permisos para rechazar casos");
		}
		public object CerrarCaso(string identity)
		{
			if (AuthNetCore.HavePermission(Permissions.ADMINISTRAR_CASOS_DEPENDENCIA.ToString(), identity)
			|| AuthNetCore.HavePermission(Permissions.TECNICO_CASOS_DEPENDENCIA.ToString(), identity))
			{
				List<Tbl_Tareas> Tbl_Tareas =
				new Tbl_Tareas() { Id_Case = this.Id_Case }.Get<Tbl_Tareas>();
				int tareasRequeridas = Tbl_Tareas.Where(c => c.Estado != TareasState.Inactivo.ToString()).ToList().Count;
				int tareasFinalizadas = Tbl_Tareas.Where(c => c.Estado == TareasState.Finalizado.ToString()).ToList().Count;
				if (tareasRequeridas != tareasFinalizadas)
				{
					return new ResponseService()
					{
						status = 500,
						message = "Debe finalizar todas las tareas activas antes de cerrar el caso"
					};
				}
				Estado = Case_Estate.Finalizado.ToString();
				return Update() ?? new ResponseService()
				{
					status = 500,
					message = "error desconocido"
				};
			}
			throw new Exception("no tienes permisos para cerrar casos");
		}
		public List<Tbl_Case> GetCasosToVinculate(string? identity, Tbl_Case inst)
		{
			this.Id_Dependencia = inst.Id_Dependencia;
			if (AuthNetCore.HavePermission(Permissions.ADMINISTRAR_CASOS_DEPENDENCIA.ToString(), identity))
			{
				if (inst.Id_Vinculate != null)
				{
					return this.Where<Tbl_Case>( FilterData.NotIn( "Id_Vinculate",
					 [inst.Id_Vinculate.ToString()]))
					 .Where(c => c.Estado != Case_Estate.Solicitado.ToString()
					 && c.Estado != Case_Estate.Rechazado.ToString()
					 && c.Estado != Case_Estate.Finalizado.ToString()).ToList();
				}
				else
				{
					return this.Where<Tbl_Case>(FilterData.NotIn( "Id_Case",
					 [inst.Id_Case.ToString()]))
					 .Where(c => c.Estado != Case_Estate.Solicitado.ToString()
					 && c.Estado != Case_Estate.Rechazado.ToString()
					 && c.Estado != Case_Estate.Finalizado.ToString()).ToList();
				}
			}
			throw new Exception("no tienes permisos para vincular casos");
		}
		public List<Tbl_Case> GetVinculateCase(string? identity)
		{
			if (Id_Vinculate != null)
			{
				return GetOwCase(identity);
			}
			else
			{
				return new List<Tbl_Case>();
			}
		}
		public List<Tbl_Case> GetSolicitudesPendientesAprobarAdmin(string? identity)
		{
			if (AuthNetCore.HavePermission(Permissions.ADMIN_ACCESS.ToString(), identity))
			{
				Estado = Case_Estate.Solicitado.ToString();
				return Get<Tbl_Case>();
			}
			throw new Exception("no tienes permisos para aprobar casos");
		}

		public object SaveOwCase(string? identity)
		{
			UserModel user = AuthNetCore.User(identity);
			Tbl_Profile? profile = new Tbl_Profile { IdUser = user.UserId }.Find<Tbl_Profile>();
			Id_Perfil = profile?.Id_Perfil;
			
			Id_Dependencia = profile?.Tbl_Dependencias_Usuarios?.First()?.Id_Dependencia;
			Estado = Case_Estate.Activo.ToString();
			if (Id_Dependencia != null)
			{
				Id_Servicio = new Tbl_Servicios { Id_Dependencia = Id_Dependencia}.Find<Tbl_Servicios>()?.Id_Servicio;
			}			
			Save();
			return new ResponseService
			{
				status = 200,
				message = "Success"

			};
		}
	}

	public class MimeMessageCaseData
	{
		public string? MessageId { get; set; }
		public string? InReplyTo { get;  set; }
	}
}