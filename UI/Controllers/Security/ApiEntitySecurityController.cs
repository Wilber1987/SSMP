using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using APPCORE.Security;
using System.Collections.Generic;
using CAPA_NEGOCIO.MAPEO;

namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ApiEntitySecurityController : ControllerBase
    {
        #region SECURITY
        [HttpPost]
        [AdminAuth]
        public Object getSecurity_Permissions(Security_Permissions inv) { return inv.Get<Security_Permissions>(); }
        [HttpPost]
        [AdminAuth]
        public Object? getSecurity_Roles(Security_Roles inv) { return inv.GetRoles(); }
        [HttpPost]
        [AdminAuth]
        public Object getSecurity_Users(CAPA_NEGOCIO.MAPEO.Security_Users inv) { return inv.GetUsers(); }

        [HttpPost]
        [AdminAuth]
        public Object? saveSecurity_Permissions(Security_Permissions inv) { return inv.Save(); }
        [HttpPost]
        [AdminAuth]
        public Object saveSecurity_Roles(Security_Roles inv) { return inv.SaveRole(); }
        [HttpPost]
        [AdminAuth]
        public Object saveSecurity_Users(CAPA_NEGOCIO.MAPEO.Security_Users inv)
        {
            return inv.SaveUserT(HttpContext.Session.GetString("sessionKey"));
        }

        [HttpPost]
        [AdminAuth]
        public Object updateSecurity_Permissions(Security_Permissions inv) { return inv.Update("Id_Permission"); }

        public Object changePassword(CAPA_NEGOCIO.MAPEO.Security_Users inv) { return inv.changePassword(HttpContext.Session.GetString("sessionKey")); }

        #endregion
    }
}
