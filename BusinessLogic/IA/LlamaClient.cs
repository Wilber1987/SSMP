﻿using System.Diagnostics;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.Rastreo.Model;
using BusinessLogic.Rastreo.Operations;
using CAPA_DATOS;
using CAPA_NEGOCIO.MAPEO;
using MimeKit.Encodings;
using Newtonsoft.Json;

namespace CAPA_NEGOCIO
{
	public class LlamaClient
	{
        public static object PlatformType { get; private set; }

        public LlamaClient()
		{

		}

		public async Task<UserMessage> GenerateResponse(UserMessage question)
		{

			var client = new HttpClient();
			question.Timestamp = DateTime.Now;  

			// evalua tipo de caso
			string tipocaso = EvaluaCaso(question).GetAwaiter().GetResult();
			question.TypeProcess = tipocaso;

			var dCaso = GestionaCaso(question);
			string? trakingNumber = TrackingOperation.FindTrackingNumber(question.Text);
			List<TrackingHistory> list = [];
			if (trakingNumber != null)
			{
				list = new TrackingHistory{ Tracking = trakingNumber}.Get<TrackingHistory> ();
			}
			
			// Crear el prompt estructurado para Ollama
			string prompt = CrearPrompt(question.Text, list);

			//if (!EsConsultaEstadoPaquete(question.Text))
			//{
			//    return "Por favor, pregunta por el estado de tu paquete proporcionando un número de seguimiento.";
			//}

			// Crea el contenido que se enviará en la solicitud
			var requestBody = new
			{
				model = "phi3:3.8b", // Aquí puedes especificar el modelo que deseas usar
				messages = new[]
				{
					new { role = "system", content = "Por favor, solo responde sobre el estado de los paquetes en tránsito, " +
					"proporcionando información clara. No des información sobre otros temas. " +
					"Solo responder en español. Si no entiendes lo que te pregunta el user solo contesta en que puedo ayudarte." +
					"No especifiques el mensaje de cliente en tu respuesta." +
					"No contestes nada fuera de este contexto. " +
					"Si el usuario manda preguntas ambiguas, solo contesta y pide la informacion necesaria para evaliar el estado o ubicacion del paquete" +
					"Responde con claridad, se especifico al estado y ubicacion del paquete, no describas nada mas" },
					new { role = "user", content = prompt } // Se pasa el mensaje que el usuario envía
				},
				stream = false // Controla si el resultado debe ser transmitido (stream)
			};

			var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

			// Realiza la solicitud POST
			var response = await client.PostAsync("http://localhost:11434/api/chat", content);

			if (response.IsSuccessStatusCode)
			{
				// Si la solicitud es exitosa, lee la respuesta
				var result = await response.Content.ReadAsStringAsync();

				var jsonResponse = JsonConvert.DeserializeObject<JsonResponse>(result);
				string messageContent = jsonResponse.Message.Content;
				question.MessageIA = messageContent;

				//guarda interaccion de mensaje
				await AddComment(dCaso, question);
				question.Id_case = dCaso.Id_Case;

				return question; // Devuelve la respuesta del modelo Llama
			}
			else
			{	
				question.MessageIA = "Error al procesar la solicitud.";
				return question; // Devuelve un mensaje de error si no fue exitosa
			}

		}

		public static Tbl_Case GestionaCaso(UserMessage data) {
			try
			{
				string resp = ""; 

				int proc = -1;
				int.TryParse(data.TypeProcess,out proc);

				Tbl_Servicios? servicios = new Tbl_Servicios().Find<Tbl_Servicios>( FilterData.Equal("Id_Servicio", proc));

				Cat_Dependencias? dependencia = new Cat_Dependencias().Find<Cat_Dependencias>(FilterData.Equal("Id_Dependencia", servicios?.Id_Dependencia ?? -1));

				if (dependencia == null)
				{
					dependencia = new Cat_Dependencias{
						DefaultDependency = true
					}.Find<Cat_Dependencias>();
				}
				//filtra casoi especifico
				var instaCase = new Tbl_Case().Find<Tbl_Case>(FilterData.Equal("Titulo", data.UserId + data.Timestamp.ToString("yyyy-MM-dd")));

				if (instaCase != null)
				{
					return instaCase;
				}
				else
				{
					var mimeMessageCaseData = new MimeMessageCaseData { PlatformType = data?.Source };

                    var newCase = new Tbl_Case() { 
						Titulo = data.UserId + data.Timestamp.ToString("yyyy-MM-dd"),
						Descripcion = data.Text,
						Estado = Case_Estate.Activo.ToString(),
						Fecha_Inicio = data.Timestamp,
						Id_Dependencia = dependencia?.Id_Dependencia,
						Id_Servicio = servicios?.Id_Servicio,
						MimeMessageCaseData = mimeMessageCaseData
                    }; 
				  
					newCase.CreateAutomaticCaseIA(data);

					return newCase;
				}
			   
			}
			catch (Exception)
			{

				throw;
			}
		}

		public async Task AddComment(Tbl_Case data, UserMessage interaction){
			
			
			Tbl_Comments us = new Tbl_Comments()	{
				Id_Case = data?.Id_Case,
				Body = interaction.Text,
				NickName = interaction.UserId,
				Fecha = interaction.Timestamp,
				Estado = CommetsState.Leido.ToString(),
				Mail = interaction.UserId,
			};

			us.Save();

			Tbl_Comments ia = new Tbl_Comments(){
				Id_Case = data?.Id_Case,
				Body = interaction.MessageIA,
				NickName = "IA",
				Fecha = DateTime.Now,
				Estado = CommetsState.Leido.ToString(),
				Mail = "IA@soporte.net",
			};

			ia.Save();
		}

