using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.IA
{
	public class CaseEvaluatorManager
	{
		public static string DeterminarCategoria(string consulta, string? previusCategory)
		{
			

			// Normalizar la consulta
			consulta = NormalizarTexto(consulta);

			// Dividir la consulta en palabras
			var palabras = consulta.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

			// Diccionario para contar coincidencias ponderadas
			Dictionary<string, int> coincidencias = new Dictionary<string, int>();
			string category = ExtractCategoryServices(consulta, palabras, coincidencias);
			if (previusCategory != null && category != "CIERRE_DE_CASO" && previusCategory != "INICIO")
			{
				return previusCategory;
			}
			// Retornar categoría predeterminada si no hay coincidencias
			return category;

		}
		// Método para normalizar texto
		private static	string NormalizarTexto(string texto)
			{
				texto = texto.ToLowerInvariant();
				texto = string.Concat(texto.Normalize(NormalizationForm.FormD)
										   .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark));
				var caracteresPermitidos = texto.Where(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c));
				return new string(caracteresPermitidos.ToArray());
			}
		private static string ExtractCategoryServices(string consulta, string[] palabras, Dictionary<string, int> coincidencias)
		{
			string category = "ASISTENCIA_GENERAL";
			/*if (consulta == "8")
			{
				return "EVENTOS";
			}
			else if (consulta == "7")
			{
				return "SOLICITUD_DE_ASISTENCIA";
			}
			else  */if (consulta.ToUpper() == "MENU")
			{
				return "INICIO";
			}
			else if (consulta == "5")
			{
				return "SOLICITUD_DE_ASISTENCIA";
			}
			else if (consulta == "4")
			{
				return "CONSULTA_DE_CONTACTO";
			}
			else if (consulta == "3")
			{
				return "INFORMACION_SOBRE_DOCUMENTOS";
			}
			else if (consulta == "2")
			{
				return "INFORMACION_SOBRE_DOCUMENTOS";
			}
			else if (consulta == "1")
			{
				return "RASTREO_Y_SEGUIMIENTOS";
			} 

			foreach (var categoria in Categorias)
			{
				int conteo = 0;

				foreach (var fraseClave in categoria.Value)
				{
					string fraseNormalizada = NormalizarTexto(fraseClave);

					// Coincidencia exacta de frase
					if (consulta.Contains(fraseNormalizada))
					{
						conteo += 10; // Frases completas tienen más peso
					}
					else
					{
						// Coincidencias parciales en palabras clave
						int palabraCoincidencias = palabras.Count(p => fraseNormalizada.Contains(p));

						// Se puede incrementar el peso de las coincidencias de "retraso" para hacerlas más significativas
						if (categoria.Key == "QUEJAS_POR_RETRASOS" && palabraCoincidencias > 0)
						{
							conteo += 20;  // Agregar más peso a coincidencias de queja por retrasos
						}
						else
						{
							conteo += palabraCoincidencias;
						}
					}
				}

				if (conteo > 0)
				{
					coincidencias[categoria.Key] = conteo;
				}
			}

			// Seleccionar la categoría con más coincidencias ponderadas
			if (coincidencias.Any())
			{
				category = coincidencias.OrderByDescending(c => c.Value).First().Key;
			}
			if (category != "CIERRE_DE_CASO")
			{
				return "INICIO";
			}

			return category;
		}

		static Dictionary<string, List<string>> Categorias = new Dictionary<string, List<string>>()
		{
			{ "CIERRE_DE_CASO", new List<string>
				{
					// Frases de agradecimiento genéricas
					"Gracias", "Muchas gracias", "Muy amable", "Agradezco su ayuda",
					"Gracias por la información", "Está bien, gracias", "Todo claro, gracias",
					"Gracias por resolver mi consulta", "Quedó claro, muchas gracias",
					"Gracias por tu tiempo", "Todo bien, gracias",

					// Frases que implican cierre
					"Eso era todo", "No necesito más ayuda", "Es todo, gracias",
					"Ya no tengo dudas", "Ya no tengo preguntas", "Con eso es suficiente",
					"Listo, gracias", "Por ahora es todo", "Está claro, no hay más dudas",

					// Variantes de finalización de interacción
					"Fue todo, gracias", "Ya está resuelto", "Consulta resuelta, gracias",
					"No tengo más consultas", "Por ahora estoy bien, gracias",
					"No necesito más información", "Estoy conforme, gracias",

					// Respuestas que implican satisfacción
					"Excelente, gracias", "Perfecto, muchas gracias", "Todo perfecto, gracias",
					"Me ayudaron mucho, gracias", "Gran ayuda, gracias",

					// Frases de despedida que sugieren cierre
					"Eso es todo por ahora", "Nos vemos, gracias", "Buen día, gracias",
					"Adiós, gracias por todo", "Hasta luego, muchas gracias",
					"Gracias y hasta la próxima", "Gracias por la atención",

					// Frases combinadas con agradecimientos y cierre
					"Agradezco la ayuda, ya no necesito nada más", "Estoy satisfecho con la respuesta, gracias",
					"Han resuelto mi duda, gracias", "Gracias por la asistencia, eso es todo",
					"Consulta cerrada, gracias por su tiempo", "Gracias, hasta luego",

					// Variantes menos formales
					"Todo bien", "Todo claro", "Todo perfecto", "Todo listo",
					"Ok, gracias", "Vale, gracias", "Gracias, ya entendí",
				}
			}, { "QUEJAS_POR_RETRASOS", new List<string>
				{
					"retraso", "tardanza", "no llega", "demora", "tardó mucho",
					"se retrasó", "demorado", "esperando paquete", "aún no llega",
					"tiempo de espera", "muy lento", "problema con el tiempo", "tarda mucho",
					"Mi paquete lleva mucho tiempo sin llegar", "Tengo un retraso con mi envío",
					"El pedido está tardando más de lo esperado", "Consulta sobre demora en mi paquete",
					"Mi paquete está retrasado", "No llega mi pedido",
					"El tiempo de espera es muy largo", "Problema con el retraso de mi envío",
					"¿Por qué mi paquete aún no llega?", "Estoy esperando pero no hay actualización"
				}
			}, { "RASTREO_Y_SEGUIMIENTOS", new List<string>
				{
					// Palabras clave comunes
					"paquete", "rastrear", "seguimiento", "número de rastreo", "estado", "dónde está",
					"tracking", "localizar", "ubicación", "mi pedido", "ver rastreo", "ver tracking",
					"consulta de estado",

					// Frases comunes y variantes
					"¿Dónde está mi paquete?", "No encuentro el estado de mi envío",
					"Quiero rastrear mi pedido", "¿Cómo hago para rastrear mi paquete?",
					"Necesito saber dónde está mi paquete", "Consulta sobre número de rastreo",
					"¿Cuál es el estado actual de mi pedido?", "No puedo ver el seguimiento",
					"Mi paquete aparece como perdido", "¿Cómo puedo localizar mi paquete?",
					"Necesito ayuda con mi paquete", "¿Dónde puedo ver el seguimiento?",
					"Estado pendiente de entrega", "Mi envío no se actualiza", "Verificar el estatus del paquete",
					"Mi paquete sigue en tránsito", "Seguimiento detenido", "¿Qué pasa con mi envío?",
					"Número de seguimiento inválido", "Estado no disponible", "Problema con el rastreo",
					"Mi pedido no tiene información", "No veo actualizaciones en el sistema",
					"El tracking muestra información errónea", "¿Está en aduanas mi paquete?",
					"Quiero saber si mi paquete está en tránsito", "Mi paquete no ha avanzado",
					"¿Cuánto tiempo tarda mi paquete en llegar?", "No puedo rastrear mi envío",
					"Rastreo detenido en aduanas", "Mi pedido está atrasado", "Consulta sobre el progreso del envío",
					"Quiero verificar el trayecto de mi paquete", "¿Por qué no aparece mi número de seguimiento?",
					"Mi envío muestra que está detenido", "Rastrear el paquete no funciona",
					"Mi pedido sigue en proceso", "No sé cómo rastrear mi paquete",

					// Variantes coloquiales o menos formales
					"¿Dónde está el paquete?", "El pedido no llega", "Quiero saber sobre mi paquete",
					"Rastreo del envío", "No tengo información de mi paquete", "Mi pedido está perdido",
					"El envío sigue parado", "Nada cambia en el seguimiento", "¿Me pueden ayudar a rastrear?",
					"El rastreo no sirve", "Estoy buscando mi pedido", "Mi paquete está atrasado",
					"No hay actualizaciones de mi pedido", "¿Por qué no cambia el estado?",

					// Frases contextuales relacionadas
					"¿Mi paquete está retenido en aduanas?", "El número de seguimiento no funciona",
					"¿Está en camino mi paquete?", "Ayuda con el estado de mi envío",
					"¿Mi paquete fue enviado correctamente?", "Problema con la ubicación del envío",
					"Seguimiento de mi pedido", "¿Cuándo llegará mi paquete?", "Seguimiento en aduanas",
					"El tracking no muestra nada", "No sé dónde está mi pedido"
				}
			}, { "INFORMACION_ENTREGAS_SEGUIMIENTOS", new List<string>
				{
					"entregado", "entrega", "recibido", "ya llegó", "confirmar entrega",
					"fecha de entrega", "días de entrega", "plazo de entrega", "entregaron",
					"ver si llegó", "ver entrega", "información de entrega", "hora de entrega", "¿Ya llegó mi paquete?", "Quiero confirmar la entrega de mi pedido",
					"¿Cómo sé si ya entregaron mi paquete?", "Consulta sobre entregas",
					"Mi pedido no fue entregado aún", "¿Cuándo se realizará la entrega?",
					"Necesito confirmar si ya fue recibido mi envío", "¿Me pueden notificar cuando llegue?",
					"El paquete debería estar entregado pero no tengo confirmación",
					"Consulta sobre el plazo de entrega"
				}
			}, { "INFORMACION_SOBRE_DOCUMENTOS", new List<string>
				{
					"documentos", "papeles", "trámites", "requisitos", "certificados",
					"formularios", "qué papeles", "qué necesito", "identificación",
					"documentación necesaria", "qué debo llevar", "solicitud de trámites",
					 "¿Qué documentos necesito para recoger mi paquete?",
			"Consulta sobre trámites para retirar un paquete",
			"¿Qué requisitos debo cumplir para el envío?",
			"Documentos necesarios para realizar un envío",
			"¿Qué papeles son obligatorios?", "Requisitos para recoger un pedido internacional",
			"Necesito información sobre la documentación", "¿Cuáles son las políticas de trámites?",
			"¿Qué identificaciones aceptan?", "¿Dónde puedo conseguir los formularios?"
				}
			}, { "QUEJAS_POR_IMPORTES", new List<string>
				{
					"impuesto", "tarifa", "monto", "pago", "importe", "cuota",
					"cargos", "costos adicionales", "dinero extra", "pago inesperado",
					"problema con pago", "gastos", "arancel", "cuánto tengo que pagar",
					"Tengo un problema con los impuestos de mi paquete", "Consulta sobre tarifas adicionales",
			"El costo de mi envío es más alto de lo informado",
			"Me cobraron una tarifa que no esperaba", "Problema con el monto a pagar",
			"¿Cuáles son los cargos extras?", "No entiendo los impuestos de mi pedido",
			"El pago no corresponde a lo acordado", "Reclamo por importes incorrectos",
			"Consulta sobre costos y pagos"
				}
			}, { "QUEJAS_POR_ESTAFA", new List<string>
				{
					"fraude", "estafa", "robado", "engañado", "falso", "problema legal",
					"engañaron", "reclamo por estafa", "me robaron", "me engañaron",
					"problema de fraude", "no confiable", "fraude con envío", "robo",
					 "Creo que me estafaron con el envío", "Problema de fraude con mi paquete",
			"Reclamo por un envío falso", "Me siento engañado por su servicio",
			"El envío no era legítimo", "Consulta por posible estafa",
			"El paquete parece haber sido robado", "Reclamo por estafa en el costo",
			"Me han robado el contenido del paquete", "Sospecho de un fraude con mi pedido"
				}
			}, { "QUEJAS_GENERALES", new List<string>
				{
					"queja", "reclamo", "problema", "inconformidad", "no estoy satisfecho",
					"fallo", "quejarse", "no me gusta", "inconveniente", "problemas con servicio",
					"mala experiencia", "quiero reportar", "mala atención", "problema en general",
					"Quiero poner una queja sobre su servicio", "Reclamo general sobre mi experiencia",
			"Estoy insatisfecho con la atención recibida", "Consulta sobre cómo presentar un reclamo",
			"Tengo problemas con su servicio", "Me gustaría expresar mi inconformidad",
			"El servicio no fue adecuado", "Reclamo por una mala experiencia",
			"Problema en general con la gestión del envío", "Quiero reportar un inconveniente"
				}
			}, { "CONSULTA_DE_HORARIOS", new List<string>
				{
					"horarios", "hora", "abren", "cierran", "apertura", "cierre",
					"horas de servicio", "disponibilidad", "qué horario tienen",
					"a qué hora trabajan", "a qué hora están disponibles", "agenda",
					 "¿A qué hora abren?", "Consulta sobre horario de atención",
			"¿Cuáles son los horarios de trabajo?", "¿A qué hora están disponibles?",
			"Horario de atención los fines de semana", "¿Cuándo abren los sábados?",
			"Consulta sobre horario de cierre", "¿Están abiertos hoy?",
			"Horario de atención durante feriados", "¿Cuándo puedo ir a la oficina?"
				}
			}, { "CONSULTA_DE_CONTACTO", new List<string>
				{
					"contactar", "teléfono", "correo", "hablar con", "dirección",
					"cómo ubicar", "redes sociales", "email", "llamar", "contacto directo",
					"número", "forma de contacto", "cómo comunicarse", "contactar servicio",
					  "¿Cómo puedo contactarlos?", "Consulta sobre teléfono de atención",
					"¿Tienen correo para contactarme?", "Necesito hablar con alguien del servicio al cliente",
					"¿Dónde puedo escribirles?", "Teléfono para consultas",
					"¿Cómo me comunico con ustedes?", "¿Tienen un número de contacto?",
					"Quiero información de contacto", "Consulta sobre formas de comunicación"
				}
			}, { "EVENTOS", new List<string>
				{
					"eventos", "actividades", "noticias", "publicidad", "anuncios", "ferias", "novedades",
					"convocatorias", "eventos estan proximos", "eventos proximos","actividades especiales", "qué eventos hay",
					"¿Tienen eventos este mes?", "Consulta sobre actividades recientes",
					"¿Hay algún evento especial en su oficina?", "Quiero información sobre campañas actuales",
					"Consulta sobre novedades", "¿Qué actividades tienen en marcha?",
					"Consulta sobre ferias o eventos especiales", "¿Ofrecen algo nuevo este mes?"
				}
			}, { "SOLICITUD_DE_ASISTENCIA", new List<string>
				{
					"ayuda", "asistencia", "soporte", "servicio al cliente", "necesito ayuda",
					"quiero ayuda", "problema técnico", "asistirme", "necesito soporte",
					"apoyo", "servicio de atención", "necesito hablar", "atención al cliente",
					"Consulta para obtener soporte",
					"Requiero asistencia con un problema", "¿Pueden ayudarme con esto?",
					"Quiero soporte técnico", "Ayuda con el servicio al cliente",
					"Necesito orientación sobre mi problema", "Quiero asistencia de inmediato",
					"Solicitud de soporte general", "Consulta sobre atención al cliente", "necesito hablar con un representante", "quiero atención personalizada",
					"servicio al cliente", "quiero asistencia de un agente",
					"necesito que alguien me atienda", "consulta con un agente de servicio",
					"quiero contactar a un representante", "necesito apoyo de atención al cliente",
					"hablar con soporte técnico", "soporte de servicio al cliente",
					"quiero comunicarme con un operador", "necesito un representante que me asista",
					"ayuda con servicio al cliente", "asistencia personalizada",
					"necesito contacto humano para resolver esto",
					"requiero asistencia directa", "problema que necesita atención personal",
					"¿pueden asignarme un agente?", "quiero hablar con alguien del equipo de soporte",
					"orientación de un representante", "atención por parte de un agente",
					"asistencia de un ser humano", "quiero soporte especializado"
				}
			}, { "INICIO", new List<string>
				{
					"hola", "buenos días", "buenas tardes", "buenas noches",
					"saludos", "qué tal", "cómo están", "hola, buen día",
					"hola, buenas", "hola a todos", "hola, ¿cómo están?",
					"buen día", "hola, necesito ayuda", "hola, ¿hay alguien?",
					"hola, estoy aquí", "hey", "buenas", "holi", "saludando",
					"hola equipo", "saludos cordiales", "hola, ¿me pueden atender?",
					"hola, tengo una consulta", "hola, un momento por favor",
					"hola, solo quería saber si están disponibles", "HI"
				}
			}
		};

	}
}