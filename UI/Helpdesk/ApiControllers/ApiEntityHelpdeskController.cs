using DataBaseModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CAPA_NEGOCIO.MAPEO;
using API.Controllers;
using System.Collections.Generic;
using System.Linq;
using CAPA_NEGOCIO.Services;
using CAPA_DATOS.Security;

namespace API.Controllers
{
	[Route("api/[controller]/[action]")]
	[ApiController]
	public class ApiEntityHelpdeskController : ControllerBase
	{
		[HttpPost]
		[AuthController]
		public List<Tbl_Comments> getTbl_Comments(Tbl_Comments Inst)
		{
			return Inst.GetComments();
		}
		[HttpPost]
		[AuthController]
		public object? saveTbl_Comments(Tbl_Comments inst)
		{
			return inst.SaveComment(HttpContext.Session.GetString("seassonKey"));
		}
		[HttpPost]
		[AuthController]
		public object? updateTbl_Comments(Tbl_Comments inst)
		{
			return inst.Update();
		}
		[HttpPost]
		[AuthController]
		public List<Tbl_Comments_Tasks> getTbl_Comments_Tasks(Tbl_Comments_Tasks Inst)
		{
			return Inst.GetComments();
		}
		[HttpPost]
		[AuthController]
		public object? saveTbl_Comments_Tasks(Tbl_Comments_Tasks inst)
		{
			return inst.SaveComment(HttpContext.Session.GetString("seassonKey"), false);
		}
		[HttpPost]
		[AuthController]
		public object? updateTbl_Comments_Tasks(Tbl_Comments_Tasks inst)
		{
			return inst.Update();
		}
		//Cat_Tipo_Evento

		[HttpPost]
		[AuthController]
		public List<Cat_Tipo_Evidencia> getCat_Tipo_Evidencia(Cat_Tipo_Evidencia Inst)
		{
			return Inst.Get<Cat_Tipo_Evidencia>();
		}
		[HttpPost]
		[AuthController]
		public object? saveCat_Tipo_Evidencia(Cat_Tipo_Evidencia inst)
		{
			return inst.Save();
		}
		[HttpPost]
		[AuthController]
		public object? updateCat_Tipo_Evidencia(Cat_Tipo_Evidencia inst)
		{
			return inst.Update();
		}
		//Cat_Cargos_Dependencias
		[HttpPost]
		[AuthController]
		public List<Cat_Cargos_Dependencias> getCat_Cargos_Dependencias(Cat_Cargos_Dependencias Inst)
		{
			return Inst.Get<Cat_Cargos_Dependencias>();
		}
		[HttpPost]
		[AuthController]
		public object? saveCat_Cargos_Dependencias(Cat_Cargos_Dependencias inst)
		{
			return inst.Save();
		}
		[HttpPost]
		[AuthController]
		public object? updateCat_Cargos_Dependencias(Cat_Cargos_Dependencias inst)
		{
			return inst.Update();
		}
		//Cat_Dependencias
		[HttpPost]
		[AuthController]
		public List<Cat_Dependencias> getCat_Dependencias(Cat_Dependencias Inst)
		{
			return Inst.GetDependencias<Cat_Dependencias>();
		}
		[HttpPost]
		[AuthController]
		public object? saveCat_Dependencias(Cat_Dependencias inst)
		{
			return inst.Save();
		}
		[HttpPost]
		[AuthController]
		public object? updateCat_Dependencias(Cat_Dependencias inst)
		{
			return inst.UpdateDependencies();
		}
		//Cat_Tipo_Participaciones
		[HttpPost]
		[AuthController]
		public List<Cat_Tipo_Participaciones> getCat_Tipo_Participaciones(Cat_Tipo_Participaciones Inst)
		{
			return Inst.Get<Cat_Tipo_Participaciones>();
		}
		[HttpPost]
		[AuthController]
		public object? saveCat_Tipo_Participaciones(Cat_Tipo_Participaciones inst)
		{
			return inst.Save();
		}
		[HttpPost]
		[AuthController]
		public object? updateCat_Tipo_Participaciones(Cat_Tipo_Participaciones inst)
		{
			return inst.Update();
		}
		//Tbl_Agenda
		[HttpPost]
		[AuthController]
		public List<Tbl_Agenda> getTbl_Agenda(Tbl_Agenda Inst)
		{
			return Inst.Get<Tbl_Agenda>();
		}
		[HttpPost]
		[AuthController]
		public object? saveTbl_Agenda(Tbl_Agenda inst)
		{
			return inst.Save();
		}
		[HttpPost]
		[AuthController]
		public object? updateTbl_Agenda(Tbl_Agenda inst)
		{
			return inst.Update();
		}
		//Tbl_Dependencias_Usuarios
		[HttpPost]
		[AuthController]
		public List<Tbl_Dependencias_Usuarios> getTbl_Dependencias_Usuarios(Tbl_Dependencias_Usuarios Inst)
		{
			return Inst.Get<Tbl_Dependencias_Usuarios>();
		}
		[HttpPost]
		[AuthController]
		public object? saveTbl_Dependencias_Usuarios(Tbl_Dependencias_Usuarios inst)
		{
			return inst.Save();
		}
		[HttpPost]
		[AuthController]
		public object? updateTbl_Dependencias_Usuarios(Tbl_Dependencias_Usuarios inst)
		{
			return inst.Update();
		}
		//Tbl_Evidencias
		[HttpPost]
		[AuthController]
		public List<Tbl_Evidencias> getTbl_Evidencias(Tbl_Evidencias Inst)
		{
			return Inst.Get<Tbl_Evidencias>();
		}
		[HttpPost]
		[AuthController]
		public object? saveTbl_Evidencias(Tbl_Evidencias inst)
		{
			return inst.Save();
		}
		[HttpPost]
		[AuthController]
		public object? updateTbl_Evidencias(Tbl_Evidencias inst)
		{
			return inst.Update();
		}
		//Tbl_Datos_Laborales
		//Cat_Paises
		[HttpPost]
		[AuthController]
		public List<Cat_Paises> getCat_Paises(Cat_Paises Inst)
		{
			return Inst.Get<Cat_Paises>();
		}
		[HttpPost]
		[AuthController]
		public object? saveCat_Paises(Cat_Paises inst)
		{
			return inst.Save();
		}
		[HttpPost]
		[AuthController]
		public object? updateCat_Paises(Cat_Paises inst)
		{
			return inst.Update();
		}
		//Cat_TipoLocalidad


