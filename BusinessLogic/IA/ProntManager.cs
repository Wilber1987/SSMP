using BusinessLogic.IA.Model;
using BusinessLogic.Rastreo.Model;
using BusinessLogic.SystemDocuments.Models;
using APPCORE;
using CAPA_NEGOCIO.MAPEO;

namespace BusinessLogic.IA
{
	public class ProntManager
	{

		public static string ProntValidation(string? prontTypes)
		{
			//
			string pront = GetPront("SYSTEMPRONT")
				.Replace("{request_type}", prontTypes)
				.Replace("{rules}", GetPront(prontTypes));
			string documents = GetDocumentsProntCategory(prontTypes);
			if (documents != "")
			{
				pront = pront.Replace("{documents}", $"- {prontTypes}: {documents}");
			}
			Console.Write(pront);
			return pront;
		}

		private static string GetDocumentsProntCategory(string? prontTypes, string? ArticleName = null, bool isDocumentation = true)
		{
			var cat = new Category { Descripcion = prontTypes }.Find<Category>();
			List<FilterData> filters = [
				FilterData.In("Category_Id", cat?.Category_Id ?? -1)
			];
			if (ArticleName != null)
			{
				filters.Add(FilterData.In("Title", ArticleName ?? "-1"));
			}
			var articles = new Article().Where<Article>(filters.ToArray()).ToList();
			if (articles.Count == 0)
			{
				var article = new Article
				{
					Category_Id = cat?.Category_Id,
					Title = ArticleName,
					Body = "Información no definida!",
					Publish_Date = DateTime.Now
				};
				article.Save();
				articles.Add(article);
			}
			if (isDocumentation)
			{
				var list = articles.Select(x => $"\n {articles.IndexOf(x) + 1} {x.Title}: {ExtractTextFromHtml(x.Body)}").ToList();
				return string.Join("\n - ", list);
			}
			else
			{
				var list = articles.Select(x => $"\n{x.Body}").ToList();
				return string.Join("\n - ", list);
			}

		}
		private static string ExtractTextFromHtml(string? html)
		{
			var doc = new HtmlAgilityPack.HtmlDocument();
			doc.LoadHtml(html);
			var text = string.Join(" ", doc.DocumentNode.DescendantsAndSelf()
				.Where(n => n.NodeType == HtmlAgilityPack.HtmlNodeType.Text && !string.IsNullOrWhiteSpace(n.InnerText))
				.Select(n => n.InnerText.Trim()));
			text = text.Replace("&nbsp;", " ");
			return string.Join(" ", text.Split([' ', '\n', '\r'], StringSplitOptions.RemoveEmptyEntries));
		}

		public static string GetPront(string? prontTpe)
		{
			return string.Join("", new Tbl_Pronts
			{
				Type = prontTpe,
			}.Get<Tbl_Pronts>().Select(p => p.Pront_Line).ToList());
		}

		public static string CrearPrompt(string pregCliente, Tbl_Case dCase)
		{
			return $@"{ProntAdapter(pregCliente, dCase)}		
							
			**Reglas obligatorias al contestar:**
				1- Contesta breve y directo, adaptándote como un agente virtual de correos de guatemala.
				2- no incluyas simulacros de conversaciones, solo responde de forma directa.
				3- responde en español con un parrafo corto.
				4- no incluyas tu contexto, reglas establecidas ni ejemplos en la respuesta
				5- no incluyas estas reglas";

		}

