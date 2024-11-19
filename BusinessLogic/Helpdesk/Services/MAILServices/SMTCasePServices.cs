using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using CAPA_DATOS;
using CAPA_DATOS.Services;
using CAPA_NEGOCIO.MAPEO;

namespace CAPA_NEGOCIO.Services
{
	public class SMTPCaseServices
	{
		public async Task<bool> sendCaseMailNotificationsAsync()
		{


			List<Tbl_Mails> caseMail = new Tbl_Mails()
			{
				Estado = MailState.PENDIENTE.ToString()
			}.Get<Tbl_Mails>();

			foreach (var item in caseMail)
			{
				try
				{
					await Task.Delay(100);
					if (item.Id_Case == null)
					{
						continue;
					}
					var Tcase = new Tbl_Case() { Id_Case = item.Id_Case }.Find<Tbl_Case>();
					if (Tcase?.Cat_Dependencias?.SMTPHOST == null && Tcase?.Cat_Dependencias?.Username == null)
					{
						continue;
					}
					var dependencia = Tcase?.Cat_Dependencias;
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
					var send = await SMTPMailServices.SendMail(item.FromAdress,
					item.ToAdress,
					item.Subject,
					item.Body,
					item.Attach_Files,
					item.Uid,
					new MailConfig()
					{
						HOST = Tcase?.Cat_Dependencias?.SMTPHOST,
						PASSWORD = Tcase?.Cat_Dependencias?.Password,
						USERNAME = Tcase?.Cat_Dependencias?.Username,
						CLIENT = Tcase?.Cat_Dependencias?.CLIENT,
						CLIENT_SECRET = Tcase?.Cat_Dependencias?.CLIENT_SECRET,
						AutenticationType = Enum.Parse<AutenticationTypeEnum>(Tcase?.Cat_Dependencias?.AutenticationType),
						TENAT = Tcase?.Cat_Dependencias?.TENAT,
						OBJECTID = Tcase?.Cat_Dependencias?.OBJECTID,
						HostService = Enum.Parse<HostServices>(Tcase?.Cat_Dependencias?.HostService)
					});
					if (send)
					{
						try
						{
							item.Estado = MailState.ENVIADO.ToString();
							item.Update();
						}
						catch (System.Exception ex)
						{
							LoggerServices.AddMessageError($"correo enviado, error al actualizar estado del correo {item.Uid}", ex);
						}
					}
				}
				catch (System.Exception ex)
				{
					LoggerServices.AddMessageError($"error al enviar el correo {item.Uid}", ex);
				}

			}

			return true;
		}
	}
}
