using Microsoft.AspNetCore.Mvc;
using CAPA_DATOS.Security;
using CAPA_DATOS;

namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ApiLogController : ControllerBase
    {
        [HttpPost]
        [AuthController]
        public object getLogError(Log Inst)
        {           
            return Inst.Get<Log>();
        }

    }
}
