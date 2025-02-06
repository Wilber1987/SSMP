using API.Controllers;
using CAPA_DATOS;
namespace DataBaseModel
{
	public class Transactional_Configuraciones : EntityClass
	{
		[PrimaryKey(Identity = true)]
		public int? Id_Configuracion { get; set; }
		public string? Nombre { get; set; }
		public string? Descripcion { get; set; }
		public string? Valor { get; set; }
		public string? Tipo_Configuracion { get; set; }
		public Transactional_Configuraciones? GetConfig(String prop)
		{
			Nombre = prop;
			return Find<Transactional_Configuraciones>();
		}
		public List<Transactional_Configuraciones> GetTheme()
		{
			return Get<Transactional_Configuraciones>()
				.Where(x => x.Tipo_Configuracion != null &&
				 x.Tipo_Configuracion.Equals(ConfiguracionesTypeEnum.THEME.ToString())).ToList();
		}
		public List<Transactional_Configuraciones> GetTypeNumbers()
		{
			return Get<Transactional_Configuraciones>()
				.Where(x => x.Tipo_Configuracion != null &&
				 x.Tipo_Configuracion.Equals(ConfiguracionesTypeEnum.NUMBER.ToString())).ToList();
		}

		public List<Transactional_Configuraciones> GetGeneralData()
		{
			return Get<Transactional_Configuraciones>()
			   .Where(x => x.Tipo_Configuracion != null &&
				x.Tipo_Configuracion.Equals(ConfiguracionesTypeEnum.GENERAL_DATA.ToString())).ToList();
		}

		public object? UpdateConfig(string? identity)
		{
			if (!AuthNetCore.HavePermission(CAPA_DATOS.Security.Permissions.ADMIN_ACCESS.ToString(), identity))
			{
				throw new Exception("no tienes permisos para configurar la aplicación");
			}
			return this.Update();
		}

		internal int GetParamNumberTemplate()
		{
			return Convert.ToInt32(Find<Transactional_Configuraciones>(
				FilterData.Equal("Nombre", ConfiguracionesTypeEnum.PARAM_NUMBER_TEMPLATE)
			)?.Valor ?? "0");			   
		}
	}
	public enum AppConfigurationList
	{
		SQLCredentials,
		SQLCredentialsSeguiminento,
		IAServices,
		MettaApi,
		XApi,
        Smtp,
        AutomaticReports
    }	

	public enum ConfiguracionesTypeEnum
	{
		THEME, GENERAL_DATA, NUMBER,
		PARAM_NUMBER_TEMPLATE
	}

	public enum ConfiguracionesThemeEnum
	{
		TITULO, SUB_TITULO, NOMBRE_EMPRESA, LOGO_PRINCIPAL, LOGO, MEDIA_IMG_PATH,
		VERSION
	}

	public class Config
	{
		public static SystemConfig SystemConfig()
		{
			return new SystemConfig();
		}

	}


}
