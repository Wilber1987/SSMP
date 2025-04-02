using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Controllers;
using APPCORE;
using APPCORE.Security;
using CAPA_NEGOCIO.Gestion_Mensajeria;
using DataBaseModel;
using Microsoft.AspNetCore.Mvc;

namespace UI.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ApiMessageManagerController : ControllerBase
    {
        [HttpPost]
        [AuthController(Permissions.SEND_MESSAGE)]
        public List<Contacto> getContacto(Contacto Inst)
        {           
            return new Conversacion().GetContactos(HttpContext.Session.GetString("sessionKey"), Inst);
        }
        //Mensajes
        [HttpPost]
		[AuthController(Permissions.SEND_MESSAGE)]
		public List<Mensajes> getMensajes(Conversacion Inst)
		{
			return new Mensajes().GetMessage(HttpContext.Session.GetString("sessionKey"), Inst);
		}
        [HttpPost]
        [AuthController(Permissions.SEND_MESSAGE)]
        public ResponseService saveMensajes(Mensajes Inst)
        {
            return Inst.SaveMessage(HttpContext.Session.GetString("sessionKey"));
        }    
        [HttpPost]
        [AuthController(Permissions.SEND_MESSAGE)]
        public Conversacion? findConversacion(Conversacion Inst)
        {
            return Inst.FindConversacion(HttpContext.Session.GetString("sessionKey"));
        }        
        [HttpPost]
        [AuthController(Permissions.SEND_MESSAGE)]
        public List<Conversacion> getConversacion(Contacto Inst)
        {
            return Conversacion.GetConversaciones(HttpContext.Session.GetString("sessionKey"), Inst);
        }
    }
}