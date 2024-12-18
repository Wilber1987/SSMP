﻿using DataBaseModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CAPA_NEGOCIO.IA
{
    public class ResponseAPI
    {

        public async Task<string> SendResponseToUser(UserMessage userMessage, string response)
        {
            try
            {
                switch (userMessage.Source)
                {
                    case "WebAPI":
                        await SendResponseToWebAPIAsync(userMessage.UserId, response);
                        break;

                    case "WhatsApp":
                        await SendResponseToWhatsApp(userMessage.UserId, response);
                        break;

                    case "Messenger":
                        await SendResponseToMessengerAsync(userMessage.UserId, response);
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
                    else {
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

        public async Task<string> SendResponseToWhatsApp(string userId, string response)
        {
            try
            {
                var client = new HttpClient();
                var content = new StringContent(JsonConvert.SerializeObject(new
                {
                    messaging_product = "whatsapp",
                    to = userId,
                    recipient_type = "individual",
                    type = "text",
                    text = new { 
                        body = response,
                        preview_url= false
                    }
                }), Encoding.UTF8, "application/json");

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", SystemConfig.AppConfigurationValue(AppConfigurationList.MettaApi, "BeaverWhatsApp"));
                client.PostAsync(SystemConfig.AppConfigurationValue(AppConfigurationList.MettaApi, "HostMessageWhatsAppServices"), content).Wait();

                return "OK";
            }
            catch (Exception)
            {

                throw;
            }

        }

        public async static Task<string> SendResponseToMessengerAsync(string userId, string response)
        {
            try
            {
                var client = new HttpClient();
                var content = new StringContent(JsonConvert.SerializeObject(new
                {
                    recipient = new { id = userId },
                    message = new { text = response }
                }), Encoding.UTF8, "application/json");

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", SystemConfig.AppConfigurationValue(AppConfigurationList.MettaApi, "BeaverMessenger"));
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

    }
}
