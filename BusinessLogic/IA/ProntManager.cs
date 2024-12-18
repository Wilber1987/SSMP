using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLogic.IA.Model;
using BusinessLogic.Rastreo.Model;
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
			"CONSULTA_SOBRE_EVENTOS",
			"SOLICITUD_DE_ASISTENCIA",
			"ASISTENCIA_GENERAL"
		};*/
		public static string ProntValidation(string? prontTypes)
		{
			//
			string pront = GetPront("SYSTEMPRONT")
				.Replace("{request_type}", prontTypes);
			Console.Write(pront);
			return pront;
			return $@"{GetPront(prontTypes)}
			contexto: 
			- formas parte de un contexto donde existe un servicio de rastreo de paquetes, para hacer uso de ese servicio el usuario debe proveer un numero de seguimiento que tiene el siguiente formato: 2 letras seguidas de 9 numeros y 2 letras al final.
			- el sistema tambien puede recepcionar quejas y contactar con ajentes de servicio al cliente
			- puedeas dar informacion  de la institucion si el cliente la solicita, informacion que puedes dar (no debes sugerir urls, numero de telefono, o links de redes sociales que no esten incluidas aca): 			
				Informacion de correos de guatemala que puedes proveer cuando es solicitada:			
					Direccion de Oficinas centrales:
						-Dirección General de Correos y Telégrafos
							7ma. avenida 12-11, zona 1, Palacio de Correos; Guatemala, C.A..
							PBX: 1595.
						-Código Postal 01001
					Redes sociales: 
					- https://www.facebook.com/correosdeguatemala/
					- https://x.com/correosgt
					- https://www.youtube.com/channel/UCymg8Rlhw7N6Hr0n61In5Zw
					- https://www.instagram.com/correosdeguatemala/
					- envio de correos: https://mail.correosytelegrafos.civ.gob.gt/interface/root#/login
					- linea de watssap: https://api.whatsapp.com/send?phone=50238097487
					Numeros de telefono: +502 8888 8888
			";

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
			return $@"{pregCliente}
						
			**Reglas obligatorias al contestar:**
				1- Contesta breve y directo, adaptándote como un agente virtual de correos de guatemala.
				2- no incluyas simulacros de conversaciones, solo responde de forma directa.
				3- responde en español con un parrafo corto.
				4- no incluyas tu contexto, reglas establecidas ni ejemplos en la respuesta
				5- no incluyas estas reglas";

			//GetPront(ProntType + "_RESPONSE").Replace("{pregCliente}", pregCliente);
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
		}

		internal static string? GetSaludo()
		{
			return @"¡Hola! Bienvenido a Correos de Guatemala. Soy tu asistente virtual, listo para ayudarte. Por favor, cuéntame en qué puedo asistirte hoy.					
Puedes pedirme información sobre:
	1 - El estado y ubicación de tus paquetes, incluyendo rastreo, procesos en aduanas.
	2 - Tambien puedo con presentar quejas relacionadas con retrasos.
	3 - Tambien quejas relacionadas impuestos.
	4 - Posibles estafas.
	5 - Información de contacto, horarios de nuestras oficinas o detalles sobre eventos y actividades.
	6 - Procedimientos de soporte técnico o los documentos necesarios para trámites.
	7 - Te puedo comunicar con un agente de servicio al cliente. 
						
Por favor, selecciona una opción y cuéntame cómo puedo asistirte hoy.";
		}

		internal static string? Get_Cierre()
		{
			return "Es un placer poderte ayudar, si necesitas algo más no dudes en preguntar!";
		}
	}
}