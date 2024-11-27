using System.Diagnostics;
using System.Security.AccessControl;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BusinessLogic.IA;
using BusinessLogic.IA.Model;
using BusinessLogic.Rastreo.Model;
using BusinessLogic.Rastreo.Operations;
using CAPA_DATOS;
using CAPA_NEGOCIO.MAPEO;
using MimeKit.Encodings;
using Newtonsoft.Json;
using static CAPA_NEGOCIO.MAPEO.Cat_Dependencias;

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
			string? trakingNumber = TrackingOperation.FindTrackingNumber(question.Text);
			var client = new HttpClient();
			question.Timestamp = DateTime.Now;

			// evalua tipo de caso
			string tipocaso = trakingNumber != null ? DefaultServices_DptConsultasSeguimientos.RASTREO_Y_SEGUIMIENTOS.ToString()
			 : ExtractAndValidateCode(EvaluaCaso(question).GetAwaiter().GetResult(), ProntManager.ValidCodes);

			question.TypeProcess = tipocaso;

			var dCaso = GestionaCaso(question);
			
			List<TrackingHistory> list = [];
			if (trakingNumber != null)
			{
				list = new TrackingHistory { Tracking = trakingNumber }.Get<TrackingHistory>();
				//tipocaso = DefaultServices_DptConsultasSeguimientos.RASTREO_Y_SEGUIMIENTOS.ToString();
				string responseUltimaHubicacionText = ProntManager.BuildTrakingResponse(trakingNumber, list);
				question.MessageIA = responseUltimaHubicacionText;
				return question;
			}
			else
			{
				// Crear el prompt estructurado para Ollama
				string prompt = ProntManager.CrearPrompt(question.Text, trakingNumber, list, tipocaso);

				//if (!EsConsultaEstadoPaquete(question.Text))
				//{
				//    return "Por favor, pregunta por el estado de tu paquete proporcionando un número de seguimiento.";
				//}
				//ProntTypes prontTypes = DefineProntType(dCaso);
				List<object> historialMensajes = GetHistoryMessage(question, dCaso, prompt, tipocaso);
				var requestBody = new
				{
					model = "phi3:3.8b", // Especifica el modelo
					messages = historialMensajes,
					stream = false, // Controla si el resultado debe ser transmitido (stream)
					temperature = 0.2, // Controla la creatividad de las respuestas
					frequency_penalty = 0.2, // Reduce repeticiones en las respuestas
					presence_penalty = 0.0, // Fomenta nuevas ideas sin desviarse del contexto
					max_tokens = 1 // Establece el número máximo de tokens permitidos en la respuesta
				};

				var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
				// Realiza la solicitud POST
				var response = await client.PostAsync("http://localhost:11434/api/chat", content);
				if (response.IsSuccessStatusCode)
				{
					// Si la solicitud es exitosa, lee la respuesta
					var result = await response.Content.ReadAsStringAsync();

					JsonResponse? jsonResponse = JsonConvert.DeserializeObject<JsonResponse>(result);
					string? messageContent = jsonResponse?.Message?.Content;
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
		}

		private static List<object> GetHistoryMessage(UserMessage question, Tbl_Case dCaso, string prompt, string prontTypes)
		{
			var historialMensajes = new List<object>
			{
				new { role = "system", content = ProntManager.ProntValidation(prontTypes)},
			};

			List<Tbl_Comments> tbl_Comments = new Tbl_Comments { Id_Case = dCaso.Id_Case }.Get<Tbl_Comments>();
			historialMensajes.AddRange(tbl_Comments.Select(c =>
				new
				{
					role = c.NickName == question.UserId ? "human" : "asistant",
					content = c.Body
				}
			));
			historialMensajes.Add(new { role = "human", content = prompt });
			return historialMensajes;
		}

		public static Tbl_Case GestionaCaso(UserMessage data)
		{
			try
			{
				string resp = "";
				string caseTitle = $"{data?.UserId} - {data?.Timestamp.ToString("yyyy-MM-dd")}";


				Tbl_Servicios? servicios = new Tbl_Servicios().Find<Tbl_Servicios>(FilterData.Equal("Descripcion_Servicio", data?.TypeProcess?.ToString()));

				Cat_Dependencias? dependencia = new Cat_Dependencias().Find<Cat_Dependencias>(FilterData.Equal("Id_Dependencia", servicios?.Id_Dependencia ?? -1));

				if (dependencia == null)
				{
					dependencia = new Cat_Dependencias
					{
						DefaultDependency = true
					}.Find<Cat_Dependencias>();
				}
				//filtra casoi especifico
				var instaCase = new Tbl_Case().Find<Tbl_Case>(FilterData.Equal("Titulo", caseTitle));

				if (instaCase != null)
				{
					if (dependencia?.DefaultDependency != true && instaCase.Cat_Dependencias?.DefaultDependency == true)
					{
						instaCase.Cat_Dependencias = dependencia;
						instaCase.Id_Dependencia = dependencia?.Id_Dependencia;
					}
					return instaCase;
				}
				else
				{
					var mimeMessageCaseData = new MimeMessageCaseData { PlatformType = data?.Source.ToUpper() };

					var newCase = new Tbl_Case()
					{
						Titulo = caseTitle,
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

		public async Task AddComment(Tbl_Case data, UserMessage interaction)
		{


			Tbl_Comments us = new Tbl_Comments()
			{
				Id_Case = data?.Id_Case,
				Body = interaction.Text,
				NickName = interaction.UserId,
				Fecha = interaction.Timestamp,
				Estado = CommetsState.Leido.ToString(),
				Mail = interaction.UserId,
			};
			us.Save();
			Tbl_Comments ia = new Tbl_Comments()
			{
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

			//string prompt = ProntManager.ServicesEvaluatorPrompt(question.Text);

			// Crea el contenido que se enviará en la solicitud
			var requestBody = new
			{
				model = "phi3:3.8b", // Aquí puedes especificar el modelo que deseas usar
				messages = new[]
				{
					new { role = "system", content =  ProntManager.GetPront("SERVICES_PRONT_VALIDATOR_CONTEXT")},
					new { role = "user", content = question.Text } // Se pasa el mensaje que el usuario envía
				},
				temperature = 0.0, // Controla la creatividad de las respuestas
				frequency_penalty = 0, // Reduce repeticiones en las respuestas
				presence_penalty = 0, // Fomenta nuevas ideas sin desviarse del contexto
				max_tokens = 1, // Establece el número máximo de tokens permitidos en la respuesta
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

		static string ExtractAndValidateCode(string botResponse, string[] validCodes)
		{
			// Expresión regular para encontrar palabras exactas que coincidan con los códigos válidos
			string pattern = string.Join("|", validCodes.Select(Regex.Escape));
			Match match = Regex.Match(botResponse, $@"\b({pattern})\b");

			if (match.Success)
			{
				return match.Value; // Retorna el código encontrado
			}
			else
			{
				return "ASISTENCIA_GENERAL"; // Retorna un código por defecto si no encuentra coincidencias
			}
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
