using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using APPCORE;
using APPCORE.Services;
using CAPA_NEGOCIO.MAPEO;
using DataBaseModel;
using System.Net;
using System.Net.Mail;
using DocumentFormat.OpenXml.Office2013.PowerPoint.Roaming;

namespace CAPA_NEGOCIO.Services
{
	public class SmtpHelpdeskServices
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
                    bool isWithDependencyHostConfig = IsWithDependencyHostConfig(Tcase);
                   
                    var send = await SMTPMailServices.SendMail(item.FromAdress,
                        item.ToAdress,
                        item.Subject,
                        item.Body,
                        item.Attach_Files,
                        item.Uid,
                        isWithDependencyHostConfig ? GetMailConfigPorDependencia(Tcase):GetMailDefaultConfig());
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

        private static bool IsWithDependencyHostConfig(Tbl_Case? Tcase)
        {
            if (Tcase?.Cat_Dependencias?.SMTPHOST == null && Tcase?.Cat_Dependencias?.Username == null)
            {
                return false;
            }
            var dependencia = Tcase?.Cat_Dependencias;
            if (dependencia.Host == null || dependencia.Username == null || dependencia.AutenticationType == null)
            {
                return false;
            }
            else if (dependencia.AutenticationType == AutenticationTypeEnum.AUTH2.ToString()
            && (dependencia.TENAT == null || dependencia.CLIENT == null || dependencia.CLIENT_SECRET == null))
            {
                return false;
            }
            else if (dependencia.AutenticationType == AutenticationTypeEnum.BASIC.ToString()
            && dependencia.Password == null)
            {
                return false;
            }

            return true;
        }

        public static async Task<bool> EnviarCorreoConAdjunto(List<string> destinatario, string asunto, string cuerpo, string htmlContent, string? header = null, string? footer = null)
        {
            var pdfContent = PdfService.ConvertHtmlToPdf(htmlContent, "oficio-horizontal", header, footer);

            var tempFilePath = Path.GetTempFileName();
            File.WriteAllBytes(tempFilePath, pdfContent);
            List<ModelFiles> attachs = [new ModelFiles
            {
                Value = tempFilePath,
                Type = "application/pdf",
                Name= "InformeNotificaciones.pdf"
            }];
            return await SendMailWithDefaultConfig(destinatario, asunto, cuerpo, attachs);
        }

        public static async Task<bool> SendMailWithDefaultConfig(List<string> destinatario, string asunto, string cuerpo, List<ModelFiles> attachs)
        {
            try
            {
                // Enviar el correo
                var send = await SMTPMailServices.SendMail("notificaciones.sat@correos.gob.gt",
                       destinatario,
                       asunto,
                       cuerpo,
                       attachs,
                       null,
                       GetMailDefaultConfig());

                Console.WriteLine("Correo enviado exitosamente.");
                return send;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al enviar el correo: " + ex.Message);
                return false;
            }
        }


		private static MailConfig GetMailConfigPorDependencia(Tbl_Case? Tcase)
		{
			return new MailConfig()
			{
				HOST = Tcase?.Cat_Dependencias?.SMTPHOST,
				PASSWORD = Tcase?.Cat_Dependencias?.Password,
				USERNAME = Tcase?.Cat_Dependencias?.Username,
				CLIENT = Tcase?.Cat_Dependencias?.CLIENT,
				CLIENT_SECRET = Tcase?.Cat_Dependencias?.CLIENT_SECRET,
				AutenticationType = Enum.Parse<AutenticationTypeEnum>(Tcase?.Cat_Dependencias?.AutenticationType ?? AutenticationTypeEnum.BASIC.ToString()),
				TENAT = Tcase?.Cat_Dependencias?.TENAT,
				OBJECTID = Tcase?.Cat_Dependencias?.OBJECTID,
				HostService = Enum.Parse<HostServices>(Tcase?.Cat_Dependencias?.HostService ?? HostServices.PRIVATE.ToString()),
			};
		}
		private static MailConfig GetMailDefaultConfig()
		{
			string? domain = SystemConfig.AppConfigurationValue(AppConfigurationList.Smtp, "Domain");
			string? user = SystemConfig.AppConfigurationValue(AppConfigurationList.Smtp, "User");
			string? password = SystemConfig.AppConfigurationValue(AppConfigurationList.Smtp, "Password");
			string? port = SystemConfig.AppConfigurationValue(AppConfigurationList.Smtp, "Port");
			return new MailConfig()
			{
				HOST = domain,
				PASSWORD = password,
				DISPLAYNAME = "CORREOS DE GUATEMALA",
				USERNAME = user,
				PORT = Convert.ToInt32(port),
				AutenticationType = AutenticationTypeEnum.BASIC,
				HostService = HostServices.PRIVATE
			};
		}


	}
}
