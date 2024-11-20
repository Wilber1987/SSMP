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
			string? trackingNumber  = FindTrackingNumber(message);
			if (trackingNumber == null)
			{
				throw new ArgumentException("no contiene traking number");
			}
			return new TrackingHistory { Tracking = trackingNumber }.Get<TrackingHistory>();
		}
		public static string? FindTrackingNumber(string message)
		{
			// Regex para buscar tracking numbers
			string pattern = @"\b[A-Z]{2}\d{9}[A-Z]{2}\b";
			Regex regex = new Regex(pattern);

			// Busca el primer tracking number en el mensaje
			Match match = regex.Match(message);

			// Retorna el tracking number si encuentra alguno
			return match.Success ? match.Value : null;
		}
	}
}