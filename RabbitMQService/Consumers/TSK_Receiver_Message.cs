using RabbitMQService.Utils;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

using CAPA_NEGOCIO;
using CAPA_NEGOCIO.MAPEO;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text.Json;
using CAPA_DATOS;
using CAPA_NEGOCIO.IA;


namespace RabbitMQService.Consumers
{
    public class TSK_Receiver_Message : BaseConsumer
    {

        public TSK_Receiver_Message(IServiceProvider provider) 
            : base("TSK_Receiver_Message", provider)
        {

            base.ReceivedMessage += (obj, e) =>
            {
                MessageDto result = new MessageDto();
                try
                {
                    var options = new JsonSerializerOptions { ReferenceHandler = ReferenceHandler.Preserve };
                    var message = System.Text.Json.JsonSerializer.Deserialize<UserMessage>(e, options);

                    result.TransactionNumber = message.SessionId;
                    var instanceIA = new LlamaClient();
                    var responseIA = new ResponseAPI();

                    // Ejecutar asincronía sincrónicamente usando GetAwaiter().GetResult()
                    var response = instanceIA.GenerateResponse(message).GetAwaiter().GetResult();
                   // var jsonResponse = JsonConvert.DeserializeObject<JsonResponse>(response); 
                    //string messageContent = jsonResponse.Message.Content;
                    var response2 = responseIA.SendResponseToUser(message, response).GetAwaiter().GetResult();

                    result.Message = "Proceso exitoso";
                    Console.WriteLine("Test receive" + message.Source);
                }
                catch (Exception ex)
                {
                    //Enviar a broker de reintentos
                    result.AddError(ex.Message);
                }
                return result;
            };
        }
         
    }
}
