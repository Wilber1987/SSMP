using CAPA_DATOS;
using CAPA_DATOS.Services;
using DataBaseModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using WhatsAppApi;

namespace CAPA_NEGOCIO.IA
{
	public class ResponseAPI
	{

		public async Task<string> SendResponseToUser(UserMessage userMessage,
			string response,
		 	bool isWithIaResponse,
		 	List<CAPA_DATOS.Services.ModelFiles>? attach_Files)
		{
			try
			{
				switch (userMessage.Source?.ToUpper())
				{
					case "WEBAPI":
						await SendResponseToWebAPIAsync(userMessage.UserId, response);
						break;
					case "WHATSAPP":
						await SendResponseToWhatsApp(userMessage.UserId, response, isWithIaResponse, attach_Files);
						break;
					case "MESSENGER":
						await SendResponseToMessengerAsync(userMessage.UserId, response, isWithIaResponse, attach_Files);
						break;
					default:
						Console.WriteLine("Canal no reconocido.");
						break;
				}
				return "Ok";
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error al enviar respuesta: {ex.Message}");
				return "Error";
			}
		}

		//este metodo puede implementar si se envia la respuesta a otra api externa, invocando la url y preparando el body
		public async static Task<string> SendResponseToWebAPIAsync(string userId, string response)
		{
			try
			{   // Ejemplo de código para enviar una respuesta a través de WebAPI
				using (var client = new HttpClient())
				{
					var content = new StringContent(JsonConvert.SerializeObject(
						new { UserId = userId, MessageIA = response }),
						Encoding.UTF8, "application/json");
					var responseMessage = await client.PostAsync(SystemConfig.AppConfigurationValue(AppConfigurationList.IAServices, "Server"), content);
					if (responseMessage.IsSuccessStatusCode)
					{
						return "OK";
					}
					else
					{
						var errorMessage = await responseMessage.Content.ReadAsStringAsync();
						return $"Error: {responseMessage.StatusCode} - {errorMessage}";
					}
				}
			}
			catch (Exception)
			{
				throw;
			}

		}
		public async Task<string> SendResponseToWhatsApp(string userId, string response,
			bool isCaseWithBotResponse = true,
			List<ModelFiles>? attach_Files = null)
		{
			try
			{
				using var client = new HttpClient();

				// Configurar el encabezado de autorización
				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
					"Bearer",
					SystemConfig.AppConfigurationValue(AppConfigurationList.MettaApi, "BeaverWhatsApp")
				);

				string numberIdentification = isCaseWithBotResponse ? "IANumberIdentification" : "NumberIdentification";
				string apiUrl = GetMetaApiRoute(numberIdentification);

				// Construir el payload del mensaje
				var messagePayloadList = BuildMessagePayload(userId, response, attach_Files);
				Boolean successResponse = true;
				foreach (var messagePayload in messagePayloadList)
				{
					// Serializar y enviar la solicitud
					string jsonBody = JsonConvert.SerializeObject(messagePayload);
					var content = new StringContent(JsonConvert.SerializeObject(messagePayload), Encoding.UTF8, "application/json");
					var responseMessage = await client.PostAsync(apiUrl, content);
					var responseContent = await responseMessage.Content.ReadAsStringAsync();
					if (!responseMessage.IsSuccessStatusCode)
					{
						LoggerServices.AddMessageError("Error enviando mensaje: " + responseMessage.StatusCode, new Exception("error al enviar el mensaje: \n\n" + responseContent));
						successResponse = false;
						break;
					}
				}
				return successResponse
					? $"Success"
					: $"Error";

			}
			catch (Exception ex)
			{
				return $"Exception: {ex.Message}";
			}
		}

