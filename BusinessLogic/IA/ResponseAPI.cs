using DataBaseModel;
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

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "EAAYIa2ZCUUBcBO638ZCTCVh7g7hytYmnJDk41iMPfZAvIQhuWhTOKYt34yJxCzIZBZCK6e7KQwg2iKiQds3f1xZC0YQMKp33hkHW5ZAfPJ3uVksSTO9ASFMSGZBrNWb6MOoj9BA8Xxgmkk1a4ZAAmZAXz8ZBwsExZAnVJutgVNGZAMkW6ni2q6lrLWJSki6zvETfulWbUvjOqBHPTz3E3OwVv");
                client.PostAsync("https://graph.facebook.com/v21.0/488818134316697/messages", content).Wait();

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

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "YOUR_FACEBOOK_PAGE_ACCESS_TOKEN");
                client.PostAsync("https://graph.facebook.com/v13.0/me/messages", content).Wait();

                return "OK";
            }
            catch (Exception)
            {

                throw;
            }

        }

    }
}
