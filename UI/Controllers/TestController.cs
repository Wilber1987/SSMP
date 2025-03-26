using Microsoft.AspNetCore.Mvc;
using APPCORE.Security; 
using CAPA_NEGOCIO.Services;
using APPCORE.Services;

namespace API.Controllers
{
	[Route("api/[controller]/[action]")]
	[ApiController]
	public class TestController : ControllerBase
	{
		[HttpGet]
		public object getMailData(){
			return null;
		}
		[HttpGet]
		public object getMailData2(){
			testIamp.RequestTokenAsync();
			return true;
		}
	 
	}
}

