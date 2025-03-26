using APPCORE;
using API.Controllers;
using APPCORE.Services;
using CAPA_NEGOCIO.IA;
using System.Threading.Tasks;

namespace CAPA_NEGOCIO.MAPEO
{
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

		public async Task<object?> SaveComment(string identity, Boolean withMail = true)
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

				// Manejo de archivos adjuntos
				foreach (var file in Attach_Files ?? new List<ModelFiles>())
				{
					ModelFiles Response = (ModelFiles)FileService.upload("Attach\\", file).body;
					file.Value = Response.Value;
					file.Type = Response.Type;
				}

				// Guardar el comentario
				Save();

				// Enviar correo si se requiere
				if (withMail)
				{
					CreateMailForComment(user, Tbl_Case);
				}

				// Verificar si el caso requiere integración con la API
				if (IsWithApi(Tbl_Case))
				{
					// Ejecutar el envío a la API en un hilo separado para que no bloquee la respuesta
					_ = Task.Run(async () =>
					{
						try
						{
							Tbl_Profile? tbl_Profile = new Tbl_Profile() { Id_Perfil = Tbl_Case.Id_Perfil }.Find<Tbl_Profile>();
							string response = await new ResponseAPI().SendResponseToUser(new UserMessage
							{
								Source = Tbl_Case?.MimeMessageCaseData?.PlatformType,
								UserId = tbl_Profile?.Correo_institucional,
							}, Body ?? "", Tbl_Case!.MimeMessageCaseData!.isWithIaResponse, Attach_Files);

							if (response.ToUpper() == "OK")
							{
								Tbl_Case!.MimeMessageCaseData!.WithAgent = true;
								Tbl_Case!.MimeMessageCaseData!.NewMessage = false;
								Tbl_Case.Update();
							}
						}
						catch (Exception ex)
						{
							LoggerServices.AddMessageError($@"Error respondiendo enviando respuesta a la API: {ex.Message},
								 (case: {Tbl_Case?.Id_Case}, 
								 plataforma: {Tbl_Case?.MimeMessageCaseData?.PlatformType})"
							, ex);
							// Manejo de errores en la tarea en segundo plano (puedes loguearlo si es necesario)						
						}
					});
				}

				// Prueba de guardar y eliminar registros
				var responses = new Tbl_Profile { Nombres = "prueba" }.Save();
				(responses as Tbl_Profile)?.Delete();
				new Tbl_Profile { Nombres = "pruebarrr" }.Save();

				// Confirmar transacción
				CommitGlobalTransaction();
				return this;
			}
			catch (System.Exception)
			{
				RollBackGlobalTransaction();
				throw;
			}
		}


		private bool IsWithApi(Tbl_Case? tbl_Case)
		{
			return tbl_Case?.MimeMessageCaseData?.PlatformType != null
				&& tbl_Case?.MimeMessageCaseData?.PlatformType != "WebAPI";
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
			 FilterData.In("Id_Case", caseTables.Select(c => c.Id_Case.ToString()).ToArray()));

		}
	}
}
