using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CAPA_DATOS;

namespace BusinessLogic.IA.Model
{
	public class Tbl_Pronts: EntityClass
	{
		public string? Pront_Line { get; set; }
		public string? Type { get; set; }
	}
	public enum ProntTypes 
	{
		GENERAL, QUEJAS, SEGUIMIENTO
	} 
}