using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BusinessLogic.Rastreo.Model;

namespace BusinessLogic.Rastreo.Operations
{


	public class TrackingOperation
	{
		public static List<TrackingHistory> TrackingHistories(string message)
		{
			string? trackingNumber = FindTrackingNumber(message);
			if (trackingNumber == null)
			{
				throw new ArgumentException("No contiene un número de tracking válido.");
			}
			return new TrackingHistory { Tracking = trackingNumber }.Get<TrackingHistory>();
		}

		public static string? FindTrackingNumber(string message)
		{
			// Regex para buscar posibles tracking numbers
			string pattern = @"\b[A-Z]{2}\d{9}[A-Z]{2}\b";
			Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);

			Match match = regex.Match(message);

			// Verifica si se encontró un código y si es válido
			if (match.Success && IsValidTrackingNumber(match.Value))
			{
				return match.Value.ToUpper(); // Devuelve en mayúsculas si es válido
			}

			return null; // Retorna null si no se encuentra un tracking válido
		}

		public static (bool, string?) FindInvalidCode(string message)
		{
			// Regex para encontrar posibles códigos sospechosos
			string pattern = @"\b[A-Z]?\d{5,12}[A-Z]?\b"; // Detecta secuencias con letras y números
			Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);

			MatchCollection matches = regex.Matches(message);

			foreach (Match match in matches)
			{
				string code = match.Value;
				if (!IsValidTrackingNumber(code))
				{
					return (true, code); // Se encontró un código incorrecto
				}
			}
			return (false, null); // No se detectaron códigos incorrectos
		}

		// Método de validación de tracking numbers correctos
		private static bool IsValidTrackingNumber(string tracking)
		{
			// Verifica si cumple estrictamente con el formato 2L + 9N + 2L
			return Regex.IsMatch(tracking, @"^[A-Z]{2}\d{9}[A-Z]{2}$", RegexOptions.IgnoreCase);
		}

	}
}