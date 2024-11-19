


using CAPA_DATOS;
using CAPA_NEGOCIO.MAPEO;

namespace CAPA_NEGOCIO.Services
{
	public class CaseBlock : TransactionalClass
	{
		public Cat_Dependencias? dependencia { get; set; }
		public Tbl_Servicios? servicio { get; set; }
		public List<Tbl_Comments>? comentarios { get; set; }
		public List<Tbl_Case> Tbl_Cases { get; set; }
		public object AprobarSolicitudes(string? v)
		{
			try
			{
				BeginGlobalTransaction();
				foreach (var item in Tbl_Cases)
				{
					item.Tbl_Servicios = servicio;
					item.AprobarSolicitud(v);
				}
				CommitGlobalTransaction();
				return new ResponseService()
				{
					status = 200,
					message = "Solicitudes aprobadas"
				};
			}
			catch (System.Exception e)
			{
				RollBackGlobalTransaction();
				return new ResponseService()
				{
					status = 500,
					message = e.Message
				};
			}
		}

		public object RechazarSolicitudes(string? v)
		{
			try
			{
				BeginGlobalTransaction();
				foreach (var item in Tbl_Cases)
				{
					item.Tbl_Comments = comentarios;
					item.RechazarSolicitud(v);
				}
				CommitGlobalTransaction();
				return new ResponseService()
				{
					status = 200,
					message = "Solicitudes rechazadas"
				};
			}
			catch (System.Exception e)
			{
				RollBackGlobalTransaction();
				return new ResponseService()
				{
					status = 500,
					message = e.Message
				};
			}

		}

		public object RemitirCasos(string? v)
		{
			try
			{
				BeginGlobalTransaction();
				foreach (var item in Tbl_Cases)
				{
					item.Cat_Dependencias = dependencia;
					item.Tbl_Servicios = servicio;
					item.CreateAsignationsByService(null);
					item.Update();
				}
				CommitGlobalTransaction();
				return new ResponseService()
				{
					status = 200,
					message = "Solicitudes rechazadas"
				};
			}
			catch (System.Exception e)
			{
				RollBackGlobalTransaction();
				return new ResponseService()
				{
					status = 500,
					message = e.Message
				};
			}




		}

		public object AprobarSolicitud(string? v, Tbl_Case caseP)
		{
			try
			{
				BeginGlobalTransaction();
				caseP.AprobarSolicitud(v);
				CommitGlobalTransaction();
				return new ResponseService()
				{
					status = 200,
					message = "Solicitudes aprobadas"
				};
			}
			catch (System.Exception e)
			{
				RollBackGlobalTransaction();
				return new ResponseService()
				{
					status = 500,
					message = e.Message
				};
			}
		}


	}
	public class ProfileTransaction : TransactionalClass
	{
		public Cat_Dependencias? dependencia { get; set; }
		public List<Tbl_Profile>? perfiles { get; set; }


		public object AsignarDependencias(string? v)
		{
			try
			{
				BeginGlobalTransaction();
				foreach (var item in perfiles)
				{
					if (item.Tbl_Dependencias_Usuarios
						.Where(x => x.Id_Dependencia == dependencia.Id_Dependencia).ToList().Count == 0)
					{
						new Tbl_Dependencias_Usuarios()
						{
							Id_Dependencia = dependencia.Id_Dependencia,
							Id_Perfil = item.Id_Perfil
						}.Save();
					}

				}
				CommitGlobalTransaction();
				return new ResponseService()
				{
					status = 200,
					message = "Asignación de dependencias exitoso."
				};
			}
			catch (System.Exception e)
			{
				RollBackGlobalTransaction();
				return new ResponseService()
				{
					status = 500,
					message = e.Message
				};
			}


		}
	}

}
