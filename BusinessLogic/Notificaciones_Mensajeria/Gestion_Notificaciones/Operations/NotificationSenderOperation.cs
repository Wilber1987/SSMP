using BusinessLogic.Notificaciones_Mensajeria.Informe;
using APPCORE;
using APPCORE.Services;
using CAPA_NEGOCIO.IA;
using CAPA_NEGOCIO.MAPEO;
using DataBaseModel;
using DatabaseModelNotificaciones;
using WhatsAppApi;

namespace BusinessLogic.Notificaciones_Mensajeria.Gestion_Notificaciones.Operations
{
	public class NotificationSenderOperation
	{
		internal static async Task SendNotificationsAsync()
		{
			if (SystemConfig.IsWhatsAppActive())
			{

				List<Notificaciones> notificaciones = new Notificaciones()
					.Where<Notificaciones>(
						FilterData.Distinc("Enviado", true),
						FilterData.NotNull("Telefono")
				);
				string defaultTemplateName = "notificacion_correo_aduana";
				string defaultTemplateImageHeader = "https://correos.gob.gt/wp-content/uploads/2025/02/logo-190-X-90.png";

				string? TemplateName = Transactional_Configuraciones
					.GetParam(ConfiguracionesThemeEnum.TEMPLATE_NAME, defaultTemplateName)?
					.Valor ?? defaultTemplateName;
				string? TemplateImageHeader = Transactional_Configuraciones
					.GetParam(ConfiguracionesThemeEnum.TEMPLATE_IMAGE_HEADER, defaultTemplateImageHeader)?
					.Valor ?? defaultTemplateImageHeader;

				string? TemplateLocation = SystemConfig.AppConfigurationValue(AppConfigurationList.MettaApi, "TemplateLocation");

				//string? TemplateName = SystemConfig.AppConfigurationValue(AppConfigurationList.MettaApi, "TemplateName");
				//string? TemplateImageHeader = SystemConfig.AppConfigurationValue(AppConfigurationList.MettaApi, "TemplateImageHeader");

				foreach (Notificaciones notificacion in notificaciones)
				{
					try
					{
						if (notificacion?.Telefono != null && notificacion?.NotificationData != null
						&& (notificacion?.NotificationData?.Reenvios == null || notificacion?.NotificationData?.Reenvios < 3))
						{
							var paramlist = notificacion?.NotificationData?.Params?.Where(p => p.Name != "N.º de teléfono" && p.Name != "Correo").ToList();

							int paramsNumber = new Transactional_Configuraciones().GetParamNumberTemplate();


							var selectedParams = paramlist?.Take(paramsNumber).ToList();

							string response = await new ResponseAPI().SendResponseToWhatsAppWithTemplate(new WhatsAppMessage(
								notificacion!.Telefono,
								TemplateName ?? "es",
								TemplateLocation ?? "es",
								selectedParams,
								TemplateImageHeader
							), true);
							if (response.Contains("Success"))
							{
								notificacion!.Fecha_Envio = DateTime.Now;
								notificacion!.Enviado = true;
								notificacion.Update();
							}
							else
							{
								if (notificacion!.NotificationData?.Reenvios == null)
								{
									notificacion.NotificationData!.Reenvios = 1;
								}
								else
								{
									notificacion.NotificationData!.Reenvios++;
								}

								notificacion.Update();
								LoggerServices.AddMessageError("Error al enviar notificacion por whatsapp", new Exception(response));

							}
						}

					}
					catch (System.Exception ex)
					{
						LoggerServices.AddMessageError("Error al enviar notificacion por whatsapp", ex);
					}

					await Task.Delay(100);
				}
			}
			else
			{
				List<Notificaciones> notificaciones = new Notificaciones()
					.Where<Notificaciones>(
						FilterData.Distinc("Enviado", true),
						FilterData.NotNull("Email")
				);
				var config = SystemConfig.GetSMTPDefaultConfig();
				foreach (Notificaciones item in notificaciones)
				{
					var send = await SMTPMailServices.SendMail(config?.USERNAME,
						new List<string> { item.Email },
						item?.Titulo,
						item?.Mensaje,
						item?.Media,
						null,
						config
					);
					if (send)
					{
						try
						{
							item.Estado = MailState.ENVIADO.ToString();
							item.Update();
						}
						catch (System.Exception ex)
						{
							LoggerServices.AddMessageError($"correo enviado, error al actualizar estado del correo", ex);
						}
					}
				}
			}
		}
		public static void SendNotificationReport()
		{
			Transactional_Configuraciones automaticReportConfig = Transactional_Configuraciones.GetParam(ConfiguracionesThemeEnum.AUTOMATIC_SENDER_REPORT, "AUTOMATICO", ConfiguracionesTypeEnum.SELECT);
			if (automaticReportConfig.Valor?.ToUpper() != "AUTOMATICO")
			{
				return;
			}
			try
			{
				DateTime firstDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
				DateTime lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
				List<Notificaciones> notificaciones = new Notificaciones().Where<Notificaciones>(
					FilterData.Between("Fecha_Envio", firstDayOfMonth, lastDayOfMonth)
				);
				if (notificaciones.Count > 0)
				{
					Transactional_Configuraciones destinatariosAutomaticReportConfig =
						Transactional_Configuraciones.GetParam(ConfiguracionesThemeEnum.DESTINATARIOS_AUTOMATIC_SENDER_REPORT, "wilberj1987@gmail.com");

					//string? destinatarioString = SystemConfig.AppConfigurationValue(AppConfigurationList.AutomaticReports, "Destinatarios");
					string? destinatarioString = destinatariosAutomaticReportConfig.Valor;
					List<string> destinatarios = destinatarioString?.Split(',').ToList() ?? [];
					string body = $"REPORTE DE NOTIFICACIONES ENVIADAS DEL {firstDayOfMonth} AL {lastDayOfMonth}";

					NotificationsReportBuilder.EnviarReporte(destinatarios,
					$"REPORTE DE NOTIFICACIONES ENVIADAS", body, notificaciones);

				}
			}
			catch (Exception ex)
			{
				LoggerServices.AddMessageError("error enviando report", ex);
			}
		}

	}

}