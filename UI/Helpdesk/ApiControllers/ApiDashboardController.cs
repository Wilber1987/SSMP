using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Controllers;
using CAPA_NEGOCIO.MAPEO;
using CAPA_NEGOCIO.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
	[ApiController]
	[Route("api/[controller]/[action]")]
	public class ApiDashboardController : ControllerBase
	{
		[HttpPost]
		[AuthController]
		public object getDashboard(ProfileTransaction Inst)
		{
			string? token = HttpContext.Session.GetString("seassonKey");
			var caseTable = new Tbl_Case().GetOwParticipantCase(token);
			return new
			{
				dependencies = new Cat_Dependencias().GetOwDependenciesConsolidados(token),
				caseTickets = caseTable,
				task = new Tbl_Tareas().GetOwActiveParticipations(token),
				comments = new Tbl_Comments().GetOwComments(caseTable)
			};
		}
	}
}