using API.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using iText.Html2pdf;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Html2pdf;
using iText.Layout.Properties;
using iText.Kernel.Geom;
//using Microsoft.Playwright;

namespace UI.Controllers
{
	[ApiController]
	[Route("api/[controller]/[action]")]
	public class ApiDocumentsDataController : ControllerBase
	{
				//pdf 
		[HttpPost]
		public IActionResult GeneratePdf([FromBody] PdfRequest request)
		{
			if (string.IsNullOrEmpty(request.HtmlContent))
			{
				return BadRequest("HTML content cannot be empty.");
			}

			try
			{
				// Convertir HTML a PDF utilizando wkhtmltopdf
				byte[] pdfBytes = ConvertHtmlToPdf(request.HtmlContent, request.PageType);

				// Devolver el archivo PDF como respuesta
				return File(pdfBytes, "application/pdf", "generated.pdf");
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Error generating PDF: {ex.Message}");
			}
		}

		private byte[] ConvertHtmlToPdf(string htmlContent, string? pageType)
		{
			using (var memoryStream = new MemoryStream())
			{
				// Configurar el tamaño de la página
				var pageSize = GetPageSize(pageType);

				// Crear un PdfWriter vinculado al MemoryStream
				using (var writer = new PdfWriter(memoryStream))
				{
					// Inicializar el documento PDF con el tamaño de página configurado
					using (var pdfDocument = new PdfDocument(writer))
					{
						// Establecer el tamaño de la página
						pdfDocument.SetDefaultPageSize(pageSize);

						// Configurar propiedades de conversión
						ConverterProperties properties = new ConverterProperties();
						properties.SetBaseUri(""); // Manejo de rutas relativas para imágenes base64

						// Convertir el HTML con estilos y encabezados
						HtmlConverter.ConvertToPdf(htmlContent, pdfDocument, properties);
					}
				}

				// Retornar el contenido del MemoryStream como un arreglo de bytes
				return memoryStream.ToArray();
			}
		}
		/*public async Task<byte[]> GeneratePdfWithPlaywright(string htmlContent, string? pageType)
		{
			using var playwright = await Playwright.CreateAsync();
			await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
			var page = await browser.NewPageAsync();
			await page.SetContentAsync(htmlContent);
			var pdfStream = await page.PdfStreamAsync(new PagePdfOptions { Format =  pageType ?? "A4"  });
			using var memoryStream = new MemoryStream();
			await pdfStream.CopyToAsync(memoryStream);
			return memoryStream.ToArray();
		}*/
		private iText.Kernel.Geom.PageSize GetPageSize(string? pageType)
		{
			switch (pageType)
			{
				case "A4":
					return PageSize.A4;
				case "A4-horizontal":
					return new PageSize(PageSize.A4.GetHeight(), PageSize.A4.GetWidth());
				case "carta":
					return new PageSize(612, 792); // Carta
				case "carta-horizontal":
					return new PageSize(612, 792).Rotate(); // Carta Horizontal
				case "oficio":
					return new PageSize(816, 1056); // Oficio
				case "oficio-horizontal":
					return new PageSize(816, 1056).Rotate(); // Oficio Horizontal
				default:
					return PageSize.A4; // Default to A4 if no valid type is provided
			}
		}


	}
	public class PdfRequest
	{
		public string? HtmlContent { get; set; }
		public string? PageType { get; set; }
	}
}