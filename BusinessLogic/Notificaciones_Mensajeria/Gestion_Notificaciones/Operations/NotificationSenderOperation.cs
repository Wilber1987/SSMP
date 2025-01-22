using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CAPA_DATOS;
using CAPA_NEGOCIO.Gestion_Mensajes.Operations;
using CAPA_NEGOCIO.IA;
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
						FilterData.Distinc("Enviado", true)
				);
				foreach (var notificacion in notificaciones)
				{
					try
					{
						if (notificacion?.Telefono != null)
						{
							string response = await new ResponseAPI().SendResponseToWhatsAppWithTemplate(new WhatsAppMessage(
								$"{WhatsAppMessage.ObtenerExtensionPorDepartamento(notificacion?.NotificationData?.Departamento)}{notificacion?.Telefono}" ?? "",
								"notificacion_paquete",
								"es",
								[
									notificacion?.NotificationData?.Departamento,
									notificacion?.NotificationData?.Direccion,
									notificacion?.NotificationData?.Destinatario,
									notificacion?.NotificationData?.Dpi ?? "DESCONOCIDO",
									notificacion?.NotificationData?.NumeroPaquete ?? "DESCONOCIDO",
									notificacion?.NotificationData?.Correlativo,
									notificacion?.NotificationData?.Fecha,
									notificacion?.NotificationData?.Destinatario,
									"Si",
									"Correos y telegrafos",
									DateTime.Now.ToString("dd/MM/yyyy"),
									DateTime.Now.ToString("HH:mm")
								]
							));
							if (response.Contains("Success"))
							{	
								notificacion.Enviado = true;
								notificacion.Update();								
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

		}
	}
}