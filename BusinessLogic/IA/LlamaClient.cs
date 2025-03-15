using System.Text;
using System.Text.RegularExpressions;
using BusinessLogic.IA;
using BusinessLogic.IA.RequestEvaluator;
using BusinessLogic.Rastreo.Model;
using BusinessLogic.Rastreo.Operations;
using CAPA_DATOS;
using CAPA_NEGOCIO.MAPEO;
using DataBaseModel;
using DatabaseModelNotificaciones;
using Newtonsoft.Json;

namespace CAPA_NEGOCIO
{
	public class LlamaClient
	{
		public async Task<UserMessage> GenerateResponse(UserMessage question)
		{
			try
			{
				bool IsInBlackList = BlackListServices.IsInBlackList(question.UserId);
				if (!IsInBlackList)
				{
					var ex = new Exception($"Usuario en lista negra {question.Source} - {question.UserId}");
					LoggerServices.AddMessageError($"ERROR:" + ex.Message, ex);
					throw ex;
				}
				bool isWithIaResponse = true;
				bool isValidProcess = true;
				if (question.IsMetaWhatsAppApi)
				{
					string? iaMetaIdentification = SystemConfig.AppConfigurationValue(AppConfigurationList.MettaApi, "IANumberIdentification");
					string? metaIdentification = SystemConfig.AppConfigurationValue(AppConfigurationList.MettaApi, "NumberIdentification");
					isWithIaResponse = question.ServicesIdentification == iaMetaIdentification;
					isValidProcess = question.ServicesIdentification == iaMetaIdentification || question.ServicesIdentification == metaIdentification;
				}
				if (question.IsMessenger)
				{
					string? iaMetaIdentification = SystemConfig.AppConfigurationValue(AppConfigurationList.MettaApi, "IAMessengerNumberIdentification");
					string? metaIdentification = SystemConfig.AppConfigurationValue(AppConfigurationList.MettaApi, "MessengerNumberIdentification");
					isWithIaResponse = question.ServicesIdentification == iaMetaIdentification;
					isValidProcess = question.ServicesIdentification == iaMetaIdentification || question.ServicesIdentification == metaIdentification;
				}
				if (!isValidProcess)
				{
					var ex = new Exception($"Plataforma invalida {question.Source} - {question.ServicesIdentification}");
					LoggerServices.AddMessageError($"ERROR:" + ex.Message, ex);
					throw ex;
				}
				question.Text = question.Text?.Trim();
				question.IsWithIaResponse = isWithIaResponse;

				Notificaciones? notificacion = new Notificaciones().Find<Notificaciones>(
					FilterData.Like("Telefono", question.UserId?.Replace("+", "")),
					FilterData.Equal("Enviado", true)
				);
				//PROCESA LA SOLICITUD Y VERIFICA QUE NO HAYA UNA NBOTIFICACION PENDIENTE DE EVALUAR
				if (notificacion != null)
				{
					(bool flowControl, UserMessage value) = await ProcessWhenIxistsNotification(question, notificacion);
					if (!flowControl)
					{
						return value;
					}
				}

				//PROCESAMIENTO CUANDO NO EXISTE UNA  NOTIFICACION PENDIENTE
				string caseTitle = $"{question?.UserId} - {(isWithIaResponse ? " WithBot" : "")} - {question?.Timestamp?.ToString("yyyy-MM-dd")} ";
				var instaCase = new Tbl_Case().Find<Tbl_Case>(
					FilterData.Equal("Titulo", caseTitle),
					FilterData.Equal("Estado", Case_Estate.Activo)
				);
				//PROCESA EL EMNSAJE Y NO AGREGA CONTESTACION DEL BOT
				if (instaCase != null && (instaCase.MimeMessageCaseData?.WithAgent == true || !isWithIaResponse))
				{
					question!.WithAgentResponse = true;
					question.Id_case = instaCase.Id_Case;
					instaCase.MimeMessageCaseData!.WithAgent = true;
					await AddComment(instaCase, question);
					return question;
				}

				//EVALUACION DE TRAKING
				string? trakingNumber = TrackingOperation.FindTrackingNumber(question.Text);

				(bool isInvalidTraking, string? invalidTraking) =
					TrackingOperation.FindInvalidCode(question.Text);

				question.Timestamp = DateTime.Now;

				string tipocaso = GetCaseType(question, instaCase, trakingNumber, isWithIaResponse);
				question.TypeProcess = tipocaso;

				var dCaso = GestionaCaso(question, caseTitle, instaCase, isWithIaResponse);

				(bool isNegationTrakingRequest, string? invalidNegationTrakingResponse) =
					NegacionTraking.ProcesarNegationTrackingResponse(question.Text, tipocaso, isInvalidTraking);

				List<TrackingHistory> list = [];


				#region RESPUESTAS CONTROLADAS
				if (!isWithIaResponse)
				{
					question!.WithAgentResponse = true;
					question.Id_case = dCaso.Id_Case;
					dCaso.MimeMessageCaseData!.WithAgent = true;
					dCaso.Update();
					await AddComment(dCaso, question);
					return question;
				}
				else if (trakingNumber != null)
				{
					var traking = new TrackingHistory { Tracking = trakingNumber };
					traking.TestConection();
					list = traking.Get<TrackingHistory>();
					//tipocaso = DefaultServices_DptConsultasSeguimientos.RASTREO_Y_SEGUIMIENTOS.ToString();
					string responseUltimaHubicacionText = ProntManager.BuildTrakingResponse(trakingNumber, list);
					question.MessageIA = responseUltimaHubicacionText;
					question.Id_case = dCaso.Id_Case;
					question.WithAgentResponse = false;
					await AddComment(dCaso, question, true);
					return question;
				}
				else if (isInvalidTraking)
				{
					question.MessageIA = ProntManager.GetInvalidTrackingResponse(invalidTraking);
					question.Id_case = dCaso.Id_Case;
					await AddComment(dCaso, question);
					return question;
				}
				else if (isNegationTrakingRequest)
				{
					question.MessageIA = invalidNegationTrakingResponse;
					question.Id_case = dCaso.Id_Case;
					await AddComment(dCaso, question);
					return question;
				}
				//CUANDO SE ESTA INICANDO UNA SOLICITUD
				else if (tipocaso == "INICIO" || question.Text.ToUpper() == "MENU")
				{
					question.MessageIA = ProntManager.GetSaludo();
					question.Id_case = dCaso.Id_Case;
					await AddComment(dCaso, question);
					return question;
				}
				//CUANDO SE ESTA CERRANDO UNA SOLICITUD
				else if (tipocaso == "CIERRE_DE_CASO")
				{
					question.MessageIA = ProntManager.Get_Cierre();
					question.Id_case = dCaso.Id_Case;
					await AddComment(dCaso, question);
					dCaso.Estado = Case_Estate.Finalizado.ToString();
					dCaso.Update();
					return question;
				}
				//CUANDO PIDEN ATENCION AL CLIENTE
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
				#endregion
				//CUANDO ENTRA LA IA REALMENTE
				else
				{
					string? automaticResponse = ProntManager.GetAutomaticResponse(question.Text);
					if (automaticResponse != null)
					{
						question.MessageIA = automaticResponse;
						question.Id_case = dCaso.Id_Case;
						await AddComment(dCaso, question);
						return question;
					}
					if (tipocaso == "RASTREO_Y_SEGUIMIENTOS")
					{
						(bool isProcesed, string? response) = TrakingUnindictableRequest.ProcessRequest();
						if (isProcesed)
						{
							question.MessageIA = response;
							question.Id_case = dCaso.Id_Case;
							await AddComment(dCaso, question);
							return question;
						}
					}
					else
					{
						(bool isProcesed, string? response) = UnindictableRequest.ProcessRequest();
						if (isProcesed)
						{
							question.MessageIA = response;
							question.Id_case = dCaso.Id_Case;
							await AddComment(dCaso, question);
							return question;
						}
					}

					return await iAProcess(question, trakingNumber, tipocaso, dCaso, list);
				}
			}
			catch (Exception ex)
			{
				LoggerServices.AddMessageError($"ERROR: GenerateResponse", ex);
				throw;
			}
		}

