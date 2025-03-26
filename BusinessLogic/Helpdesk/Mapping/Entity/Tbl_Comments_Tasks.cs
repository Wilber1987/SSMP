using APPCORE;
using API.Controllers;
using APPCORE.Services;

namespace CAPA_NEGOCIO.MAPEO
{
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
}
