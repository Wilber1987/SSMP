using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Controllers;
using Microsoft.AspNetCore.Mvc;
using CAPA_NEGOCIO.MAPEO;

namespace SNI_UI2.CAPA_NEGOCIO.Helpdesk.ApiControllers
{
	[ApiController]
	[Route("api/[controller]/[action]")]
	public class ApiEntityHelpdeskPublicController : ControllerBase
	{
		[HttpPost]
		[AuthController]
		public List<Tbl_Profile> getTbl_Profile(Tbl_Profile Inst)
		{
			Inst.Estado = "ACTIVO";
			return Inst.SimpleGet<Tbl_Profile>();
		}
	}
}