using Microsoft.Extensions.Configuration;
namespace DataBaseModel
{
    public class SystemConfig
	{
		public SystemConfig()
		{
			configuraciones = new Transactional_Configuraciones().Get<Transactional_Configuraciones>();
			TITULO = configuraciones.Find(c => c.Nombre != null &&
				c.Nombre.Equals(ConfiguracionesThemeEnum.TITULO.ToString()))?.Valor ?? TITULO;
			SUB_TITULO = configuraciones.Find(c => c.Nombre != null &&
				c.Nombre.Equals(ConfiguracionesThemeEnum.SUB_TITULO.ToString()))?.Valor ?? SUB_TITULO;
			NOMBRE_EMPRESA = configuraciones.Find(c => c.Nombre != null &&
				c.Nombre.Equals(ConfiguracionesThemeEnum.NOMBRE_EMPRESA.ToString()))?.Valor ?? NOMBRE_EMPRESA;
			LOGO_PRINCIPAL = configuraciones.Find(c => c.Nombre != null &&
				c.Nombre.Equals(ConfiguracionesThemeEnum.LOGO_PRINCIPAL.ToString()))?.Valor ?? LOGO_PRINCIPAL;
			MEDIA_IMG_PATH = configuraciones.Find(c => c.Nombre != null &&
				c.Nombre.Equals(ConfiguracionesThemeEnum.MEDIA_IMG_PATH.ToString()))?.Valor ?? MEDIA_IMG_PATH;
			VERSION = configuraciones.Find(c => c.Nombre != null &&
				c.Nombre.Equals(ConfiguracionesThemeEnum.VERSION.ToString()))?.Valor ?? VERSION;

		}
		public string TITULO = "TEMPLATE";
		public string SUB_TITULO = "Template";
		public string NOMBRE_EMPRESA = "TEMPLATE";
		public string LOGO_PRINCIPAL = "logo.png";
		public string MEDIA_IMG_PATH = "/media/img/";
		public string VERSION = "2024.07";
		public List<Transactional_Configuraciones> configuraciones = new List<Transactional_Configuraciones>();

		public static bool isAutomaticCaseActive()
		{
			//TODO IMPLEMENTAR ESTE METODO
			return true;
		}
		public static bool isNotificationsActive()
		{
			//TODO IMPLEMENTAR ESTE METODO
			return true;
		}
		public static bool isMessagesActive()
		{
			//TODO IMPLEMENTAR ESTE METODO
			return false;
		}
		public static bool isQuestionnairesActive()
		{
			//TODO IMPLEMENTAR ESTE METODO
			return true;
		}
		public static IConfigurationRoot AppConfiguration()
		{
			return new ConfigurationBuilder()
					.SetBasePath(Directory.GetCurrentDirectory())
					.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
					.Build();
		}
		public static IConfigurationSection AppConfigurationSection(string sectionName)
		{
			return new ConfigurationBuilder()
					.SetBasePath(Directory.GetCurrentDirectory())
					.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
					.Build().GetSection(sectionName);
		}
		public static string? AppConfigurationValue(AppConfigurationList sectionName, string value)
		{
			return new ConfigurationBuilder()
					.SetBasePath(Directory.GetCurrentDirectory())
					.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
					.Build().GetSection(sectionName.ToString())[value];
		}

	}


}
