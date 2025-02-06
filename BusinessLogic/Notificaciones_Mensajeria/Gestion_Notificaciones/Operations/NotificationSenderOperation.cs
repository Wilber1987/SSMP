using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLogic.Notificaciones_Mensajeria.Informe;
using CAPA_DATOS;
using CAPA_DATOS.Services;
using CAPA_NEGOCIO.Gestion_Mensajes.Operations;
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
				string? TemplateName = SystemConfig.AppConfigurationValue(AppConfigurationList.MettaApi, "TemplateName");
				string? TemplateLocation = SystemConfig.AppConfigurationValue(AppConfigurationList.MettaApi, "TemplateLocation");
				string? TemplateImageHeader = SystemConfig.AppConfigurationValue(AppConfigurationList.MettaApi, "TemplateImageHeader");

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
								$"{WhatsAppMessage.ObtenerExtensionPorDepartamento(notificacion?.NotificationData?.Departamento)}{notificacion?.Telefono}" ?? "",
								TemplateName ?? "es",
								TemplateLocation ?? "es",
								selectedParams,
								TemplateImageHeader
							));
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
			try
			{
				DateTime firstDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
				DateTime lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
				List<Notificaciones> notificaciones = new Notificaciones().Where<Notificaciones>(
					FilterData.Between("Fecha_Envio", firstDayOfMonth, lastDayOfMonth)
				);
				string? destinatarioString = SystemConfig.AppConfigurationValue(AppConfigurationList.AutomaticReports, "Destinatarios");
				List<string> destinatarios = destinatarioString?.Split(',').ToList() ?? [];
				string body = $"REPORTE DE NOTIFICACIONES ENVIADAS DEL {firstDayOfMonth} AL {lastDayOfMonth}";
				destinatarios.ForEach(destinatario =>
				{
					NotificationsReportBuilder.EnviarReporte(destinatario,
					$"REPORTE DE NOTIFICACIONES ENVIADAS", body, notificaciones);
				});
			}
			catch (Exception ex)
			{
				LoggerServices.AddMessageError("error enviando report", ex);
			}


		}
	}

}