		// Método para construir el mensaje según el tipo de contenido
		private List<object> BuildMessagePayload(string userId, string response, List<ModelFiles>? attach_Files)
		{
			string defaultUrl = "https://chatbot.correos.gob.gt:8443";
			string domainUrl = Transactional_Configuraciones.GetParam(ConfiguracionesThemeEnum.DOMAIN_URL, defaultUrl, ConfiguracionesTypeEnum.THEME)?.Valor ?? defaultUrl;
			string recipient = "+" + userId.Replace("+", "");
			List<object> messagePayload = [];
			if (attach_Files != null && attach_Files.Any())
			{
				int messageindex = 0;
				attach_Files.ForEach(file =>
				{

					string? fileType = GetWhatsAppFileType(file.Type);

					if (fileType == null)
					{
						throw new ArgumentException($"Tipo de archivo no soportado ({file.Type})");
					}

					if (fileType == "image")
					{
						messagePayload.Add(new
						{
							messaging_product = "whatsapp",
							to = recipient,
							type = fileType,
							image = new
							{
								link = $"{domainUrl}/{file.Value?.Replace("wwwroot\\", "")}",
								//link =  "https://chatbot.correos.gob.gt:8443/Media/img/logo.png",
								//link = "https://correos.gob.gt/wp-content/uploads/2025/02/logo-190-X-90.png",
								caption = !string.IsNullOrEmpty(response) && messageindex == 0 ? response : null
							}

						});
					}
					else
					{
						messagePayload.Add(new
						{
							messaging_product = "whatsapp",
							to = recipient,
							type = "document",
							document = new
							{
								link = $"{domainUrl}/{file.Value?.Replace("wwwroot\\", "")}",
								//link = "https://8584-152-231-33-60.ngrok-free.app/Media/img/logo.png",
								caption = !string.IsNullOrEmpty(response) && messageindex == 0 ? response : null
							}
						});
					}
					messageindex++;
				});
				return messagePayload;
			}

			// Mensaje solo de texto
			return [new
			{
				messaging_product = "whatsapp",
				to = recipient,
				type = "text",
				text = new { body = response }
			}];
		}
		private string? GetWhatsAppFileType(string? fileType)
		{
			if (string.IsNullOrEmpty(fileType)) return null;

			var fileTypeMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
			{
				{ ".jpg", "image" },
				{ ".jpeg", "image" },
				{ ".png", "image" },
				{ ".gif", "image" },
				{ ".pdf", "document" },
				{ ".doc", "document" },
				{ ".docx", "document" },
				{ ".xls", "document" },
				{ ".xlsx", "document" },
				{ ".mp4", "video" },
				{ ".mp3", "audio" },
				{ ".ogg", "audio" }
			};

			return fileTypeMap.TryGetValue(fileType, out string? whatsappType) ? whatsappType : null;
		}



		private static string? GetMetaApiRoute(string numberIdentification)
		{
			return SystemConfig.AppConfigurationValue(AppConfigurationList.MettaApi, "HostMessageWhatsAppServices")?
					.Replace("{Identification}", SystemConfig.AppConfigurationValue(AppConfigurationList.MettaApi, numberIdentification));
		}