		//Tbl_Profile
		[HttpPost]
		[AuthController(Permissions.PERFIL_MANAGER)]
		public List<CAPA_NEGOCIO.MAPEO.Tbl_Profile> getTbl_Profile(CAPA_NEGOCIO.MAPEO.Tbl_Profile Inst)
		{
			return Inst.GetProfiles(HttpContext.Session.GetString("seassonKey"));
		}
		[HttpPost]
		[AuthController(Permissions.PERFIL_MANAGER)]
		public object? saveTbl_Profile(CAPA_NEGOCIO.MAPEO.Tbl_Profile inst)
		{
			return inst.SaveProfile();
		}
		[HttpPost]
		[AuthController(Permissions.PERFIL_MANAGER)]
		public object? updateTbl_Profile(CAPA_NEGOCIO.MAPEO.Tbl_Profile inst)
		{
			inst.IdUser = AuthNetCore.User(HttpContext.Session.GetString("seassonKey")).UserId;
			return inst.SaveProfile();
		}

		//Cat_Tipo_Servicio
		[HttpPost]
		[AuthController]
		public List<Cat_Tipo_Servicio> getCat_Tipo_Servicio(Cat_Tipo_Servicio Inst)
		{
			return Inst.Get<Cat_Tipo_Servicio>();
		}
		[HttpPost]
		[AuthController]
		public object? saveCat_Tipo_Servicio(Cat_Tipo_Servicio inst)
		{
			return inst.Save();
		}
		[HttpPost]
		[AuthController]
		public object? updateCat_Tipo_Servicio(Cat_Tipo_Servicio inst)
		{
			return inst.Update();
		}
		//Tbl_Servicios
		[HttpPost]
		[AuthController]
		public List<Tbl_Servicios> getTbl_Servicios(Tbl_Servicios Inst)
		{
			return Inst.Get<Tbl_Servicios>();
		}
		[HttpPost]
		[AuthController]
		public object? saveTbl_Servicios(Tbl_Servicios inst)
		{
			return inst.Save();
		}
		[HttpPost]
		[AuthController]
		public object? updateTbl_Servicios(Tbl_Servicios inst)
		{
			return inst.Update();
		}

