﻿using CAPA_NEGOCIO.MAPEO;
using APPCORE.Security; 
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        [HttpPost]
        [AuthController]
        //Save
        public object? SaveProfile(CAPA_NEGOCIO.MAPEO.Tbl_Profile Inst)
        {
            Inst.IdUser = AuthNetCore.User(HttpContext.Session.GetString("sessionKey")).UserId;
            return Inst.SaveProfile();
        }
        [HttpPost]
        [AuthController]
        //Save
        public object? TakeProfile(CAPA_NEGOCIO.MAPEO.Tbl_Profile Inst)
        {
            Inst.Id_Perfil = null;
            Inst.IdUser = AuthNetCore.User(HttpContext.Session.GetString("sessionKey")).UserId;
            return Inst.TakeProfile();
        }

        [HttpGet]
        [AuthController]
        //Save
        public object? TakeProfile2()
        {
            CAPA_NEGOCIO.MAPEO.Tbl_Profile Inst = new()
            {
                IdUser = AuthNetCore.User(HttpContext.Session.GetString("sessionKey")).UserId
            };
            return Inst.TakeProfile();
        }

    }
}