		public async Task<string> EvaluaCaso(UserMessage question)
		{
			var client = new HttpClient();

			string prompt = ConsultaCasoPrompt(question.Text);
			
			// Crea el contenido que se enviará en la solicitud
			var requestBody = new
			{
				model = "phi3:3.8b", // Aquí puedes especificar el modelo que deseas usar
				messages = new[]
				{
					new { role = "system", content = "Por favor, solo evalua el texto sobre el estado de los paquetes, proporcionando información clara y detallada relacionada con la ubicación o estado de envío. No des información sobre otros temas. Solo responder en español." },
					new { role = "user", content = prompt } // Se pasa el mensaje que el usuario envía
				},
				stream = false // Controla si el resultado debe ser transmitido (stream)
			};

			var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

			// Realiza la solicitud POST
			var response = await client.PostAsync("http://localhost:11434/api/chat", content);

			if (response.IsSuccessStatusCode)
			{
				// Si la solicitud es exitosa, lee la respuesta
				var result = await response.Content.ReadAsStringAsync();

				var jsonResponse = JsonConvert.DeserializeObject<JsonResponse>(result);
				string messageContent = jsonResponse.Message.Content;

				return messageContent; // Devuelve la respuesta del modelo Llama
			}
			else
			{
				return "Error al procesar la solicitud."; // Devuelve un mensaje de error si no fue exitosa
			}

		}

        private string CrearPrompt(string pregCliente, List<TrackingHistory> historial)
        {
            // Inicializar variables para la última ubicación y fecha
            string ultimaUbicacion = "Información no disponible";
            DateTime? ultimaFecha = null;
			string? numeroSeguimiento = null;
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

            // Construir el prompt basado en los datos del historial y el número de seguimiento
            return $@"
        1- Pregunta cliente: ({pregCliente}).
        2- Verifica que la información que el cliente pregunta es específica sobre estados de paquete, rastreo o ubicación.
        3- Si el número de seguimiento no es nulo ({numeroSeguimiento}) es válido:
            - Última ubicación registrada: {ultimaUbicacion}.
            {(ultimaFecha.HasValue ? $"(Fecha: {ultimaFecha.Value:dd/MM/yyyy HH:mm})" : "")}
        4- Si el número de seguimiento no es válido o no se proporciona, solicita al cliente el identificador único del paquete.
        5- Contesta breve y directo, adaptándote como un agente virtual de una agencia de envíos de paquetes.
        6- En caso de problemas como identificador incorrecto o datos incompletos, indica las políticas de identificación necesarias.
        7- Si se trata de una excepción (como envíos desde Taiwán), comunica las políticas.
    ";
        }


        private string ConsultaCasoPrompt(string text)
		{
			// Crear la prompt basada en el flujo de trabajo de consulta de paquetes
			return $@"
Por favor, solo evalua el texto en una conversación de seguimiento de un cliente 
donde se presentan varios desafíos relacionados al estado actual del paquete pendiente de envio, enviado, recibido. 
La pregunta inicial del cliente es: ""{text}"". 
Considera las siguientes instrucciones adicionales para responder de manera efectiva pero simplificada:
Evalúa si la consulta del cliente es sobre Estado o Ubicación . 

1- Si se trata solo del estado, porque el cliente mencionó específicamente un estado. Responde Codigo clasificacion ""1"".
Si la respuesta es ""1"", responde solo ""1"".

2- Considerando que la pregunta implica ambas cosas, Responde Codigo clasificacion ""2"".
Si la respuesta es ""2"", responde solo ""2"".

3- Además de las dos partes, también reconoce cualquier indicio que sugiera urgencia en la pregunta del cliente 
como un caso urgente y que indique atención prioritaria como quiero saber la ubicacion ya. Responde Codigo clasificacion ""3"".
Si la respuesta es ""3"", responde solo ""3"".

4- Si la consulta incluye más preguntas o pedidos adicionales por parte del cliente que no se relacionan 
con el estado actual y/o ubicación, Responde Codigo clasificacion ""4"". 
Si la respuesta es ""4"", responde solo ""4"".

5- Asegúrate de no proporcionar información técnica y ninguna informacion que no sea relevante a esta clasificacion de caso
de acuerdo a la pregunta inicial del cliente.

6- No incluyas información sobre procedimientos generales, tiempo estimado de entrega ni políticas internas, y nada que tenga que ver con descipcion 
extra sobre lo que se pregunta, especifica solo el codigo de clasificacion del texto. 

";
		}

		// Método para analizar el mensaje y determinar si es una consulta válida sobre el estado del paquete
		public bool EsConsultaEstadoPaquete(string mensaje)
		{
			// Definir una lista de palabras clave que indican una consulta sobre el estado de un paquete
			var palabrasClave = new[] { "paquete", 
										"estado", 
										"seguimiento", 
										"dónde está mi paquete", 
										"ubicación", 
										"rastreo" };

			// Convertir el mensaje a minúsculas para facilitar la búsqueda
			mensaje = mensaje.ToLower();

			// Verificar si alguna palabra clave está presente en el mensaje
			return palabrasClave.Any(palabra => mensaje.Contains(palabra));
		}
	}

}