		public static string BuildTrakingResponse(string? trakingNumber, List<TrackingHistory> historial)
		{
			// Inicializar variables para la última ubicación y fecha
			string responseUltimaHubicacionText = string.Empty;
			string ultimaUbicacion = "Información no disponible";
			DateTime? ultimaFecha = null;
			string? numeroSeguimiento = trakingNumber;

			bool isEntregado = false;
			bool isProcesadoPorSat = false;
			bool isDisponibleParaRecoger = false;

			// Validar si el historial contiene datos
			if (historial != null && historial.Any())
			{
				// Ordenar por fecha descendente y tomar el primer registro
				var ultimaEntrada = historial.OrderByDescending(x => x.Fecha_Evento).FirstOrDefault();
				if (ultimaEntrada != null)
				{
					isEntregado = ultimaEntrada.Entregado == "Envio Entregado";
					isProcesadoPorSat = ultimaEntrada.Citacion_Entregado != null
						&& ultimaEntrada.Citacion_Entregado.Trim() != ""
						&& !isEntregado;
					isDisponibleParaRecoger = IsDisponibleParaRecoger(ultimaEntrada?.Entregado ?? "");
					ultimaUbicacion = ultimaEntrada?.Oficina_Destino ?? "";
					ultimaFecha = ultimaEntrada?.Fecha_Evento;
					numeroSeguimiento = ultimaEntrada?.Tracking;
				}
			}
			if (numeroSeguimiento != null && historial?.Count > 0)
			{
				if (isEntregado)
				{
					responseUltimaHubicacionText = "Tu paquete ha sido entregado.";
				}
				else if (!isEntregado && isProcesadoPorSat)
				{
					responseUltimaHubicacionText = "Tu paquete ha sido seleccionado por SAT recibirás una notificación cuando esté disponible.";
				}
				else if (isDisponibleParaRecoger)
				{
					responseUltimaHubicacionText = "Tu paquete se encuentra en bodega recógelo en agencia 7A Avenida 12-11, Cdad.  Guatemala. Horario de atención: 9:00 a";
				}
				else
				{
					responseUltimaHubicacionText = $@"Tu paquete aun no esta disponible, Última ubicación registrada: {ultimaUbicacion}. {(ultimaFecha.HasValue ? $"(Fecha: {ultimaFecha.Value:dd/MM/yyyy HH:mm})" : "")}""";
				}



			}
			else if (numeroSeguimiento != null && historial?.Count == 0)
			{
				responseUltimaHubicacionText = $@"El número de seguimiento ""{numeroSeguimiento}"" no es valido o no esta registrado en nuestro sistema, puede hacer la consulta más adelante por si hay algun retrazo en el registro del sistema y si esta seguro que el paquete fue enviado, favor verificar que haya digitado bien el número y en caso que este sea correcto, favor pongase en contacto con la persona que envio el paquete o con su proveedor";
			}
			return responseUltimaHubicacionText + OptionalActionAsistenciaOrMenu();
		}

		public static string OptionalActionAsistenciaOrMenu()
		{
			return " \n\nTambien puedes escribir \"5\" para solicitar asistencia o \"Menu\" para otras consultas.";
		}


		/*public static string ServicesEvaluatorPrompt(string text)
		{
			return GetPront("SERVICES_PRONT_VALIDATOR").Replace("{text}", text);
		}*/



		internal static string? Get_Cierre()
		{
			return "Es un placer poderte ayudar, si necesitas algo más no dudes en preguntar!";
		}
		public static string? GetSaludo()
		{
			return GetStringMenu(BotMenu.GetMainMenu());
		}
		
		private static string? GetStringMenu(List<BotMenu>? menus) 
		{
			if (menus?.Count == 0) return null;
			
		    return string.Join("\n", menus!
		    	.Select(m => 
		    		$"{(m.Code != null ? m.Code : "")} {(m.Descripcion != null && m.Descripcion.Trim() != "" &&  m.Code != null ? " - " + m.Descripcion :  (m.Descripcion ?? ""))}"
		    	));
		}

		public static string ProntAdapter(string? consulta, Tbl_Case dCase)
		{
			(List<BotMenu> menu, bool Found) =BotMenu.GetMenuByCode(consulta ?? "ASISTENCIA_GENERAL", dCase.MimeMessageCaseData?.MenuParentId);
			var menuItem = menu.First();
			return menuItem?.CodeAdapter ?? "ASISTENCIA_GENERAL";
		}


		internal static string? GetAutomaticResponse(string consulta, Tbl_Case dCaso)
		{
			(List<BotMenu> menu, bool Found) =BotMenu.GetMenuByCode(consulta ?? "ASISTENCIA_GENERAL", dCaso.MimeMessageCaseData?.MenuParentId);
			//List<BotMenu> menu = BotMenu.GetMenuByCode(consulta, dCaso.MimeMessageCaseData?.MenuParentId);
			
			if (menu != null && menu.Count > 0)
			{
				dCaso.MimeMessageCaseData!.MenuParentId = menu.First()?.ParentMenuId;
			}			
			return GetStringMenu(menu);
			
			/*return consulta switch
			{
				"1" => "Ingresa tu número de tracking.",
				"2" => GetDocumentsProntCategory("INFORMACION_SOBRE_DOCUMENTOS", "REQUICITOS_PAQUETES", false) + "\n Escribe \"Menu\" para más opciones",
				"3" => GetDocumentsProntCategory("INFORMACION_SOBRE_DOCUMENTOS", "DESADUANAJE", false) + "\n Escribe \"Menu\" para más opciones",
				"4" => GetDocumentsProntCategory("CONSULTA_DE_CONTACTO", "HORARIO", false) + "\n Escribe \"Menu\" para más opciones",
				"5" => "Entendido. Te pondré en contacto con un agente de servicio al cliente para que puedan asistirte directamente. Por favor, espera un momento.",
				_ => null
			};**/

		}