		//Tbl_Case
		[HttpPost]
		[AuthController]
		public List<Tbl_Case> getTbl_Case(Tbl_Case Inst)
		{
			return Inst.Get<Tbl_Case>();
		}
		[HttpPost]
		[AuthController]
		public object? saveTbl_Case(Tbl_Case inst)
		{
			inst.Estado ??= Case_Estate.Solicitado.ToString();
			inst.Id_Perfil = AuthNetCore.User(HttpContext.Session.GetString("seassonKey")).UserId;
			inst.Mail = AuthNetCore.User(HttpContext.Session.GetString("seassonKey")).mail;
			inst.Titulo = inst?.Titulo?.ToUpper();
			return inst.Save();
		}
		[HttpPost]
		[AuthController(Permissions.ADMINISTRAR_CASOS_DEPENDENCIA)]
		public object? updateTbl_Case(Tbl_Case inst)
		{
			return inst.Update();
		}
		//Tbl_Calendario
		[HttpPost]
		[AuthController]
		public List<ViewCalendarioByDependencia> getTbl_Calendario(ViewCalendarioByDependencia Inst)
		{
			return Inst.Get<ViewCalendarioByDependencia>();
		}
		[HttpPost]
		[AuthController]
		public object? saveTbl_Calendario(Tbl_Calendario inst)
		{
			return inst.Save();
		}
		[HttpPost]
		[AuthController]
		public object? updateTbl_Calendario(Tbl_Calendario inst)
		{
			return inst.Update();
		}
		//Tbl_Tareas
		[HttpPost]
		[AuthController]
		public List<Tbl_Tareas> getTbl_Tareas(Tbl_Tareas Inst)
		{
			return Inst.Get<Tbl_Tareas>();
		}
		[HttpPost]
		[AuthController(Permissions.GESTOR_TAREAS)]
		public object? saveTbl_Tareas(Tbl_Tareas inst)
		{
			return inst.SaveTareaWithTransaction(HttpContext.Session.GetString("seassonKey"));
		}
		[HttpPost]
		[AuthController]
		public object? updateTbl_Tareas(Tbl_Tareas inst)
		{
			return inst.UpdateTarea();
		}
		//Tbl_Participantes
		[HttpPost]
		[AuthController]
		public List<Tbl_Participantes> getTbl_Participantes(Tbl_Participantes Inst)
		{
			return Inst.Get<Tbl_Participantes>();
		}
		[HttpPost]
		[AuthController]
		public object? saveTbl_Participantes(Tbl_Participantes inst)
		{
			return inst.Save();
		}
		[HttpPost]
		[AuthController]
		public object? updateTbl_Participantes(Tbl_Participantes inst)
		{
			return inst.Update();
		}
		//Cat_Cargos
		[HttpPost]
		[AuthController]
		public List<Tbl_VinculateCase> getTbl_VinculateCase(Tbl_VinculateCase inst)
		{
			return inst.Get<Tbl_VinculateCase>();
		}
		[HttpPost]
		[AuthController]
		public object? saveTbl_VinculateCase(Tbl_VinculateCase inst)
		{
			return inst.Save();
		}
		[HttpPost]
		[AuthController]
		public object? updateTbl_VinculateCase(Tbl_VinculateCase inst)
		{
			return inst.Update();
		}
		[HttpPost]
		[AuthController]
		public List<Tbl_Profile_CasosAsignados> getTbl_Profile_CasosAsignados(Tbl_Profile_CasosAsignados inst)
		{
			return inst.Get<Tbl_Profile_CasosAsignados>();
		}


		#region INVESTIGACIONES y grupos de trabajo
		[HttpPost]
		[AuthController]
		public List<Tbl_Grupo> getTbl_Grupo(Tbl_Grupo Inst)
		{
			return Inst.Get<Tbl_Grupo>();
		}
		[HttpPost]
		[AuthController]
		public List<Tbl_Grupo> findTbl_Grupo(Tbl_Grupo Inst)
		{
			return Inst.Get<Tbl_Grupo>();
		}
		[HttpPost]
		[AuthController]
		public object? saveTbl_Grupo(Tbl_Grupo inst)
		{
			return inst.SaveGroup(HttpContext.Session.GetString("seassonKey"));
		}
		[HttpPost]
		[AuthController]
		public object? updateTbl_Grupo(Tbl_Grupo inst)
		{
			return inst.UpdateGroup(HttpContext.Session.GetString("seassonKey"));
		}
		[HttpPost]
		[AuthController]
		public object? getTbl_Grupos_Profile(Tbl_Grupos_Profile inst)
		{
			return inst.Get<Tbl_Grupos_Profile>();
		}
		
		[HttpPost]
		[AuthController]
		public object? saveTbl_Grupos_Profile(Tbl_Grupos_Profile inst)
		{
			return inst.SaveGroup(HttpContext.Session.GetString("seassonKey"));
		}
		[HttpPost]
		[AuthController]
		public object? updateTbl_Grupos_Profile(Tbl_Grupos_Profile inst)
		{
			return inst.UpdateGroup(HttpContext.Session.GetString("seassonKey"));
		}
		#endregion

	}
}
