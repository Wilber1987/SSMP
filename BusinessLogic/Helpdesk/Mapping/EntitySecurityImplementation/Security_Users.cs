using CAPA_DATOS;
using API.Controllers;
using CAPA_DATOS.Security;
using CAPA_DATOS.Services;

namespace CAPA_NEGOCIO.MAPEO
{
    public class Security_Users : CAPA_DATOS.Security.Security_Users
	{
		[OneToMany(TableName = "Security_Users_Roles", KeyColumn = "Id_User", ForeignKeyColumn = "Id_User")]
		public new List<Security_Users_Roles>? Security_Users_Roles { get; set; }

		[OneToMany(TableName = "Tbl_Profile", KeyColumn = "Id_User", ForeignKeyColumn = "IdUser")]
		public new List<Tbl_Profile>? Tbl_Profiles { get; set; }


		public new Security_Users? GetUserData()
		{
			Security_Users? user = this.Find<Security_Users>();
			if (user != null && user.Estado == "ACTIVO")
			{
				user.Security_Users_Roles = new Security_Users_Roles()
				{
					Id_User = user.Id_User
				}.Get<Security_Users_Roles>();
				foreach (Security_Users_Roles role in user.Security_Users_Roles ?? new List<Security_Users_Roles>())
				{
					role.Security_Role?.GetRolData();
				}
				return user;
			}
			if (user?.Estado == "INACTIVO")
			{
				throw new Exception("usuario inactivo");
			}
			return null;
		}

		public new object SaveUser(string identity)
		{
			if (!AuthNetCore.HavePermission(Permissions.ADMINISTRAR_USUARIOS.ToString(), identity))
			{
				throw new Exception("no tiene permisos");
			}
			try
			{
				this.BeginGlobalTransaction();
				if (this.Password != null)
				{
					this.Password = EncrypterServices.Encrypt(this.Password);
				}
				if (this.Id_User == null)
				{
					if (new Security_Users() { Mail = this.Mail }.Exists())
					{
						throw new Exception("Correo en uso");
					}
					var user = Save();
					if (Tbl_Profiles != null)
					{
						Tbl_Profiles?.ForEach(Tbl_Profile =>
						{
							var pic = (ModelFiles?)FileService.upload("profiles\\", new ModelFiles
							{
								Value = Tbl_Profile.Foto,
								Type = "png",
								Name = "profile"
							}).body;
							Tbl_Profile.Foto = pic?.Value?.Replace("wwwroot", "");
							Tbl_Profile.IdUser = ((Security_Users?)user)?.Id_User;
							Tbl_Profile.Save();
							Tbl_Profile?.SaveDependenciesAndservices();
						});

					}
					else
					{
						var Profile = new Tbl_Profile()
						{
							Nombres = this.Nombres,
							Estado = this.Estado,
							Correo_institucional = this.Mail,
							Foto = "\\Media\\profiles\\avatar.png",
							IdUser = Id_User
						};

						Tbl_Profiles = [Profile];
					}

				}
				else
				{
					if (this.Estado == null)
					{
						this.Estado = "ACTIVO";
					}
					if (Tbl_Profiles != null)
					{
						Tbl_Profiles?.ForEach(Tbl_Profile =>
						{
							if (Tbl_Profile.Id_Perfil == null)
							{
								Tbl_Profile.IdUser = Id_User;
								Tbl_Profile.Save();
							}
							else
							{
								Tbl_Profile.Update();
							}
							Tbl_Profile?.SaveDependenciesAndservices();
						});

					}
					this.Update();
				}
				if (this.Security_Users_Roles != null)
				{
					Security_Users_Roles IdI = new Security_Users_Roles();
					IdI.Id_User = this.Id_User;
					IdI.Delete();
					foreach (Security_Users_Roles obj in this.Security_Users_Roles)
					{
						obj.Id_User = this.Id_User;
						obj.Save();
					}
				}
				this.CommitGlobalTransaction();
				return this;
			}
			catch (System.Exception)
			{
				this.RollBackGlobalTransaction();
				throw;
			}

		}


		public new object GetUsers()
		{
			var Security_Users_List = this.Get<Security_Users>();
			foreach (Security_Users User in Security_Users_List)
			{
				User.Password = null;
				//User.Tbl_Profile = User.Tbl_Profile?.Find<Tbl_Profile>();
			}
			return Security_Users_List;
		}
		public bool IsAdmin()
		{
			return Security_Users_Roles?.Find(r => r.Security_Role?.Security_Permissions_Roles?.Find(p =>
			 	p.Security_Permissions.Descripcion.Equals(Permissions.ADMIN_ACCESS.ToString())
			) != null) != null;
		}
		public static UserModel RecoveryPassword(string? mail)
		{
			var Cat_Dependencias = new Cat_Dependencias { DefaultDependency = true }.SimpleFind<Cat_Dependencias>();
			var dependenciaConfig = new MailConfig
			{
				HOST = Cat_Dependencias?.SMTPHOST,
				PASSWORD = Cat_Dependencias?.Password,
				USERNAME = Cat_Dependencias?.Username,
				CLIENT = Cat_Dependencias?.CLIENT,
				CLIENT_SECRET = Cat_Dependencias?.CLIENT_SECRET,
				AutenticationType = Enum.Parse<AutenticationTypeEnum>(Cat_Dependencias?.AutenticationType),
				TENAT = Cat_Dependencias?.TENAT,
				OBJECTID = Cat_Dependencias?.OBJECTID,
				HostService = Enum.Parse<HostServices>(Cat_Dependencias?.HostService)
			};
			SMTPMailServices.Config = dependenciaConfig;
			return AuthNetCore.RecoveryPassword(mail, dependenciaConfig);
		}

		public new object? changePassword(string? identfy)
		{
			var security_User = AuthNetCore.User(identfy).UserData;
			Password = EncrypterServices.Encrypt(Password);
			Id_User = security_User?.Id_User;
			return Update();
		}

		public new Tbl_Profile Get_Profile()
		{
			return Tbl_Profile.Get_Profile(Id_User.GetValueOrDefault());
		}
	}
}
