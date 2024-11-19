using Microsoft.AspNetCore.Mvc;
using CAPA_NEGOCIO.Views;
using CAPA_NEGOCIO.MAPEO;

namespace API.Controllers
{
	[Route("api/[controller]/[action]")]
	[ApiController]
	public class CalendarController : ControllerBase
	{
		[HttpPost]
		[AuthController]
		public Object TakeData()
		{
			List<object> data = new List<object>();
			Cat_Dependencias Dep = new Cat_Dependencias();
			data.Add(Dep.Get<Cat_Dependencias>());
			Tbl_Profile Users = new Tbl_Profile();
			data.Add(Users.Get<Tbl_Profile>());
			Users.Id_Perfil = AuthNetCore.User(HttpContext.Session.GetString("seassonKey")).UserId;
			data.Add(Users.TakeDepCoordinaciones());
			return data;
		}
		[HttpPost]
		[AuthController]
		public Object TakeCalendar(Calendar cal)
		{
			cal.TakeActividades();
			cal.TakeAgenda();
			cal.TakeCalendario();
			return cal;
		}
		[HttpPost]
		[AuthController]
		public Object TakeActividades(Tbl_Case Inst)
		{            
			return Inst.Get<Tbl_Case>();
		}
		[HttpPost]
		[AuthController]
		public Object TakeActividad(Tbl_Case Inst)
		{
			return Inst.GetActividad();
		}
		[HttpPost]
		[AuthController]
		public Object SaveActividad(Tbl_Case Act)
		{            
			return true;
		}
		[HttpPost]
		[AuthController]
		public Object? SaveTarea(Tbl_Tareas Act)
		{
			return Act.Save();
		}
		[HttpPost]
		[AuthController]
		public Object SolicitarActividad(Tbl_Case Act)
		{
			return Act.SolicitarActividades(HttpContext.Session.GetString("seassonKey"));
		}
		//Agenda por usuario
		[HttpPost]
		[AuthController]
		public Object AgendaUsuarioDependencia(Cat_Dependencias Act)
		{
			Tbl_Agenda ag = new Tbl_Agenda();
			ag.Id_Dependencia = Act.Id_Dependencia;
			ag.Id_Perfil = AuthNetCore.User(HttpContext.Session.GetString("seassonKey")).UserId;
			return ag.Get<Tbl_Agenda>();
		}
		[HttpPost]
		[AuthController]
		public Object? SaveAgendaUsuarioDependencia(Tbl_Agenda Act)
		{
			if (Act.IdAgenda != null)
			{
				return Act.Update("IdAgenda");
			}
			return Act.Save();
		}
	}
	public class Calendar
	{
		public int Id_Dependencia { get; set; }
		public int IdUsuario { get; set; }
		public Object? Actividades { get; set; }
		public Object? Agenda { get; set; }
		public Object? Calendario { get; set; }

		public void TakeAgenda()
		{
			Tbl_Agenda ag = new Tbl_Agenda();
			ag.Id_Dependencia = this.Id_Dependencia;
			this.Agenda = ag.Get<Tbl_Agenda>();
		}
		public void TakeActividades()
		{
			Tbl_Case ag = new Tbl_Case();
			this.Actividades = ag.Get<Tbl_Case>();
		}
		public void TakeCalendario()
		{
			ViewCalendarioByDependencia ag = new ViewCalendarioByDependencia();
			ag.Id_Dependencia = this.Id_Dependencia;
			this.Calendario = ag.Get<ViewCalendarioByDependencia>();
		}
	}
}
