using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using CAPA_DATOS.Services;
using DataBaseModel;
using DatabaseModelNotificaciones;

namespace BusinessLogic.Notificaciones_Mensajeria.Informe
{
	public class NotificationsReportBuilder
	{

		public static string DrawInformeNotifications(List<Notificaciones> notificaciones)
		{
			var NotificationsNotificacion = notificaciones.GroupBy(p => p.Year).ToDictionary(g => g.Key, g => g.ToList());
			string containerInforme = ViewNotificacionInforme(NotificationsNotificacion);
			return containerInforme;
		}
		public static string ViewNotificacionInforme(Dictionary<string, List<Notificaciones>> notificaciones)
		{
			var html = new StringBuilder();
			html.Append(@"<div class='Notification-container'>
						<style>
							.Notification-container {
								
								flex-direction: column;
								gap: 10px;
								padding: 10px;
							}
							.component{
						display: block;
						}    
						.CANCELADO {
							color: green;
						}  
						.PENDIENTE {
							color: red
						}  
						.mes-container {
							/* display: grid;
							grid-template-columns: repeat(14, 1fr); */
							gap: 5px;
							& h3 {
								grid-column: span 14;
								font-size: 1em;
								border-bottom: 1px solid #919191;
							}
							
						}   
						table.mes-container {
							gap: 5px;
							border-collapse: collapse;
							width: 100%;
						}
						.Notification-details-container {
							/* display: contents; */
							grid-column: span 14;
						}
						.total-container {
							display: flex;
							justify-content: space-between;
							background-color: #f1f1f1;
							font-weight: bold;
						}
						.Notification-title {
							font-size: 0.8em;
							padding: 8px;
							font-weight: bold;
							background-color: #f1f1f1;
						}
						.Notification-value {
							font-size: 0.8em;
							padding: 8px;
						}
						.total-container {			
							font-size: 1em;
						} 
						.value, .total-cargos {
							text-align: right;
						}  
						.Historial{
							display: flex;
							flex-direction: column;            
							gap: 20px;
						}   
						.Historial .options-container {
							display: flex;
							align-items: center;
							justify-content: space-between;
							grid-column: span 2;
						}
						.Notificacion-card-container {
							display: flex;
							border: 1px solid #d6d6d6;;
							border-radius: 10px;
							cursor: pointer;
							padding: 10px;
							max-width: 400px; 
						}
						.Notificacion-card {
							display: flex;         
							gap: 10px;
							min-width: 400px;
							align-items: center;
						}
						.alumnos-container {
							display: flex;
							flex-direction: row;
							flex-wrap: wrap;
							gap: 10px;
						}
						.TabContainer {
							min-height: 400px;
						}
						.avatar-est{
							height: 100px;
							width: 80px;
							min-width: 80px;
							border-radius: 10px !important;
							object-fit: cover;
						}
						
						.aside-container {            
							padding: 0;
							border-radius: 0;
							box-shadow: unset
						}
						.Notificacion-container {
							display: flex;
							flex-direction: column;
							gap: 0px;
							padding: 10px;
							& .Notificacion {
								margin-bottom: 20px;
							}
							& .data-container {
								display: flex;
								justify-content: flex-start;
								/* border-bottom: 1px solid #d6d6d6; */
								& .Notificacion-prop {
									background-color: #f1f1f1;
									width: 100px;
								}
								& label {
									padding: 10px;
									margin-bottom: 0;
								}
							}
						}
						
						@media (max-width: 768px) {
							.Historial{               
								grid-template-columns: 100%;
							} 
							.Historial .options-container {
								grid-column: span 1;
							}
							.TabContainer {
								border-left: unset;
								padding-left: unset;                
							}
						}
							
						</style>");

			foreach (var notificacionId in notificaciones.Keys)
			{
				var notificacion = notificaciones[notificacionId][0];
				html.Append($@"<div>
							<h1 style='color: #09559c'>{notificacion.Year}</h1>
							<h2 style='color: #0c3964; border-bottom: solid 1px #cccccc;'>Informe de notificaciones enviadas {notificacion.Year}</h2>
						  </div>");

				var notificacionesMes = BuildNotificacionesXMes(notificaciones[notificacionId]);
				html.Append(notificacionesMes);
			}

			html.Append("</div>");
			return html.ToString();
		}

		private static string BuildNotificacionesXMes(List<Notificaciones> notificaciones)
		{
			var html = new StringBuilder();
			html.Append("<div class='Notification-mes-container'></div>");

			var notificacionesGroup = new Dictionary<string, List<Notificaciones>>();
			foreach (var notificacion in notificaciones)
			{
				if (!notificacionesGroup.ContainsKey(notificacion.Month))
				{
					notificacionesGroup[notificacion.Month] = new List<Notificaciones>();
				}
				notificacionesGroup[notificacion.Month].Add(notificacion);
			}

			foreach (var mes in notificacionesGroup.Keys)
			{
				html.Append($"<h3>{mes.ToUpper()}</h3>");
				html.Append(@"<table class='mes-container'>
							<tr class='Notification-details-container'>
								<td class='Notification-title'>Identificador</td>
								<td class='Notification-title'>Cita</td>
								<td class='Notification-title'>Nombre del destinatario</td>
								<td class='Notification-title'>Dirección del destinatario</td>
								<td class='Notification-title'>No de teléfono</td>
								<td class='Notification-title'>Fecha de ingreso</td>
								<td class='Notification-title'>Departamento</td>
								<td class='Notification-title'>Municipio</td>
								<td class='Notification-title'>Agencia</td>
								<td class='Notification-title'>Correlativo</td>
								<td class='Notification-title'>Fecha de ingreso de notificación</td>
								<td class='Notification-title'>DPI</td>
								<td class='Notification-title'>NIT</td>
								<td class='Notification-title'>E-mail</td>
							</tr>");

				foreach (var notificacion in notificacionesGroup[mes])
				{
					html.Append(NotificationsCard(notificacion));
				}
				html.Append($@"</table>");
				
				html.Append($@"<div class='Notification-details-container total-container'>
							<div class='Notification-title' style='grid-column: span 12'>Sub-Total</div>
							<div class='Notification-title value'>{notificacionesGroup[mes].Count}</div>
						  </div>");
			}

			html.Append($@"<div class='mes-container total-container'>
						<div class='Notification-title' style='grid-column: span 12'>Total</div>
						<div class='Notification-title total-cargos'>{notificaciones.Count}</div>
					  </div>");

			return html.ToString();
		}

		private static string NotificationsCard(Notificaciones notificacion)
		{
			return $@"<tr class='Notification-details-container'>
					<td class='Notification-value'>{notificacion.GetParam("Identificador")}</td>
					<td class='Notification-value'>{notificacion.GetParam("Cita")}</td>
					<td class='Notification-value'>{notificacion.GetParam("Destinatario")}</td>
					<td class='Notification-value'>{notificacion.GetParam("DireccionDestinatario")}</td>
					<td class='Notification-value'>{notificacion.GetParam("Telefono")}</td>
					<td class='Notification-value'>{notificacion.Fecha.GetValueOrDefault().ToShortDateString()}</td>
					<td class='Notification-value'>{notificacion.GetParam("Departamento")}</td>
					<td class='Notification-value'>{notificacion.GetParam("Municipio")}</td>
					<td class='Notification-value'>{notificacion.GetParam("Agencia")}</td>
					<td class='Notification-value'>{notificacion.GetParam("Correlativo")}</td>
					<td class='Notification-value'>{notificacion.Fecha_Envio.GetValueOrDefault().ToShortDateString()}</td>
					<td class='Notification-value'>{notificacion.GetParam("Dpi")}</td>
					<td class='Notification-value'>{notificacion.GetParam("Nit")}</td>
					<td class='Notification-value'>{notificacion.GetParam("Correo")}</td>
				  </tr>";
		}

		public static void EnviarReporte(string destinatario, string asunto, string cuerpo, List<Notificaciones> notificaciones)
		{
			EnviarCorreoConAdjunto(destinatario, asunto, cuerpo, DrawInformeNotifications(notificaciones));
		}

		public static void EnviarCorreoConAdjunto(string destinatario, string asunto, string cuerpo, string htmlContent)
		{
			string? domain = SystemConfig.AppConfigurationValue(AppConfigurationList.Smtp, "Domain");
			string? user = SystemConfig.AppConfigurationValue(AppConfigurationList.Smtp, "User");
			string? password = SystemConfig.AppConfigurationValue(AppConfigurationList.Smtp, "Password");
			string? port = SystemConfig.AppConfigurationValue(AppConfigurationList.Smtp, "Port");

			var pdfContent = PdfService.ConvertHtmlToPdf(htmlContent, "oficio-horizontal");
			// Convertir HTML a PDF
			// Crear el mensaje de correo
			var mailMessage = new MailMessage
			{
				From = new MailAddress("notificaciones.sat@correos.gob.gt"), // Cuenta de correo
				Subject = asunto,
				Body = cuerpo,
				IsBodyHtml = true
			};
			mailMessage.To.Add(destinatario);

			// Adjuntar el PDF al correo
			var tempFilePath = Path.GetTempFileName();
			File.WriteAllBytes(tempFilePath, pdfContent);

			var attachment = new Attachment(tempFilePath, "application/pdf");
			attachment.Name = "InformeNotificaciones.pdf"; // Nombre del archivo adjunto
			mailMessage.Attachments.Add(attachment);

			// Configurar el cliente SMTP
			var smtpClient = new SmtpClient(domain) // Servidor SMTP
			{
				Port = Convert.ToInt32(port), // Puerto SMTP
				Credentials = new NetworkCredential(user, password), // Credenciales
				EnableSsl = true, // Habilitar TLS
				DeliveryMethod = SmtpDeliveryMethod.Network,
				UseDefaultCredentials = false // Usar las credenciales proporcionadas
			};

			try
			{
				// Enviar el correo
				smtpClient.Send(mailMessage);
				Console.WriteLine("Correo enviado exitosamente.");
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error al enviar el correo: " + ex.Message);
			}
			finally
			{
				// Eliminar el archivo temporal
				//File.Delete(tempFilePath);
			}
		}
	}
}