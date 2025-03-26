using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Controllers;
using APPCORE;
using APPCORE.Security;
using APPCORE.Services;

namespace CAPA_NEGOCIO.MAPEO
{
	public class Tbl_Profile : APPCORE.Security.Tbl_Profile
	{
		public static Tbl_Profile? GetUserProfile(string identity)
		{
			return new Tbl_Profile() { IdUser = AuthNetCore.User(identity).UserId }.Find<Tbl_Profile>();
		}
		public int? Id_Pais_Origen { get; set; }
		public int? Id_Institucion { get; set; }
		public string? Indice_H { get; set; }

		public string? ORCID { get; set; }
		//[ManyToOne(TableName = "Security_Users", KeyColumn = "Id_User", ForeignKeyColumn = "IdUser")]
		public Security_Users? Security_Users { get; set; }
		//[ManyToOne(TableName = "Cat_Paises", KeyColumn = "Id_Pais", ForeignKeyColumn = "Id_Pais_Origen")]
		public Cat_Paises? Cat_Paises { get; set; }
		//[OneToMany(TableName = "Tbl_Case", KeyColumn = "Id_Perfil", ForeignKeyColumn = "Id_Perfil")]
		public List<Tbl_Case>? Tbl_Case { get; set; }
		//[OneToMany(TableName = "Tbl_Agenda", KeyColumn = "Id_Perfil", ForeignKeyColumn = "Id_Perfil")]
		public List<Tbl_Agenda>? Tbl_Agenda { get; set; }
		[OneToMany(TableName = "Tbl_Dependencias_Usuarios", KeyColumn = "Id_Perfil", ForeignKeyColumn = "Id_Perfil")]
		public List<Tbl_Dependencias_Usuarios>? Tbl_Dependencias_Usuarios { get; set; }

		[OneToMany(TableName = "Tbl_Servicios_Profile", KeyColumn = "Id_Perfil", ForeignKeyColumn = "Id_Perfil")]
		public List<Tbl_Servicios_Profile>? Tbl_Servicios_Profile { get; set; }

		//[OneToMany(TableName = "Tbl_Participantes", KeyColumn = "Id_Perfil", ForeignKeyColumn = "Id_Perfil")]

		public List<Tbl_Servicios?>? Tbl_Servicios { get; set; }

		[OneToMany(TableName = "Tbl_Grupos_Profile", KeyColumn = "Id_Perfil", ForeignKeyColumn = "Id_Perfil")]
		public List<Tbl_Grupos_Profile>? Tbl_Grupos_Profiles { get; set; }


		public List<Cat_Dependencias?>? Cat_Dependencias { get; set; }

		public List<Tbl_Participantes>? Tbl_Participantes { get; set; }
		public void SaveDependenciesAndservices()
		{
			if (this.Id_Perfil != null && this.Tbl_Servicios_Profile?.Count > 0)
			{
				new Tbl_Servicios_Profile { Id_Perfil = this.Id_Perfil }.Delete();
			}
			if (this.Id_Perfil != null && this.Tbl_Dependencias_Usuarios?.Count > 0)
			{
				new Tbl_Dependencias_Usuarios { Id_Perfil = this.Id_Perfil }.Delete();
			}

			this.Tbl_Dependencias_Usuarios?.ForEach(du =>
			{
				du.Id_Perfil = this.Id_Perfil;
				du.Save();
			});
			this.Tbl_Servicios_Profile?.ForEach(du =>
			{
				du.Id_Perfil = this.Id_Perfil;
				du.Save();
			});
		}

		public List<Tbl_Dependencias_Usuarios> TakeDepCoordinaciones()
		{
			Tbl_Dependencias_Usuarios DU = new Tbl_Dependencias_Usuarios();
			DU.Id_Perfil = this.Id_Perfil;
			return DU.Where<Tbl_Dependencias_Usuarios>(FilterData.In("Id_Cargo", "1", "2"));
		}
		public Object? TakeProfile()
		{
			try
			{
				return this.Find<Tbl_Profile>();
			}
			catch (Exception)
			{

				throw;
			}
		}
		public Object Postularse()
		{
			try
			{
				this.Estado = "POSTULANTE";
				SaveProfile();
				return true;
			}
			catch (Exception) { return false; }

		}
		public Object SaveProfile()
		{
			try
			{
				BeginGlobalTransaction();
				if (Foto != null)
				{
					ModelFiles? pic = (ModelFiles?)FileService.upload("profiles\\", new ModelFiles
					{
						Value = Foto,
						Type = "png",
						Name = "profile"
					}).body;
					Foto = pic?.Value?.Replace("wwwroot", "");

				}
				if (this.Id_Perfil == null)
				{
					this.Save();
				}
				else
				{
					Correo_institucional = null;
					IdUser = null;
					if (Cat_Dependencias != null)
					{
						Tbl_Dependencias_Usuarios = Cat_Dependencias?.Select(s => new Tbl_Dependencias_Usuarios { Id_Dependencia = s?.Id_Dependencia, Id_Perfil = this.Id_Perfil }).ToList();
						new Tbl_Dependencias_Usuarios() { Id_Perfil = Id_Perfil }.Delete();

					}
					if (Tbl_Servicios != null)
					{
						Tbl_Servicios_Profile = Tbl_Servicios?
							.Where(s => Tbl_Dependencias_Usuarios.Select(d => d.Id_Dependencia).ToList().Contains(s?.Id_Dependencia))
							.ToList()
							.Select(s => new Tbl_Servicios_Profile { Id_Servicio = s?.Id_Servicio, Id_Perfil = this.Id_Perfil })
							.ToList();
						new Tbl_Servicios_Profile() { Id_Perfil = Id_Perfil }.Delete();
					}
					this.Update();
				}
				SaveDependenciesAndservices();
				CommitGlobalTransaction();
				return this;

			}
			catch (System.Exception)
			{
				RollBackGlobalTransaction();
				throw;
			}


		}
		public Object AdmitirPostulante()
		{
			try
			{
				// new Security_Users()
				// {
				//     Mail = this.Correo_institucional,
				//     Nombres = this.Nombres + " " + this.Apellidos,
				//     Estado = "Activo",
				//     Descripcion = "Investigador postulado",
				//     Password = Guid.NewGuid().ToString(),
				//     Token = Guid.NewGuid().ToString(),
				//     Token_Date = DateTime.Now,
				//     Token_Expiration_Date = DateTime.Now.AddMonths(6),
				//     Security_Users_Roles = new List<Security_Users_Roles>(){
				//         new Security_Users_Roles() { Id_Role = 2 }
				//     }
				// }.SaveUser();
				// this.Estado = "ACTIVO";
				// this.Update("Id_Perfil");
				//MailServices.SendMail(this.Correo_institucional);
				return true;
			}
			catch (Exception) { return false; }
		}