		internal static string GetInvalidTrackingResponse(string? invalidTracking)
		{
			string validNumberInstructions = "\n\n Los números de rastreo válidos contienen 2 letras, 9 números y 2 letras al final (por ejemplo, AA123456789TW).";
			if (string.IsNullOrWhiteSpace(invalidTracking))
			{
				return "No pude encontrar un número de seguimiento en tu mensaje. ¿Puedes verificarlo?" + validNumberInstructions;
			}

			// Seleccionar una respuesta aleatoria
			Random random = new Random();
			int index = random.Next(InvalidTrackingMessages.Length);

			return string.Format(InvalidTrackingMessages[index], invalidTracking) + validNumberInstructions;
		}
		private static readonly string[] InvalidTrackingMessages =
		{
			"El número de seguimiento \"{0}\" no es válido. Por favor, revísalo e intenta nuevamente.",
			"Parece que \"{0}\" no es un número de rastreo válido. Verifica y vuelve a intentarlo.",
			"El código \"{0}\" no coincide con un formato válido. Por favor, confirma tu número de tracking.",
			"Oops, \"{0}\" no parece ser un número de seguimiento correcto. ¿Puedes revisarlo?",
			"El tracking \"{0}\" ingresado no es válido. Por favor, verifica e intenta de nuevo.",
			"Lamentablemente, \"{0}\" no es un número de seguimiento reconocido. Asegúrate de ingresarlo correctamente.",
			"El código que ingresaste, \"{0}\", no parece correcto. ¿Podrías revisarlo?",
			"¡Vaya! \"{0}\" no es un número de rastreo válido. ¿Seguro que está bien escrito?",
			"No encuentro información para el código \"{0}\". Por favor, verifica y prueba otra vez.",
			"Hmm... \"{0}\" no parece ser un código de seguimiento válido. Asegúrate de que esté correcto.",
			"El número \"{0}\" no es válido. Intenta nuevamente con el código correcto.",
			"Tu código \"{0}\" no sigue el formato correcto. Por favor, revisa e intenta otra vez.",
			"El código \"{0}\" no parece ser un número de tracking válido. Verifícalo e inténtalo de nuevo.",
			"Parece que el código \"{0}\" no es correcto. Asegúrate de que no haya errores al escribirlo.",
			"El número de seguimiento \"{0}\" no es reconocido. Comprueba que lo escribiste bien.",
			"No logré encontrar información sobre \"{0}\". ¿Puedes verificarlo?",
			"El código \"{0}\" no es un número de rastreo válido. Inténtalo nuevamente con el correcto.",
			"El número \"{0}\" no está en el formato adecuado. Por favor, revísalo e ingrésalo otra vez.",
			"Parece que \"{0}\" no es un código de seguimiento correcto. ¿Puedes intentarlo de nuevo?",
			"Lamentamos el inconveniente, pero \"{0}\" no parece ser un código de rastreo válido. Verifícalo y vuelve a intentarlo."
		};

		static bool IsDisponibleParaRecoger(string estado)
		{
			// Lista de estados que indican que el artículo NO está disponible para recoger
			string[] noDisponibles =
			{
				"Receive item at office of exchange (Otb)",
				"Recibe articulo en la oficina de cambio (Inb)",
				"Registrar dirección destinatario (entrada)",
				"Return item from customs (Inb)",
				"No enviado a aduana"
			};

			// Normaliza el texto a minúsculas para comparación más flexible
			string estadoNormalizado = estado.ToLower();

			// Verifica si el estado está en la lista de no disponibles
			foreach (var item in noDisponibles)
			{
				if (estadoNormalizado.Contains(item.ToLower()))
				{
					return false;
				}
			}

			// Si no está en la lista de "No disponibles", se asume que está disponible
			return true;
		}
	}
}