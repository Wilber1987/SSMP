using System;
using System.Threading;
using System.Threading.Tasks;
using APPCORE;
using CAPA_NEGOCIO;
using CAPA_NEGOCIO.IA;

namespace BusinessLogic
{
	public static class BackgroundProcessor
	{
		private static readonly SemaphoreSlim Semaphore = new SemaphoreSlim(10); // Limitar a 10 tareas simultáneas

		public static async Task ProcessInBackground(UserMessage unifiedMessage)
		{
			await Semaphore.WaitAsync(); // Limitar concurrencia
			try
			{
				var instanceIA = new LlamaClient();
				var response = await instanceIA.GenerateResponse(unifiedMessage);
				var responseIA = new ResponseAPI();
				// Enviar recibo de entrega				
				if (response?.MessageIA != null && response?.MessageIA.Trim() != "")
				{
					if (unifiedMessage.IsMetaWhatsAppApi)
					{
						try
						{
							var deliveryResult = await responseIA.SendDeliveryReceipt(unifiedMessage.Id, unifiedMessage.UserId, response?.MessageIA != null);
							LoggerServices.AddMessageInfo(deliveryResult);
						}
						catch (System.Exception)
						{ }
					}
					await responseIA.SendResponseToUser(unifiedMessage, response?.MessageIA, response.IsWithIaResponse, []);
					var readResult = await responseIA.SendReadReceipt(unifiedMessage.Id, unifiedMessage.UserId);
					LoggerServices.AddMessageInfo(readResult);
				}


			}
			catch (Exception ex)
			{
				LoggerServices.AddMessageError("ERROR: Procesando en segundo plano", ex);
			}
			finally
			{
				Semaphore.Release(); // Liberar el recurso
			}
		}

	}
}