﻿using System.Text.Json;
using API.Controllers;
using CAPA_DATOS;
using CAPA_NEGOCIO;
using CAPA_NEGOCIO.IA;
using DataBaseModel;
using IA.Dto;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;


namespace UI.SSMP_IA.ApiControllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class WebhookSsmpIAController : ControllerBase
	{
		private static readonly SemaphoreSlim Semaphore = new SemaphoreSlim(10); // Limitar a 10 tareas simultáneas

		[HttpPost]
		[AuthController] // TODO: QUITAR
		public IActionResult ReceiveMessage([FromBody] dynamic message, [FromHeader(Name = "X-Platform")] string platform = null)
		{
			try
			{
				if (string.IsNullOrEmpty(platform))
				{
					platform = "whatsapp"; // Valor predeterminado o manejo alternativo.
				}
				try
				{
					LoggerServices.AddMessageInfo($"Info: nuevoMensaje: " + message.ToString());
				}
				catch (Exception)
				{
					// Manejo silencioso del error de logging
				}

				// Determinar el origen del mensaje
				UserMessage unifiedMessage = platform?.ToLower() switch
				{
					"whatsapp" => UserMessage.ProcessWhatsAppMessage(message),
					"messenger" => UserMessage.ProcessMessengerMessage(message),
					"webapi" => UserMessage.ProcessWebApiMessage(message),
					_ => throw new InvalidOperationException("Unsupported platform")
				};

				if (unifiedMessage != null)
				{
					switch (unifiedMessage.Source)
					{
						case "webapi":
							// Procesar inmediatamente para webapi
							var resp = EnqueueMessageWebApi(unifiedMessage, "webapi");
							ResponseWebApi reply = new ResponseWebApi()
							{
								Reply = resp.MessageIA,
								WithAgentResponse = resp.WithAgentResponse ?? false,
								ProfileName = "IA",
								Id_Case = resp.Id_case,
							};
							// Respuesta inmediata para webapi
							return Ok(reply);
						case "WhatsApp":
						case "messenger":
							// Procesar en segundo plano para WhatsApp y Messenger
							Task.Run(() => BusinessLogic.BackgroundProcessor.ProcessInBackground(unifiedMessage));
							return Ok("EVENT_RECEIVED");
						default:
							return BadRequest("Unsupported platform source.");
					}
				}
				return BadRequest("Message could not be processed.");
			}
			catch (Exception ex)
			{
				// Manejo de errores genérico
				Console.WriteLine($"Error processing message: {ex}");
				LoggerServices.AddMessageError($"ERROR: procesando mensaje", ex);
				return StatusCode(500, "Internal Server Error");
			}
		}
		private static UserMessage EnqueueMessageWebApi(UserMessage message, string source)
		{
			message.Source = source; // Añadimos la fuente al mensaje
			var instanceIA = new LlamaClient();
			// Ejecutar asincronía sincrónicamente usando GetAwaiter().GetResult()
			var response = instanceIA.GenerateResponse(message).GetAwaiter().GetResult();
			return response;
		}

		[HttpGet]
		public IActionResult VerifyToken()
		{
			try
			{
				string? AccessToken = SystemConfig.AppConfigurationValue(AppConfigurationList.MettaApi, "AppToken");

				var token = Request.Query["hub.verify_token"].ToString();
				var challenge = Request.Query["hub.challenge"].ToString();

				if (challenge != null && token != null && token == AccessToken)
				{
					return Ok(challenge);
				}
				else
				{
					return BadRequest();
				}
			}
			catch (System.Exception ex)
			{
				LoggerServices.AddMessageError($"ERROR: Verificando token", ex);
				throw;
			}

		}

	}
}
