using System;
using System.Threading;
using System.Threading.Tasks;
using CAPA_DATOS;
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
				var deliveryResult = await responseIA.SendDeliveryReceipt(unifiedMessage.Id, unifiedMessage.UserId);
				LoggerServices.AddMessageInfo(deliveryResult);
				
				await responseIA.SendResponseToUser(unifiedMessage, response?.MessageIA);
				var readResult = await responseIA.SendReadReceipt(unifiedMessage.Id, unifiedMessage.UserId);
				LoggerServices.AddMessageInfo(readResult);
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