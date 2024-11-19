using CAPA_DATOS;
using CAPA_DATOS.Security;
using CAPA_NEGOCIO.MAPEO;
using CAPA_NEGOCIO.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
	[Route("api/[controller]/[action]")]
	[ApiController]
	public class ProyectController : ControllerBase
	{
		[HttpPost]
		[AuthController]
		public object TakeProyect(Tbl_Servicios Inst)
		{
			return Inst.Get<Tbl_Servicios>();
		}
		[HttpPost]
		[AuthController]
		public object TakeProyects(Tbl_Servicios Inst)
		{
			return Inst.Get<Tbl_Servicios>();
		}
		[HttpPost]
		[AuthController]
		public object TakeTypeProyects(Cat_Tipo_Servicio Inst)
		{
			return Inst.Get<Cat_Tipo_Servicio>();
		}
		[HttpPost]
		[AuthController]
		public List<Tbl_Tareas> GetOwParticipations(Tbl_Tareas inst)
		{
			return inst.GetOwParticipations(HttpContext.Session.GetString("seassonKey"));
		}
		[HttpPost]
		[AuthController]
		public List<Tbl_Case> GetOwCase(Tbl_Case inst)
		{
			return inst.GetOwCase(HttpContext.Session.GetString("seassonKey"));
		}
		
		[HttpPost]
		[AuthController]
		public List<Tbl_Case> GetOwCloseCase(Tbl_Case inst)
		{
			return inst.GetOwCloseCase(HttpContext.Session.GetString("seassonKey"));
		}
		[HttpPost]
		[AuthController]
		public List<Tbl_Case> GetVinculateCase(Tbl_Case inst)
		{
			return inst.GetVinculateCase(HttpContext.Session.GetString("seassonKey"));
		}
		[HttpPost]
		[AuthController]
		public List<Cat_Dependencias> GetOwDependencies(Cat_Dependencias inst)
		{
			return inst.GetOwDependencies(HttpContext.Session.GetString("seassonKey"));
		}
		//Pendiente, Solicitado, Activo, Finalizado, Espera
		[HttpPost]
		[AuthController]
		public List<Tbl_Case> GetOwSolicitudesPendientesAprobar(Tbl_Case inst)
		{
			return inst.GetOwSolicitudes(HttpContext.Session.GetString("seassonKey"), Case_Estate.Solicitado);
		}
		[HttpPost]
		[AuthController]
		public List<Tbl_Case> GetOwSolicitudesAprobadas(Tbl_Case inst)
		{
			return inst.GetOwSolicitudes(HttpContext.Session.GetString("seassonKey"), Case_Estate.Activo);
		}
		[HttpPost]
		[AuthController]
		public List<Tbl_Case> GetOwSolicitudesRechazadas(Tbl_Case inst)
		{
			return inst.GetOwSolicitudes(HttpContext.Session.GetString("seassonKey"), Case_Estate.Rechazado);
		}
		[HttpPost]
		[AuthController]
		public List<Tbl_Case> GetOwSolicitudesFinalizadas(Tbl_Case inst)
		{
			return inst.GetOwSolicitudes(HttpContext.Session.GetString("seassonKey"), Case_Estate.Finalizado);
		}
		[HttpPost]
		[AuthController]
		public List<Tbl_Case> GetOwSolicitudesVinculadas(Tbl_Case inst)
		{
			return inst.GetOwSolicitudes(HttpContext.Session.GetString("seassonKey"), Case_Estate.Vinculado);
		}

		[HttpPost]
		[AuthController]
		public List<Tbl_Case> GetOwSolicitudesPendientes(Tbl_Case inst)
		{
			return inst.GetOwSolicitudes(HttpContext.Session.GetString("seassonKey"), Case_Estate.Pendiente);
		}
		[HttpPost]
		[AuthController]
		public List<Tbl_Case> GetSolicitudesPendientesAprobar(Tbl_Case inst)
		{
			return inst.GetSolicitudesPendientesAprobar(HttpContext.Session.GetString("seassonKey"), Case_Estate.Solicitado);
		}
		[HttpPost]
		[AuthController]
		public List<Tbl_Case> GetSolicitudesPendientesAprobarAdmin(Tbl_Case inst)
		{
			return inst.GetSolicitudesPendientesAprobarAdmin(HttpContext.Session.GetString("seassonKey"));
		}
		[HttpPost]
		[AuthController]
		public List<Tbl_Case> GetOwSolicitudesProceso(Tbl_Case inst)
		{
			return inst.GetOwSolicitudes(HttpContext.Session.GetString("seassonKey"), Case_Estate.Activo);
		}
		[HttpPost]
		[AuthController]
		public List<Tbl_Case> GetOwSolicitudesEspera(Tbl_Case inst)
		{
			return inst.GetOwSolicitudes(HttpContext.Session.GetString("seassonKey"), Case_Estate.Espera);
		}
		[HttpPost]
		[AuthController(Permissions.ADMINISTRAR_CASOS_DEPENDENCIA)]

		public object RechazarSolicitud(Tbl_Case Tbl_Case)
		{
			return Tbl_Case.RechazarSolicitud(HttpContext.Session.GetString("seassonKey"));
		}

		[HttpPost]
		[AuthController(Permissions.ADMINISTRAR_CASOS_DEPENDENCIA)]

		public object AprobarSolicitud(Tbl_Case Tbl_Case)
		{
			return new CaseBlock().AprobarSolicitud(HttpContext.Session.GetString("seassonKey"), Tbl_Case);
		}
		[HttpPost]
		[AuthController(Permissions.ADMINISTRAR_CASOS_DEPENDENCIA, Permissions.TECNICO_CASOS_DEPENDENCIA)]
		public object CerrarCaso(Tbl_Case Tbl_Case)
		{
			return Tbl_Case.CerrarCaso(HttpContext.Session.GetString("seassonKey"));
		}
		//CASOS VINCULADOS      
		[HttpPost]
		[AuthController(Permissions.ADMINISTRAR_CASOS_DEPENDENCIA)]

		public object VincularCaso(Tbl_VinculateCase inst)
		{
			return inst.VincularCaso();
		}
		[HttpPost]
		[AuthController(Permissions.ADMINISTRAR_CASOS_DEPENDENCIA)]

		public object DesvincularCaso(Tbl_VinculateCase inst)
		{
			return inst.DesvincularCaso(inst.Casos_Vinculados.FirstOrDefault());
		}
		public List<Tbl_Case> GetCasosToVinculate(Tbl_Case Inst)
		{
			return new Tbl_Case()
			.GetCasosToVinculate(HttpContext.Session.GetString("seassonKey"), Inst);
		}
		[HttpPost]
		[AuthController(Permissions.ADMINISTRAR_CASOS_DEPENDENCIA)]
		public object AprobarCaseList(CaseBlock Inst)
		{
			return Inst.AprobarSolicitudes(HttpContext.Session.GetString("seassonKey"));
		}
		[HttpPost]
		[AuthController(Permissions.ADMINISTRAR_CASOS_DEPENDENCIA)]
		public object RechazarCaseList(CaseBlock Inst)
		{
			return Inst.RechazarSolicitudes(HttpContext.Session.GetString("seassonKey"));
		}
		[HttpPost]
		[AuthController(Permissions.ADMINISTRAR_CASOS_DEPENDENCIA)]
		public object RemitirCasos(CaseBlock Inst)
		{
			return Inst.RemitirCasos(HttpContext.Session.GetString("seassonKey"));
		}
		[HttpPost]
		[AuthController(Permissions.PERFIL_MANAGER)]
		public object AsignarDependencias(ProfileTransaction Inst)
		{
			return Inst.AsignarDependencias(HttpContext.Session.GetString("seassonKey"));
		}
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
		[HttpPost]
		[AuthController]
		public object getDashboardgET(DateFilter dateFilter)
		{
			string? token = HttpContext.Session.GetString("seassonKey");
			var caseTable = new Tbl_Case
			{
				filterData = new List<FilterData> { FilterData.Between("Fecha_Inicio", dateFilter.Desde, dateFilter.Hasta) }
			}.GetOwParticipantCase(token);
			return new
			{
				dependencies = new Cat_Dependencias().GetOwDependenciesConsolidados(token),
				caseTickets = caseTable,
				task = new Tbl_Tareas()
				{
					filterData = new List<FilterData> { FilterData.Between("Fecha_Inicio", dateFilter.Desde, dateFilter.Hasta) }
				}.GetOwActiveParticipations(token),
				comments = new Tbl_Comments()
				{
					filterData = new List<FilterData> { FilterData.Between("Fecha", dateFilter.Desde, dateFilter.Hasta) }
				}.GetOwComments(caseTable)
			};
		}
		[HttpPost]
		[AuthController]
		public object SaveOwCase(Tbl_Case Inst)
		{
			string? token = HttpContext.Session.GetString("seassonKey");
			return Inst.SaveOwCase(token);
		}

	}
	public class DateFilter
	{
		public DateTime Desde { get; set; }
		public DateTime Hasta { get; set; }
	}
}
