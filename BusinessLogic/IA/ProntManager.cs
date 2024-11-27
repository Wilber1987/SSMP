using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLogic.IA.Model;
using BusinessLogic.Rastreo.Model;
using CAPA_NEGOCIO.IA;

namespace BusinessLogic.IA
{
	public class ProntManager
	{
		 public static string[] ValidCodes = {
			"RASTREO_Y_SEGUIMIENTOS",
			"INFORMACION_ENTREGAS_SEGUIMIENTOS",
			"INFORMACION_SOBRE_DOCUMENTOS",
			"QUEJAS_POR_RETRASOS",
			"QUEJAS_POR_IMPORTES",
			"QUEJAS_POR_ESTAFA",
			"CONSULTA_DE_HORARIOS",
			"CONSULTA_DE_CONTACTO",
			"CONSULTA_SOBRE_EVENTOS",
			"SOLICITUD_DE_ASISTENCIA",
			"ASISTENCIA_GENERAL"
		};
		public static string ProntValidation(string? prontTpe)
		{
			return GetPront(prontTpe);

			/*return @"
				Eres un asistente virtual de soporte para la oficina de correos de Guatemala. 
				Tu tarea es proporcionar información clara y precisa sobre:
				- Estado y ubicación de paquetes (rastreo, aduanas, retrasos).
				- Quejas relacionadas con envíos (retrasos, impuestos, estafas).
				- Información de contacto, horarios y eventos.
				- Procedimientos de soporte técnico o documentación necesaria.

				Reglas:
				1. Responde en español y sé breve pero informativo.
				2. Evalúa sinónimos y frases relacionadas para identificar correctamente la categoría de la consulta.
				3. Si la consulta no encaja en una categoría específica, proporciona asistencia general respetando las políticas.
				4. Cuando sea necesario, solicita información adicional como números de rastreo válidos o datos específicos del cliente.
				5. Si el cliente menciona soporte técnico, pídele que escriba 'SOPORTE' para conectarlo con el departamento correspondiente.
			";*/

			//@"Eres un asistente virtual oficial de correos de Guatemala. Solo puedes proporcionar información basada en las políticas y datos proporcionados aquí. No especules ni improvises. Si no puedes responder dentro de este contexto, indica: 				'Lo siento, no puedo ayudarte con esa solicitud. Por favor, consulta el sitio oficial o llama al número de atención al cliente.' 				Responde en español, de forma clara y profesional. Reglas: 1. Los números de rastreo válidos contienen 2 letras, 9 números y 2 letras finales (ejemplo: AA123456789GT).2. Los horarios de atención son lunes a viernes, de 8:00 am a 5:00 pm. 3. Los pagos de impuestos deben realizarse en oficinas autorizadas. No se aceptan pagos a domicilio.	4. Reclamos por fraude deben dirigirse al departamento especializado al número +502 8888 8888.	Responde solo basándote en estas políticas. Si algo no está en estas reglas, informa que no puedes responder.";
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
			return GetPront(ProntType + "_RESPONSE").Replace("{pregCliente}", pregCliente);
			/*return $@"
				1- Pregunta cliente: ({pregCliente}).
				2- Verifica que la información que el cliente pregunta es específica sobre estados de paquete, rastreo o ubicación.				
				3- Contesta breve y directo, adaptándote como un agente virtual de una agencia de envíos de paquetes.
				4- En caso de problemas como identificador incorrecto o datos incompletos, indica que el numero de seguimiento es requerido o que debe especificar una consulta valida.
				5- Si se trata de una excepción (como envíos desde Taiwán), comunica las políticas.
				6- no des ningun tipo de ejemplo ni de conversacion ni de proceso.
				7- no incluyas simulacros de conversaciones, solo responde de forma directa.
				";*/


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

		public static string ServicesEvaluatorPrompt(string text)
		{
			return GetPront("SERVICES_PRONT_VALIDATOR").Replace("{text}", text);
			
			/*return $@"Por favor, evalúa el texto proporcionado por el cliente y responde únicamente con el código de clasificación correspondiente. Responde **solo** con el código, sin incluir ningún texto adicional.
				los codigos son los siguientes:
					- ""RASTREO_Y_SEGUIMIENTOS""
					- ""INFORMACION_ENTREGAS_SEGUIMIENTOS""
					- ""INFORMACION_SOBRE_DOCUMENTOS""
					- ""QUEJAS_POR_RETRASOS""
					- ""QUEJAS_POR_IMPORTES""
					- ""QUEJAS_POR_ESTAFA""
					- ""CONSULTA_DE_HORARIOS""
					- ""CONSULTA_DE_CONTACTO""
					- ""CONSULTA_SOBRE_EVENTOS""
					- ""SOLICITUD_DE_ASISTENCIA""
					- ""ASISTENCIA_GENERAL""

				Consulta inicial del cliente: ""{text}"".

				Categorías de respuesta:
				1. Si el cliente menciona el estado o ubicación de un paquete:
				Responde: ""RASTREO_Y_SEGUIMIENTOS"".

				2. Si el cliente menciona tiempos de espera o ubicación de entrega:
				Responde: ""INFORMACION_ENTREGAS_SEGUIMIENTOS"".

				3. Si el cliente pregunta por documentación necesaria:
				Responde: ""INFORMACION_SOBRE_DOCUMENTOS"".

				4. Si el cliente menciona retrasos o quejas sobre entregas tardías:
				Responde: ""QUEJAS_POR_RETRASOS"".

				5. Si el cliente menciona costos adicionales o impuestos inesperados:
				Responde: ""QUEJAS_POR_IMPORTES"".

				6. Si el cliente menciona estafas, robos, fraudes o envíos incompletos:
				Responde: ""QUEJAS_POR_ESTAFA"".

				7. Si el cliente pregunta por horarios de atención:
				Responde: ""CONSULTA_DE_HORARIOS"".

				8. Si el cliente pregunta por números de contacto o sitios web:
				Responde: ""CONSULTA_DE_CONTACTO"".

				9. Si el cliente menciona eventos especiales en correos:
				Responde: ""CONSULTA_SOBRE_EVENTOS"".

				10. Si el cliente desea hablar con soporte técnico o un agente:
					Responde: ""SOLICITUD_DE_ASISTENCIA"".

				11. Si no puedes identificar ninguna de las categorías anteriores:
					Responde: ""ASISTENCIA_GENERAL"".

				Ejemplo de respuestas:
				- ""RASTREO_Y_SEGUIMIENTOS""
				- ""INFORMACION_ENTREGAS_SEGUIMIENTOS""
				- ""INFORMACION_SOBRE_DOCUMENTOS""
				- ""QUEJAS_POR_RETRASOS""
				- ""QUEJAS_POR_IMPORTES""
				- ""QUEJAS_POR_ESTAFA""
				- ""CONSULTA_DE_HORARIOS""
				- ""CONSULTA_DE_CONTACTO""
				- ""CONSULTA_SOBRE_EVENTOS""
				- ""SOLICITUD_DE_ASISTENCIA""
				- ""ASISTENCIA_GENERAL""
			"; */
		}
	}
}