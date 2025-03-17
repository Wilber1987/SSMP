using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CAPA_NEGOCIO.Gestion_Mensajes.Operations;
using iText.Layout.Element;

namespace WhatsAppApi
{

	public class WhatsAppMessage
	{
		public string messaging_product { get; set; } = "whatsapp";
		public string to { get; set; } // Número del destinatario
		public string type { get; set; } = "template";
		public Template template { get; set; }

		private static bool IsImageHeader(string imageParam)
		{
			return imageParam.Contains(".jpg") || imageParam.Contains(".png");
		}

		public WhatsAppMessage(string to, string templateName, string languageCode, List<NotificationsParams>? dataSource, string? imageParam = null)
		{
			this.to = to;
			var components = new List<Component> { };
			if (imageParam != null)
			{
				List<Parameter> parameters = [];
				if (IsImageHeader(imageParam))
				{
					parameters.Add(new Parameter
					{
						type = "IMAGE",
						image = new { link = imageParam }
					});
				}
				else if (IsVideoHeader(imageParam))
				{
					parameters.Add(new Parameter
					{
						type = "video",
						video = new { link = imageParam }
					});
				}
				else if (IsVideDocumentHeader(imageParam))
				{
					parameters.Add(new Parameter
					{
						type = "document",
						document = new { link = imageParam }
					});
				}
				components.Add(new Component
				{

					type = "header",
					parameters = parameters.ToArray()
				});
			}
			components.Add(new Component { type = "body", parameters = CreateParameters(dataSource ?? []) });

			template = new Template
			{
				name = templateName,
				language = new Language { code = languageCode },
				components = components
			};
		}

		private bool IsVideDocumentHeader(string imageParam)
		{
			return imageParam.Contains(".pdf");
		}

		private bool IsVideoHeader(string imageParam)
		{
			return imageParam.Contains(".mp4") || imageParam.Contains(".avi");
		}

		private Parameter[] CreateParameters(List<NotificationsParams> values)
		{
			var parameters = new Parameter[values.Count];
			int index = 0;
			values.ForEach(param =>
			{
				parameters[index] = new Parameter { type = param.Type ?? "text", text = param.Value ?? "Desconocido" };
				index++;
			});
			return parameters;
		}

		public static string ObtenerExtensionPorDepartamento(string? departamento)
		{
			// Diccionario que mapea departamentos a países y extensiones telefónicas
			var departamentosCentroamerica = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
			{
				// El Salvador
				{ "San Salvador", "+503" },
				{ "Santa Ana", "+503" },
				{ "La Libertad", "+503" },

				// Guatemala
				{ "Alta Verapaz", "+502" },
				{ "Baja Verapaz", "+502" },
				{ "Chimaltenango", "+502" },
				{ "Chiquimula", "+502" },
				{ "El Progreso", "+502" },
				{ "Escuintla", "+502" },
				{ "Guatemala", "+502" },
				{ "Huehuetenango", "+502" },
				{ "Izabal", "+502" },
				{ "Jalapa", "+502" },
				{ "Jutiapa", "+502" },
				{ "Petén", "+502" },
				{ "Quetzaltenango", "+502" },
				{ "Quiché", "+502" },
				{ "Retalhuleu", "+502" },
				{ "Sacatepéquez", "+502" },
				{ "San Marcos", "+502" },
				{ "Santa Rosa", "+502" },
				{ "Sololá", "+502" },
				{ "Suchitepéquez", "+502" },
				{ "Totonicapán", "+502" },
				{ "Zacapa", "+502" },

				// Honduras
				{ "Tegucigalpa", "+504" },
				{ "San Pedro Sula", "+504" },
				{ "La Ceiba", "+504" },

				// Nicaragua
				{ "Boaco", "+505" },
				{ "Carazo", "+505" },
				{ "Chinandega", "+505" },
				{ "Chontales", "+505" },
				{ "Estelí", "+505" },
				{ "Granada", "+505" },
				{ "Jinotega", "+505" },
				{ "León", "+505" },
				{ "Madriz", "+505" },
				{ "Managua", "+505" },
				{ "Masaya", "+505" },
				{ "Matagalpa", "+505" },
				{ "Nueva Segovia", "+505" },
				{ "Rivas", "+505" },
				{ "Río San Juan", "+505" },
				{ "Costa Caribe Norte", "+505" },
				{ "Costa Caribe Sur", "+505" },

				// Costa Rica
				{ "San José", "+506" },
				{ "Alajuela", "+506" },
				{ "Cartago", "+506" },

				// Panamá
				{ "Panamá", "+507" },
				{ "Colón", "+507" },
				{ "David", "+507" }
			};

			// Buscar el departamento en el diccionario
			return departamentosCentroamerica.TryGetValue(departamento, out string extension) ? extension : "+502";
		}
	}

	public class Template
	{
		public string name { get; set; } // Nombre de la plantilla
		public Language language { get; set; }
		public List<Component> components { get; set; }
	}

	public class Language
	{
		public string code { get; set; } = "es";// Código de idioma (ejemplo: "es_ES")
	}

	public class Component
	{
		public string type { get; set; } // Tipo de componente (ejemplo: "body")
		public Parameter[] parameters { get; set; }
	}

	public class Parameter
	{
		public object? video { get; set; }
		public object? document { get; set; }
		public string? type { get; set; } // Tipo de parámetro (ejemplo: "text")
		public string? text { get; set; } // Valor del parámetro
		public object? image { get; set; }
	}

}