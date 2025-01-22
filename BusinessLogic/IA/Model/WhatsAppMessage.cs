using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace WhatsAppApi
{

	public class WhatsAppMessage
	{
		public string messaging_product { get; set; } = "whatsapp";
		public string to { get; set; } // Número del destinatario
		public string type { get; set; } = "template";
		public Template template { get; set; }

		public WhatsAppMessage(string to, string templateName, string languageCode, params string?[] parameters)
		{
			this.to = to;
			template = new Template
			{
				name = templateName,
				language = new Language { code = languageCode },
				components =
				[
					new Component
					{
						
						type = "body",
						parameters = CreateParameters(parameters)
					}
				]
			};
		}
		public WhatsAppMessage(string to, string templateName, string languageCode, object dataSource)
		{
			this.to = to;
			template = new Template
			{
				name = templateName,
				language = new Language { code = languageCode },
				components = new[]
				{
					new Component
					{
						
						type = "body",
						parameters = CreateParametersFromObject(dataSource)
					}
				}
			};
		}
		private Parameter[] CreateParametersFromObject(object dataSource)
		{
			// Obtener las propiedades públicas de la clase y sus valores
			var properties = dataSource.GetType().GetProperties();
			var parameters = new Parameter[properties.Length];

			for (int i = 0; i < properties.Length; i++)
			{
				// Obtener el valor de la propiedad
				var value = properties[i].GetValue(dataSource)?.ToString() ?? string.Empty;

				// Crear un nuevo parámetro con el valor
				parameters[i] = new Parameter
				{
					type = "text",
					text = value
				};
			}
			return parameters;
		}


		private Parameter[] CreateParameters(string[] values)
		{
			var parameters = new Parameter[values.Length];
			for (int i = 0; i < values.Length; i++)
			{
				parameters[i] = new Parameter { type = "text", text = values[i] };
			}
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
		public Component[]? components { get; set; }
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
		public string type { get; set; } // Tipo de parámetro (ejemplo: "text")
		public string text { get; set; } // Valor del parámetro
	}

}