using System.Text;
using System.Text.RegularExpressions;
using BusinessLogic.IA;
using BusinessLogic.Rastreo.Model;
using BusinessLogic.Rastreo.Operations;
using CAPA_DATOS;
using CAPA_NEGOCIO.MAPEO;
using DataBaseModel;
using Newtonsoft.Json;

namespace CAPA_NEGOCIO
{
	public class LlamaClient
	{
		public async Task<UserMessage> GenerateResponse(UserMessage question)
		{
			try
			{
				string caseTitle = $"{question?.UserId} - {question?.Timestamp.ToString("yyyy-MM-dd")}";
				var instaCase = new Tbl_Case().Find<Tbl_Case>(FilterData.Equal("Titulo", caseTitle));

				if (instaCase != null && instaCase.MimeMessageCaseData?.WithAgent == true)
				{
					question!.WithAgentResponse = true;
					question.Id_case = instaCase.Id_Case;
					instaCase.MimeMessageCaseData!.WithAgent = true;
					await AddComment(instaCase, question);
					return question;
				}
				string? trakingNumber = TrackingOperation.FindTrackingNumber(question.Text);

				question.Timestamp = DateTime.Now;

				// evalua tipo de caso
				string tipocaso = trakingNumber != null ? DefaultServices_DptConsultasSeguimientos.RASTREO_Y_SEGUIMIENTOS.ToString()
				 : CaseEvaluatorManager.DeterminarCategoria(question.Text);
				 // ExtractAndValidateCode(EvaluaCaso(question).GetAwaiter().GetResult(), ProntManager.ValidCodes);

				question.TypeProcess = tipocaso;

				var dCaso = GestionaCaso(question, caseTitle, instaCase);

				List<TrackingHistory> list = [];
				if (trakingNumber != null)
				{
					list = new TrackingHistory { Tracking = trakingNumber }.Get<TrackingHistory>();
					//tipocaso = DefaultServices_DptConsultasSeguimientos.RASTREO_Y_SEGUIMIENTOS.ToString();
					string responseUltimaHubicacionText = ProntManager.BuildTrakingResponse(trakingNumber, list);
					question.MessageIA = responseUltimaHubicacionText;
					question.Id_case = dCaso.Id_Case;
					question.WithAgentResponse = false;
					await AddComment(dCaso, question, true);
					return question;
				} 
				else if (tipocaso == "SALUDOS")
				{ 
					question.MessageIA = ProntManager.GetSaludo();
					question.Id_case = dCaso.Id_Case;
					await AddComment(dCaso, question);					
					return question;
				}
				else if (tipocaso == "CIERRE_DE_CASO")
				{ 
					question.MessageIA = ProntManager.Get_Cierre();
					question.Id_case = dCaso.Id_Case;					
					await AddComment(dCaso, question);	
					dCaso.Estado = Case_Estate.Finalizado.ToString();	
					dCaso.Update();			
					return question;
				}
				else if (tipocaso == "SOLICITUD_DE_ASISTENCIA")
				{
					question.MessageIA = "Con mucho gusto te comunicare con un asistente de servicio al cliente, por favor espera en linea";
					question.WithAgentResponse = true;
					question.Id_case = dCaso.Id_Case;
					dCaso.MimeMessageCaseData!.WithAgent = true;
					await AddComment(dCaso, question);
					dCaso.Update();
					return question;
				}
				else
				{
					// Crear el prompt estructurado para Ollama
					string prompt = ProntManager.CrearPrompt(question.Text, trakingNumber, list, tipocaso);
					List<object> historialMensajes = GetHistoryMessage(question, dCaso, prompt, tipocaso);
					object requestBody = BuildLLamaConfig(historialMensajes);
					var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
					// Realiza la solicitud POST
					HttpResponseMessage response = await GetIAResponse(content);
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
			catch (Exception ex)
			{
				LoggerServices.AddMessageError($"ERROR: GenerateResponse", ex);
				throw ex;
			}

		}

		private static object BuildLLamaConfig(List<object> historialMensajes)
		{
			var requestBody = new
			{
				model = SystemConfig.AppConfigurationValue(AppConfigurationList.IAServices, "Model"), // Especifica el modelo
				messages = historialMensajes,
				stream = false, // Controla si el resultado debe ser transmitido (stream)
				temperature = 0.7, // Controla la creatividad de las respuestas
				frequency_penalty = 0.2, // Reduce repeticiones en las respuestas
				presence_penalty = 0.2, // Fomenta nuevas ideas sin desviarse del contexto
				//max_tokens = 1 // Establece el número máximo de tokens permitidos en la respuesta
			};
			return requestBody;
		}

		private static List<object> GetHistoryMessage(UserMessage question, Tbl_Case dCaso, string prompt, string prontTypes)
		{
			//Console.Write(ProntManager.ProntValidation(prontTypes));
			var historialMensajes = new List<object>
			{
				new { role = "system", content = ProntManager.ProntValidation(prontTypes)},
			};

			List<Tbl_Comments> tbl_Comments = new Tbl_Comments { Id_Case = dCaso.Id_Case }.Get<Tbl_Comments>();
			historialMensajes.AddRange(tbl_Comments.Select(c =>
				new
				{
					role = c.NickName == question.UserId ? "user" :  (c.NickName == "traking_system" ? "traking_system" : "asistant") ,
					content = c.Body
				}
			));
			historialMensajes.Add(new { role = "user", content = prompt });
			return historialMensajes;
		}

		public static Tbl_Case GestionaCaso(UserMessage data, string caseTitle, Tbl_Case? instaCase)
		{
			try
			{
				Tbl_Servicios? servicios = new Tbl_Servicios().Find<Tbl_Servicios>(FilterData.Equal("Descripcion_Servicio", data?.TypeProcess?.ToString()));
				Cat_Dependencias? dependencia = new Cat_Dependencias().Find<Cat_Dependencias>(FilterData.Equal("Id_Dependencia", servicios?.Id_Dependencia ?? -1));

				if (dependencia == null)
				{
					dependencia = new Cat_Dependencias
					{
						DefaultDependency = true
					}.Find<Cat_Dependencias>();
				}
				if (instaCase != null)
				{
					if (dependencia?.DefaultDependency != true && instaCase.Cat_Dependencias?.DefaultDependency == true)
					{
						instaCase.Cat_Dependencias = dependencia;
						instaCase.Id_Dependencia = dependencia?.Id_Dependencia;
						instaCase.Update();
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

		public async Task AddComment(Tbl_Case data, UserMessage interaction, bool isSystem = false)
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
			if (interaction.MessageIA != null)
			{
				if (isSystem)	 
				{
					Tbl_Comments ia = new Tbl_Comments()
					{
						Id_Case = data?.Id_Case,
						Body = interaction.MessageIA,
						NickName = "traking_system",
						Fecha = DateTime.Now,
						Estado = CommetsState.Leido.ToString(),
						Mail = "traking_system@soporte.net",
					};
					ia.Save();
				} else 
				{
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
				
			}

		}

		public async Task<string> EvaluaCaso(UserMessage question)
		{

			//string prompt = ProntManager.ServicesEvaluatorPrompt(question.Text);
			List<object> historialMensajes = [
				new { role = "system", content =  ProntManager.GetPront("SERVICES_PRONT_VALIDATOR_CONTEXT")},
				new { role = "user", content = question.Text } // Se pasa el mensaje que el usuario envía
			];
			object requestBody = BuildLLamaConfig(historialMensajes);
			var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
			HttpResponseMessage response = await GetIAResponse(content);
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

		private static async Task<HttpResponseMessage> GetIAResponse(StringContent content)
		{
			var client = new HttpClient
			{
				Timeout = TimeSpan.FromMinutes(10) // Ajusta según sea necesario
			};
			// Realiza la solicitud POST
			var response = await client.PostAsync(SystemConfig.AppConfigurationValue(AppConfigurationList.IAServices, "IAHost"), content);
			return response;
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
	}

}