		public List<Tbl_Profile> GetProfiles(string? identity)
		{
			List<Tbl_Profile> profiles = new List<Tbl_Profile>();
			if (AuthNetCore.HavePermission(Permissions.ADMIN_ACCESS.ToString(), identity))
			{
				profiles.AddRange(Where<Tbl_Profile>(FilterData.NotNull("IdUser")));
			}
			else if (AuthNetCore.HavePermission(Permissions.PERFIL_MANAGER.ToString(), identity))
			{
				UserModel user = AuthNetCore.User(identity);
				Tbl_Profile? userProfile = new Tbl_Profile { IdUser = user.UserId }.Find<Tbl_Profile>();

				List<Tbl_Dependencias_Usuarios> dependencias = new Tbl_Dependencias_Usuarios
				{
					Id_Perfil = userProfile?.Id_Perfil
				}.Get<Tbl_Dependencias_Usuarios>();

				List<Tbl_Dependencias_Usuarios> dependenciasU = new Tbl_Dependencias_Usuarios()
					.Where<Tbl_Dependencias_Usuarios>(FilterData.In(
						"Id_Dependencia",
						dependencias.Select(d => d.Id_Dependencia.ToString()).ToArray()
					));

				List<Tbl_Profile> profilesD = Where<Tbl_Profile>(FilterData.In(
						"Id_Perfil",
						dependenciasU.Select(d => d.Id_Perfil).ToArray()
					));
				profiles.AddRange(profilesD);
			}
			else
			{
				List<Tbl_Profile> profilesD = GetProfilesByGroup(identity);
				profiles.AddRange(profilesD);
			}

			foreach (var profile in profiles)
			{
				foreach (var dep in profile.Tbl_Dependencias_Usuarios ?? new List<Tbl_Dependencias_Usuarios>())
				{
					dep.Cat_Dependencias!.Password = "PROTECTED";
				}
				profile.Tbl_Servicios = profile.Tbl_Servicios_Profile?.Select(s => s.Tbl_Servicios).ToList();
				profile.Cat_Dependencias = profile.Tbl_Dependencias_Usuarios?.Select(s => s.Cat_Dependencias).ToList();
			}
			return profiles;
		}

		public static List<Tbl_Profile> GetProfilesByGroup(string? identity)
		{
			UserModel user = AuthNetCore.User(identity);
			Tbl_Profile? profile = new Tbl_Profile { IdUser = user.UserId }.Find<Tbl_Profile>();
			var grupos = new Tbl_Grupos_Profile().Where<Tbl_Grupos_Profile>(
				FilterData.Equal("Id_Perfil", profile?.Id_Perfil),
				FilterData.Equal("Estado", GroupState.ACTIVO.ToString())
			);
			List<Tbl_Profile> profilesD = new Tbl_Grupos_Profile()
				.Where<Tbl_Grupos_Profile>(FilterData.In("Id_Grupo", grupos.Select(g => g.Id_Grupo).ToArray()))
				.Select(g => g.Tbl_Profile ?? new Tbl_Profile())
				.Where(g => g.Id_Perfil != profile?.Id_Perfil).ToList();
			return profilesD;
		}

		public static Tbl_Profile Get_Profile(UserModel User)
		{
			return Get_Profile(User.UserId.GetValueOrDefault());
		}

		public static Tbl_Profile Get_Profile(int UserId)
		{
			Tbl_Profile? tbl_Profile = new Tbl_Profile { IdUser = UserId }.Find<Tbl_Profile>();
			tbl_Profile!.Cat_Dependencias = tbl_Profile.Tbl_Dependencias_Usuarios?.Select(d => d.Cat_Dependencias).ToList();
			return tbl_Profile ?? new Tbl_Profile { IdUser = UserId };
		}

	}	
}