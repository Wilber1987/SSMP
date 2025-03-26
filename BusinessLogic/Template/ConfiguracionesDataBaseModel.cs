using API.Controllers;
using APPCORE;
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
		public static Transactional_Configuraciones GetParam(ConfiguracionesThemeEnum prop, string defaultValor = "", ConfiguracionesTypeEnum TYPE = ConfiguracionesTypeEnum.THEME)
		{

			var find = new Transactional_Configuraciones
			{
				Nombre = prop.ToString(),
			}.Find<Transactional_Configuraciones>();
			if (find == null)
			{
				find = new Transactional_Configuraciones
				{
					Valor = defaultValor,
					Descripcion = prop.ToString(),
					Nombre = prop.ToString(),
					Tipo_Configuracion = TYPE.ToString()
				};
				find.Save();
			}
			return find;
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
			if (!AuthNetCore.HavePermission(APPCORE.Security.Permissions.ADMIN_ACCESS.ToString(), identity))
			{
				throw new Exception("no tienes permisos para configurar la aplicación");
			}
			return this.Update();
		}

		internal int GetParamNumberTemplate()
		{
			return Convert.ToInt32(Find<Transactional_Configuraciones>(
				FilterData.Equal("Nombre", ConfiguracionesThemeEnum.PARAM_NUMBER_TEMPLATE)
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
		THEME, GENERAL_DATA, NUMBER, SELECT,
        IMAGE
    }
    

	public enum ConfiguracionesThemeEnum
	{
		TITULO, SUB_TITULO, NOMBRE_EMPRESA, LOGO_PRINCIPAL, LOGO, MEDIA_IMG_PATH,
		VERSION, PARAM_NUMBER_TEMPLATE, MESSAGE_TEMPLATE, AUTOMATIC_SENDER_REPORT,
        DESTINATARIOS_AUTOMATIC_SENDER_REPORT,
        MEMBRETE_HEADER,
        MEMBRETE_FOOTHER,
        DOMAIN_URL,
        //ESTAS SE ESTAN USANDO PARA PLANTILLAS DE MENSAJES DE META API
        TEMPLATE_NAME,
        TEMPLATE_IMAGE_HEADER,
        BLACK_LIST
    }

	public class Config
	{
		public static SystemConfig SystemConfig()
		{
			return new SystemConfig();
		}

	}


}
