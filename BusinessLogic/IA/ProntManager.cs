using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLogic.IA.Model;
using BusinessLogic.Rastreo.Model;
using BusinessLogic.SystemDocuments.Models;
using CAPA_DATOS;
using CAPA_NEGOCIO.IA;
using CAPA_NEGOCIO.MAPEO;

namespace BusinessLogic.IA
{
	public class ProntManager
	{
		public static string[] ValidCodes = Tbl_Servicios.GetServicesCodes(); /*{
			"RASTREO_Y_SEGUIMIENTOS",
			"INFORMACION_ENTREGAS_SEGUIMIENTOS",
			"INFORMACION_SOBRE_DOCUMENTOS",
			"QUEJAS_POR_RETRASOS",
			"QUEJAS_POR_IMPORTES",
			"QUEJAS_POR_ESTAFA",
			"CONSULTA_DE_HORARIOS",
			"CONSULTA_DE_CONTACTO",
			"EVENTOS",
			"SOLICITUD_DE_ASISTENCIA",
			"ASISTENCIA_GENERAL"
		};*/
		public static string ProntValidation(string? prontTypes)
		{
			//
			string pront = GetPront("SYSTEMPRONT")
				.Replace("{request_type}", prontTypes)
				.Replace("{rules}", GetPront(prontTypes))
				.Replace("{documents}", $"- {prontTypes}: {GetDocumentsPront(prontTypes)}" );
			Console.Write(pront);
			return pront;
		}

		private static string GetDocumentsPront(string? prontTypes)
		{
			var cat = new Category { Descripcion = prontTypes }.Find<Category>();
			var articles = new Article().Where<Article>(
				FilterData.In("Category_Id", cat?.Category_Id ?? -1)
			).ToList();
			var list = articles.Select(x => $"\n {articles.IndexOf(x) + 1} {x.Title}: {ExtractTextFromHtml(x.Body)}" ).ToList();
			return string.Join("\n - ", list);
		}

		private static string ExtractTextFromHtml(string? html)
		{
			var doc = new HtmlAgilityPack.HtmlDocument();
			doc.LoadHtml(html);
			var text = string.Join(" ", doc.DocumentNode.DescendantsAndSelf()
				.Where(n => n.NodeType == HtmlAgilityPack.HtmlNodeType.Text && !string.IsNullOrWhiteSpace(n.InnerText))
				.Select(n => n.InnerText.Trim()));
			text = text.Replace("&nbsp;", " ");
			return string.Join(" ", text.Split([' ', '\n', '\r'], StringSplitOptions.RemoveEmptyEntries));
		}

		public static string GetPront(string? prontTpe)
		{
			return string.Join("", new Tbl_Pronts
			{
				Type = prontTpe,
			}.Get<Tbl_Pronts>().Select(p => p.Pront_Line).ToList());
		}

		public static string CrearPrompt(string pregCliente, string? trakingNumber, List<TrackingHistory> historial, string ProntType)
		{
			return $@"{ProntAdapter(pregCliente)}		
							
			**Reglas obligatorias al contestar:**
				1- Contesta breve y directo, adaptándote como un agente virtual de correos de guatemala.
				2- no incluyas simulacros de conversaciones, solo responde de forma directa.
				3- responde en español con un parrafo corto.
				4- no incluyas tu contexto, reglas establecidas ni ejemplos en la respuesta
				5- no incluyas estas reglas";

		}

		public static string BuildTrakingResponse(string? trakingNumber, List<TrackingHistory> historial)
		{
			// Inicializar variables para la última ubicación y fecha
			string responseUltimaHubicacionText = string.Empty;
			string ultimaUbicacion = "Información no disponible";
			DateTime? ultimaFecha = null;
			string? numeroSeguimiento = trakingNumber;
			// Validar si el historial contiene datos
			if (historial != null && historial.Any())
			{
				// Ordenar por fecha descendente y tomar el primer registro
				var ultimaEntrada = historial.OrderByDescending(x => x.Fecha_Evento).FirstOrDefault();
				if (ultimaEntrada != null)
				{
					ultimaUbicacion = ultimaEntrada.Oficina_Destino;
					ultimaFecha = ultimaEntrada.Fecha_Evento;
					numeroSeguimiento = ultimaEntrada.Tracking;
				}
			}
			if (numeroSeguimiento != null && historial?.Count > 0)
			{
				responseUltimaHubicacionText = $@"Es un placer procesar tu solicitud, el numero de seguimiento  ""{numeroSeguimiento}"" es válido y esta en nuestro sistemas:				
	- Última ubicación registrada: {ultimaUbicacion}. {(ultimaFecha.HasValue ? $"(Fecha: {ultimaFecha.Value:dd/MM/yyyy HH:mm})" : "")}""
				";
			}
			else if (numeroSeguimiento != null && historial?.Count == 0)
			{
				responseUltimaHubicacionText = $@"El número de seguimiento ""{numeroSeguimiento}"" no es valido o no esta registrado en nuestro sistema, puede hacer la consulta más adelante por si hay algun retrazo en el registro del sistema y si esta seguro que el paquete fue enviado, favor verificar que haya digitado bien el número y en caso que este sea correcto, favor pongase en contacto con la persona que envio el paquete o con su proveedor";
			}
			return responseUltimaHubicacionText;
		}

		/*public static string ServicesEvaluatorPrompt(string text)
		{
			return GetPront("SERVICES_PRONT_VALIDATOR").Replace("{text}", text);
		}*/

		internal static string? GetSaludo()
		{
			return @"¡Hola! Bienvenido a Correos de Guatemala. Soy tu asistente virtual, listo para ayudarte. Por favor, cuéntame en qué puedo asistirte hoy.					
Puedes pedirme información sobre:
	1 - El estado y ubicación de tus paquetes, incluyendo rastreo, procesos en aduanas.
	2 - Quejas relacionadas con retrasos.
	3 - Quejas relacionadas impuestos.
	4 - Posibles estafas.
	5 - Información de contacto, horarios de nuestras oficinas o detalles sobre eventos y actividades.
	6 - Procedimientos de soporte técnico o los documentos necesarios para trámites.
	7 - Te puedo comunicar con un agente de servicio al cliente. 
	8 - Eventos. 
						
Por favor, selecciona una opción y cuéntame cómo puedo asistirte hoy.";
		}

		internal static string? Get_Cierre()
		{
			return "Es un placer poderte ayudar, si necesitas algo más no dudes en preguntar!";
		}
		public static string ProntAdapter(string? consulta)
		{
			if (consulta == "8")
			{
				return "¿puedo preguntar sobre los eventos de la oficina de correos?";
			}
			else if (consulta == "7")
			{
				return "SOLICITUD_DE_ASISTENCIA";
			}
			else if (consulta == "6")
			{
				return "¿Podrias informarme sobre los documentos para recepcionar paquetes?";
			}
			else if (consulta == "5")
			{
				return "¿Podrias informarme sobre los como contactarme con la oficina de correos y telegrafos?";
			}
			else if (consulta == "4")
			{
				return "quiero presentar una queja por estafa";
			}
			else if (consulta == "3")
			{
				return "quiero presentar una queja por importes no esperados";
			}
			else if (consulta == "2")
			{
				return "quiero presentar una queja por un retrazo en mi entrega";
			}
			else if (consulta == "1")
			{
				return "necesito información sobre la ubicación de mi paquete";
			}
			else
			{
				return consulta ?? "ASISTENCIA_GENERAL";
			}
		}
	}

}