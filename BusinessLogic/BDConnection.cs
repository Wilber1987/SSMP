using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CAPA_DATOS;
using CAPA_DATOS.BDCore.Abstracts;
using DataBaseModel;
using Microsoft.Extensions.Configuration;

namespace BusinessLogic.Rastreo.Model
{
	public class BDConnection
	{
		public BDConnection()
		{
			var configuration = SystemConfig.AppConfiguration();
			SqlCredentials = configuration.GetSection("SQLCredentials");
			SqlCredentialsSeguimiento = configuration.GetSection("SQLCredentialsSeguiminento");
			DataMapperSeguimiento = SqlADOConexion.BuildDataMapper(
				SqlCredentialsSeguimiento["Server"],
				SqlCredentialsSeguimiento["User"],
				SqlCredentialsSeguimiento["Password"],
				SqlCredentialsSeguimiento["Database"]
			);
		}
		//  public WDataMapper? DataMapper = SqlADOConexion.BuildDataMapper("localhost", "sa", "zaxscd", "IPS5Db");
		public WDataMapper? DataMapperSeguimiento { get; set; }
		public IConfigurationSection SqlCredentials { get; private set; }
		public IConfigurationSection SqlCredentialsSeguimiento { get; private set; }

		public bool IniciarMainConecction()
		{
			return SqlADOConexion.IniciarConexion(
				SqlCredentials["User"],
				SqlCredentials["Password"],
				SqlCredentials["Server"],
				SqlCredentials["Database"]
			);//SIASMOP USAV
		}
	}
}