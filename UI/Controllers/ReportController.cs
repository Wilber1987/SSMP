using Microsoft.AspNetCore.Mvc;
using CAPA_DATOS.Security;
using CAPA_NEGOCIO.Services;
using CAPA_DATOS.Services;
using BusinessLogic.Notificaciones_Mensajeria.Informe;
using DatabaseModelNotificaciones;
using CAPA_DATOS;

namespace API.Controllers
{
	[Route("api/[controller]/[action]")]
	[ApiController]
	public class ReportController : ControllerBase
	{
		[HttpPost]
		public IActionResult SenReportNotifications(ReportRequest reportRequest)
		{
			try
			{
				string body = $"REPORTE DE NOTIFICACIONES ENVIADAS DEL {reportRequest.FirstDate} AL {reportRequest.LastDate}";
				NotificationsReportBuilder.EnviarReporte([reportRequest.Mail], $"REPORTE DE NOTIFICACIONES ENVIADAS", body, reportRequest.Notificaciones);
				return StatusCode(200, new ResponseService
				{
					status= 200,
					message = "Informe enviado"
				});
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Error generating PDF: {ex.Message}");
			}
			// Aquí puedes usar el parámetro facturaId para realizar alguna operación			
		}
		public class ReportRequest
		{
			public DateTime FirstDate { get; set; }
			public DateTime LastDate { get; set; }
			public string? Mail { get; set; }
			public List<Notificaciones> Notificaciones { get; set; } = [];

		}

	}
}