		//{"error":{"message":"(#132001) Template name does not exist in the translation","type":"OAuthException","code":132001,"error_data":{"messaging_product":"whatsapp","details":"template name (notificacion_paquete) does not exist in es"},"fbtrace_id":"AKCYcYG8nDBghnNaaYAQx3M"}}
		//{"error":{"message":"(#100) The parameter messaging_product is required.","type":"OAuthException","code":100,"fbtrace_id":"ABuwLf-MvFCEZAEGKA-c8m_"}}
		public async Task<string> SendResponseToWhatsAppWithTemplate(WhatsAppMessage message, bool isCaseWithBotResponse = true)
		{
			try
			{
				using var client = new HttpClient();

				/*message.template.name = "hello_world";
				message.template.language.code = "en_US";
				message.template.components = null;*/

				// Crear el contenido de la solicitud
				var content = new StringContent(JsonConvert.SerializeObject(message), Encoding.UTF8, "application/json");

				// Configurar el encabezado de autorización
				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
					"Bearer",
					SystemConfig.AppConfigurationValue(AppConfigurationList.MettaApi, "BeaverWhatsApp")
				);

				// Enviar la solicitud y capturar la respuesta
				string numberIdentification = isCaseWithBotResponse ? "IANumberIdentification" : "NumberIdentification";

				// Enviar la solicitud y capturar la respuesta
				var responseMessage = await client.PostAsync(GetMetaApiRoute(numberIdentification), content);
				// Leer el contenido de la respuesta
				var responseContent = await responseMessage.Content.ReadAsStringAsync();

				if (responseMessage.IsSuccessStatusCode)
				{
					// La solicitud fue exitosa
					return $"Success: {responseContent}";
				}
				else
				{
					LoggerServices.AddMessageError($"Error al enviar notificacion por whatsapp ---- {content} ---- Error: {responseMessage.StatusCode} - {responseContent}", new Exception(responseContent));
					// La solicitud falló, registra los detalles del error
					return $"Error: {responseMessage.StatusCode} - {responseContent}";
				}
			}
			catch (Exception ex)
			{
				LoggerServices.AddMessageError("Error al enviar notificacion por whatsapp", ex);
				// Manejo de excepciones
				return $"Exception: {ex.Message}";
			}
		}
		public async Task<string> SendDeliveryReceipt(string messageId, string userId, bool isCaseWithBotResponse = true)
		{
			try
			{
				using var client = new HttpClient();

				// Crear el contenido de la solicitud
				var content = new StringContent(JsonConvert.SerializeObject(new
				{
					messaging_product = "whatsapp",
					status = "read",
					message_id = messageId
				}), Encoding.UTF8, "application/json");
				
				// Enviar la solicitud y capturar la respuesta
				string numberIdentification = isCaseWithBotResponse ? "IANumberIdentification" : "NumberIdentification";

				// Enviar la solicitud y capturar la respuesta
			

				// Configurar el encabezado de autorización
				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
					"Bearer",
					SystemConfig.AppConfigurationValue(AppConfigurationList.MettaApi, "BeaverWhatsApp")
				);
				var responseMessage = await client.PostAsync(GetMetaApiRoute(numberIdentification), content);

				// Leer el contenido de la respuesta
				var responseContent = await responseMessage.Content.ReadAsStringAsync();

				if (responseMessage.IsSuccessStatusCode)
				{
					return $"Delivery receipt sent successfully: {responseContent}";
				}
				else
				{
					return $"Error sending delivery receipt: {responseMessage.StatusCode} - {responseContent}";
				}
			}
			catch (Exception ex)
			{
				return $"Exception while sending delivery receipt: {ex.Message}";
			}
		}

		public async Task<string> SendReadReceipt(string messageId, string userId)
		{
			try
			{
				using var client = new HttpClient();

				// Crear el contenido de la solicitud
				var content = new StringContent(JsonConvert.SerializeObject(new
				{
					messaging_product = "whatsapp",
					status = "read",
					message_id = messageId,
					to = "+" + userId.Replace("+", "")
				}), Encoding.UTF8, "application/json");

				// Configurar el encabezado de autorización
				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
					"Bearer",
					SystemConfig.AppConfigurationValue(AppConfigurationList.MettaApi, "BeaverWhatsApp")
				);

				// Enviar la solicitud
				var responseMessage = await client.PostAsync(
					SystemConfig.AppConfigurationValue(AppConfigurationList.MettaApi, "HostMessageWhatsAppServices"),
					content
				);

				// Leer el contenido de la respuesta
				var responseContent = await responseMessage.Content.ReadAsStringAsync();

				if (responseMessage.IsSuccessStatusCode)
				{
					return $"Read receipt sent successfully: {responseContent}";
				}
				else
				{
					return $"Error sending read receipt: {responseMessage.StatusCode} - {responseContent}";
				}
			}
			catch (Exception ex)
			{
				return $"Exception while sending read receipt: {ex.Message}";
			}
		}

		public async static Task<string> SendResponseToMessengerAsync(string userId, string response, bool isWithIaResponse = true, List<ModelFiles>? attach_Files = null)
		{
			try
			{
				var client = new HttpClient();
				var content = new StringContent(JsonConvert.SerializeObject(new
				{
					recipient = new { id = userId },
					message = new { text = response }
				}), Encoding.UTF8, "application/json");
				string identificationPage  =  isWithIaResponse ? "BeaverIAMessenger" : "BeaverMessenger";

				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", SystemConfig.AppConfigurationValue(AppConfigurationList.MettaApi, identificationPage));
				client.PostAsync(SystemConfig.AppConfigurationValue(AppConfigurationList.MettaApi, "HostMessageMessengerServices"), content).Wait();

				return "OK";
			}
			catch (Exception)
			{

				throw;
			}

		}

		public async static Task<string> SendResponseToInstagramAsync(string userId, string response)
		{
			try
			{
				var client = new HttpClient();
				var content = new StringContent(JsonConvert.SerializeObject(new
				{
					recipient = new { id = userId },
					message = new { text = response }
				}), Encoding.UTF8, "application/json");

				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", SystemConfig.AppConfigurationValue(AppConfigurationList.MettaApi, "Beaver"));
				client.PostAsync(SystemConfig.AppConfigurationValue(AppConfigurationList.MettaApi, "HostMessageInstagramServices"), content).Wait();

				return "OK";
			}
			catch (Exception)
			{

				throw;
			}

		}

		public async static Task<string> SendTwitterDirectMessageAsync(string userId, string message)
		{
			try
			{
				using (var client = new HttpClient())
				{
					// Configurar encabezados de autorización
					client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", SystemConfig.AppConfigurationValue(AppConfigurationList.MettaApi, "BeaverTwiter"));

					// Construir el cuerpo del mensaje
					var messageBody = new
					{
						@event = new
						{
							type = "message_create",
							message_create = new
							{
								target = new { recipient_id = userId },
								message_data = new { text = message }
							}
						}
					};

					// Serializar a JSON
					var jsonContent = JsonConvert.SerializeObject(messageBody);
					var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

					// Hacer la solicitud
					var response = await client.PostAsync(SystemConfig.AppConfigurationValue(AppConfigurationList.MettaApi, "HostMessageTwitterServices"), content);

					if (response.IsSuccessStatusCode)
					{
						return "Ok";
					}
					else
					{
						var errorResponse = await response.Content.ReadAsStringAsync();
						throw new Exception($"Error al enviar mensaje: {errorResponse}");
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception($"Excepción: {ex.Message}", ex);
			}
		}

		public async static Task<string> SendTelegramDirectMessageAsync(string userId, string message)
		{
			try
			{
				using (var client = new HttpClient())
				{
					// Configurar encabezados de autorización
					client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", SystemConfig.AppConfigurationValue(AppConfigurationList.MettaApi, "BeaverTelegram"));


					string url = $"https://api.telegram.org/bot{SystemConfig.AppConfigurationValue(AppConfigurationList.MettaApi, "BeaverTelegram")}/sendMessage";
					var content = new StringContent($"{{\"chat_id\": \"{userId}\", \"text\": \"{message}\"}}", Encoding.UTF8, "application/json");

					HttpResponseMessage response = await client.PostAsync(url, content);
					if (response.IsSuccessStatusCode)
					{
						return "Ok";
					}
					else
					{
						var errorResponse = await response.Content.ReadAsStringAsync();
						throw new Exception($"Error al enviar mensaje: {errorResponse}");
					}

				}
			}
			catch (Exception ex)
			{
				throw new Exception($"Excepción: {ex.Message}", ex);
			}
		}


		public static async Task<ModelFiles> DownloadImageAsync(string? imageId, string mime_type)
		{
			try
			{
				using (var handler = new HttpClientHandler { AllowAutoRedirect = true }) // Permite redirecciones
				using (var client = new HttpClient(handler))
				{
					client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",
						SystemConfig.AppConfigurationValue(AppConfigurationList.MettaApi, "BeaverWhatsApp"));

					client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64)");

					string? metaApiUrl = SystemConfig.AppConfigurationValue(AppConfigurationList.MettaApi, "HostWhatsAppServices");

					// Paso 1: Obtener la URL de la imagen desde la API de Meta
					HttpResponseMessage response = await client.GetAsync($"{metaApiUrl}/{imageId}");
					response.EnsureSuccessStatusCode();

					string jsonResponse = await response.Content.ReadAsStringAsync();
					JObject data = JObject.Parse(jsonResponse);
					string imageUrl = data["url"].ToString();

					// **IMPORTANTE:** Agregar autorización en la descarga de la imagen
					using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, imageUrl))
					{
						requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer",
							SystemConfig.AppConfigurationValue(AppConfigurationList.MettaApi, "BeaverWhatsApp"));
						requestMessage.Headers.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64)");

						// Paso 2: Descargar la imagen
						HttpResponseMessage imageResponse = await client.SendAsync(requestMessage);
						imageResponse.EnsureSuccessStatusCode();

						byte[] imageBytes = await imageResponse.Content.ReadAsByteArrayAsync();
						string savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Media", "Upload", imageId + mime_type);						
					
						string RutaRelativa = Path.GetRelativePath(Directory.GetCurrentDirectory(), Path.Combine("wwwroot", "Media", "Upload", imageId + mime_type));

						
						await File.WriteAllBytesAsync(savePath, imageBytes);
						
						LoggerServices.AddMessageInfo($"Imagen descargada de {imageUrl} en ruta {RutaRelativa}");
						return new ModelFiles
						{
							Type = mime_type,
							Value =  RutaRelativa,
							Name = imageId + mime_type
						};
					}
				}
			}
			catch (Exception ex)
			{
				LoggerServices.AddMessageError($"Error al descargar la imagen: {ex.Message}", ex);
				Console.WriteLine($"Error al descargar la imagen: {ex.Message}");
				return null;
			}
		}

	}
}
