using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using APPCORE;
using APPCORE.BDCore.Abstracts;
using APPCORE.Services;
using CAPA_NEGOCIO.MAPEO;

namespace CAPA_NEGOCIO.Services
{
	public class IMAPCaseServices
	{

		public async Task<bool> chargeAutomaticCase()
		{
			//WDataMapper? wDataMapper = SqlADOConexion.BuildDataMapper("localhost", "sa", "zaxscd", "PROYECT_MANAGER_BD");
			var dependenciaE = new Cat_Dependencias();
			//dependenciaE.SetConnection(wDataMapper);
			List<Cat_Dependencias> dependencias = dependenciaE.Get<Cat_Dependencias>();
			foreach (var dependencia in dependencias)
			{
				if (dependencia.Host == null || dependencia.Username == null || dependencia.AutenticationType == null)
				{
					continue;
				}
				else if (dependencia.AutenticationType == AutenticationTypeEnum.AUTH2.ToString()
				&& (dependencia.TENAT == null || dependencia.CLIENT == null || dependencia.CLIENT_SECRET == null))
				{
					continue;
				}
				else if (dependencia.AutenticationType == AutenticationTypeEnum.BASIC.ToString()
				&& dependencia.Password == null)
				{
					continue;
				}

				try
				{
					var messages = await new IMAPServices().GetMessages(new MailConfig()
					{
						HOST = dependencia.Host,
						PASSWORD = dependencia.Password,
						USERNAME = dependencia.Username,
						CLIENT = dependencia.CLIENT,
						CLIENT_SECRET = dependencia.CLIENT_SECRET,
						AutenticationType = Enum.Parse<AutenticationTypeEnum>(dependencia.AutenticationType),
						TENAT = dependencia.TENAT,
						OBJECTID = dependencia.OBJECTID,
						HostService = Enum.Parse<HostServices>(dependencia?.HostService)
					});
					var caseE = new Tbl_Case();
					//caseE.SetConnection(wDataMapper);
					messages.ForEach(async m => await caseE.CreateAutomaticCase(m, dependencia));
				}
				catch (System.Exception ex)
				{
					LoggerServices.AddMessageError($"error obteniendo mensjes de la dependencia: {dependencia.Descripcion}", ex);
				}

			}
			return true;
		}
	}
}
