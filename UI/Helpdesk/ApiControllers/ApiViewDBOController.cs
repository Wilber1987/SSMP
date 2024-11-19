using DataBaseModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using API.Controllers;

namespace API.Controllers {
   [Route("api/[controller]/[action]")]
   [ApiController]
   public class  ApiViewDBOController : ControllerBase {
       //ViewInvestigacionesDisciplinas
       
       //ViewParticipantesServicios
       [HttpPost]
       [AuthController]
       public List<ViewParticipantesServicios> getViewParticipantesServicios(ViewParticipantesServicios Inst) {
           return Inst.Get<ViewParticipantesServicios>();
       }
       //ViewCalendarioByDependencia
       [HttpPost]
       [AuthController]
       public List<ViewCalendarioByDependencia> getViewCalendarioByDependencia(ViewCalendarioByDependencia Inst) {
           return Inst.Get<ViewCalendarioByDependencia>();
       }
       //ViewActividadesParticipantes
       [HttpPost]
       [AuthController]
       public List<ViewActividadesParticipantes> getViewActividadesParticipantes(ViewActividadesParticipantes Inst) {
           return Inst.Get<ViewActividadesParticipantes>();
       }
   }
}