		private async Task<UserMessage> iAProcess(UserMessage question, string? trakingNumber, string tipocaso, Tbl_Case dCaso, List<TrackingHistory> list)
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

		private static string GetCaseType(UserMessage question, Tbl_Case? instaCase, string? trakingNumber, bool isWithIaResponse)
		{
			if (!isWithIaResponse)
			{
				return "SOLICITUD_DE_ASISTENCIA";
			}
			// evalua tipo de caso
			return instaCase == null ? "INICIO" : trakingNumber != null
			? DefaultServices_DptConsultasSeguimientos.RASTREO_Y_SEGUIMIENTOS.ToString()
			: CaseEvaluatorManager.DeterminarCategoria(question.Text, instaCase?.Tbl_Servicios?.Descripcion_Servicio);
		}

		private async Task<(bool flowControl, UserMessage value)> ProcessWhenIxistsNotification(UserMessage question, Notificaciones notificacion)
		{
			var NotificationCase = new Tbl_Case().Find<Tbl_Case>(
									FilterData.Like("Titulo", $"Notificación de paquete {notificacion.NotificationData?.NumeroPaquete}"),
									FilterData.Equal("Estado", Case_Estate.Finalizado)
								);
			if (NotificationCase == null && (question.Text == "1" || question.Text == "2" || question.Text == "3"))
			{
				string title = $@"Notificación de paquete {notificacion.NotificationData?.NumeroPaquete} ({(question.Text == "1" ? "No utiliza servicio de desaduanaje" : question.Text == "2" ? "Utiliza servicio de desaduanaje" : "Paquete retirado")})";
				Tbl_Case? tbl_Case = new Tbl_Case().CreateAutomaticCaseNotification(notificacion, title);
				question.MessageIA = question.Text == "1" || question.Text == "3" ? "Es un placer servirte, ¿te puedo ayudar en algo más?" : "Con mucho gusto atenderemos te petición";
				//question.WithAgentResponse = true;	
				question.Text = question.Text == "1" ? "Visitar el Palacio de Correos" : "Utilizar servicio de desaduanaje";
				question.Id_case = tbl_Case?.Id_Case;
				await AddComment(tbl_Case, question);
				return (flowControl: false, value: question);
			}
			else if (NotificationCase == null && question.Text != "1" && question.Text != "2" && question.Text != "3")
			{
				question.MessageIA = $@"Tu paquete no. {notificacion?.NotificationData?.NumeroPaquete} esta a la espera de ser retirado, Si en caso se te dificulta venir, te ofrecemos el servicio de desaduanaje remoto el cual consiste en realizar, con tu debida autorización, todos los procesos por ti y entregarte el paquete en la dirección consignada en el mismo. 

¿Qué opción has decidido? 
1. Visitarnos en el Palacio de Correos
2. Utilizar nuestro servicio de desaduanaje
3. Ya lo retiré

Selecciona una opción";
				return (flowControl: false, value: question);
			}

			return (flowControl: true, value: null);
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
					role = c.NickName == question.UserId ? "user" : (c.NickName == "traking_system" ? "traking_system" : "asistant"),
					content = ProntManager.ProntAdapter(c.Body)
				}
			));
			historialMensajes.Add(new { role = "user", content = prompt });
			return historialMensajes;
		}

		public static Tbl_Case GestionaCaso(UserMessage data, string caseTitle, Tbl_Case? instaCase, bool isCaseWithIABotResponse)
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
					if (instaCase.Tbl_Servicios == null && servicios != null)
					{
						instaCase.Cat_Dependencias = dependencia;
						instaCase.Id_Dependencia = dependencia?.Id_Dependencia;
						instaCase.Id_Servicio = servicios?.Id_Servicio;
						instaCase.Tbl_Servicios = servicios;
						instaCase.Update();
					}
					return instaCase;
				}
				else
				{
					var mimeMessageCaseData = new MimeMessageCaseData { PlatformType = data!.Source?.ToUpper(), isWithIaResponse = isCaseWithIABotResponse, NewMessage = true };

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
				Attach_Files = [interaction.Attach],
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
				}
				else
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
			data.MimeMessageCaseData.NewMessage = true;
			data.Update();

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

	}

}
