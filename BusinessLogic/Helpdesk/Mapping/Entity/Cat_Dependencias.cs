using APPCORE;
using APPCORE.Security;
using API.Controllers;
using APPCORE.Services;

namespace CAPA_NEGOCIO.MAPEO;

public class Cat_Dependencias : EntityClass
{
	[PrimaryKey(Identity = true)]
	public int? Id_Dependencia { get; set; }
	public string? Descripcion { get; set; }
	public string? Username { get; set; }
	public string? Password { get; set; }
	public string? Host { get; set; }
	public int? Id_Dependencia_Padre { get; set; }
	public int? Id_Institucion { get; set; }
	public string? AutenticationType { get; set; }
	public string? HostService { get; set; }
	//AUTH 2.0
	public string? TENAT { get; set; }
	public string? CLIENT { get; set; }
	public string? OBJECTID { get; set; }
	public string? CLIENT_SECRET { get; set; }
	public string? SMTPHOST { get; set; }
	public bool? DefaultDependency { get; set; }


	[ManyToOne(TableName = "Cat_Dependencias", KeyColumn = "Id_Dependencia", ForeignKeyColumn = "Id_Dependencia_Padre")]
	public Cat_Dependencias? Cat_Dependencia { get; set; }
	[OneToMany(TableName = "Cat_Dependencias", KeyColumn = "Id_Dependencia", ForeignKeyColumn = "Id_Dependencia_Padre")]
	public List<Cat_Dependencias>? Cat_Dependencias_Hijas { get; set; }
	//[OneToMany(TableName = "Tbl_Case", KeyColumn = "Id_Dependencia", ForeignKeyColumn = "Id_Dependencia")]
	// public List<Tbl_Case>? Tbl_Case { get; set; }
	[OneToMany(TableName = "Tbl_Agenda", KeyColumn = "Id_Dependencia", ForeignKeyColumn = "Id_Dependencia")]
	public List<Tbl_Agenda>? Tbl_Agenda { get; set; }
	[OneToMany(TableName = "Tbl_Dependencias_Usuarios", KeyColumn = "Id_Dependencia", ForeignKeyColumn = "Id_Dependencia")]
	public List<Tbl_Dependencias_Usuarios>? Tbl_Dependencias_Usuarios { get; set; }
	public List<Cat_Dependencias> GetOwDependencies(string identity)
	{
		if (AuthNetCore.User(identity).isAdmin)
		{
			return Get<Cat_Dependencias>();
		}
		Tbl_Profile? profile = new Tbl_Profile() { IdUser = AuthNetCore.User(identity).UserId }.Find<Tbl_Profile>();
		Tbl_Dependencias_Usuarios Inst = new Tbl_Dependencias_Usuarios()
		{
			Id_Perfil = Tbl_Profile.GetUserProfile(identity)?.Id_Perfil
		};
		return Where<Cat_Dependencias>(FilterData.In(
			"Id_Dependencia",
			Inst.Get<Tbl_Dependencias_Usuarios>().Select(p => p.Id_Dependencia.ToString()).ToArray()
		));
	}
	[OneToMany(TableName = "Tbl_Servicios", KeyColumn = "Id_Dependencia", ForeignKeyColumn = "Id_Dependencia")]
	public List<Tbl_Servicios>? Tbl_Servicios { get; set; }

	public List<Cat_Dependencias> GetDependencias<T>()
	{
		return Get<Cat_Dependencias>().Select(m =>
		{
			m.Password = "PROTECTED";
			return m;
		}).ToList();
	}

	public object GetOwDependenciesConsolidados(string token)
	{
		return GetOwDependencies(token).Select(
			D =>
				new
				{
					D.Id_Dependencia,
					D.Descripcion,
					D.Username,
					D.Tbl_Dependencias_Usuarios,
					NCasos = new Tbl_Case().Count(
						FilterData.In("Id_Dependencia", D.Id_Dependencia),
						FilterData.In("Estado", Case_Estate.Activo.ToString())
					),
					NCasosFinalizados = new Tbl_Case { Id_Dependencia = D.Id_Dependencia, Estado = Case_Estate.Finalizado.ToString() }.Count()
				}
		).ToList();
	}

	public object? UpdateDependencies()
	{
		if (Tbl_Agenda != null && Tbl_Agenda.Count > 0)
		{
			new Tbl_Agenda { Id_Dependencia = this.Id_Dependencia }.Delete();
			foreach (var item in Tbl_Agenda)
			{
				item.IdAgenda = null;
			}
		}
		if (Tbl_Dependencias_Usuarios != null && Tbl_Dependencias_Usuarios.Count > 0)
		{
			new Tbl_Dependencias_Usuarios { Id_Dependencia = this.Id_Dependencia }.Delete();
			foreach (var item in Tbl_Dependencias_Usuarios)
			{
				item.Cat_Dependencias = null;
				item.Id_Dependencia = null;
				item.Id_Cargo = null;
				item.Id_Perfil = null;
			}
		}
		return this.Update();
	}

	public static void PrepareDefaultDependencys()
	{
		var DefaultDependencysList = Enum.GetValues(typeof(DefaultDependencys));
		foreach (DefaultDependencys defaultDependency in DefaultDependencysList)
		{
			var dep = new Cat_Dependencias { Descripcion = defaultDependency.ToString() }.Find<Cat_Dependencias>();
			if (dep == null)
			{
				List<Tbl_Servicios> servicios = [];
				if (defaultDependency == DefaultDependencys.DEFAULT)
				{
					foreach (var service in Enum.GetValues(typeof(DefaultServices_Default)))
					{
						servicios.Add(new Tbl_Servicios
						{
							Descripcion_Servicio = service.ToString(),
							Nombre_Servicio = service.ToString()
						});
					}

				}
				else if (defaultDependency == DefaultDependencys.CONSULTAS_SEGUIMIENTOS)
				{
					foreach (var service in Enum.GetValues(typeof(DefaultServices_DptConsultasSeguimientos)))
					{
						servicios.Add(new Tbl_Servicios
						{
							Descripcion_Servicio = service.ToString(),
							Nombre_Servicio = service.ToString()
						});
					}

				}
				else if (defaultDependency == DefaultDependencys.DEPARTAMENTO_DE_QUEJAS)
				{
					foreach (var service in Enum.GetValues(typeof(DefaultServices_DptQuejas)))
					{
						servicios.Add(new Tbl_Servicios
						{
							Descripcion_Servicio = service.ToString(),
							Nombre_Servicio = service.ToString()
						});
					}

				}
				new Cat_Dependencias
				{
					Descripcion = defaultDependency.ToString(),
					Tbl_Servicios = servicios
				}.Save();
			}
		}
	}
}
public enum DefaultDependencys
{
	DEFAULT,
	CONSULTAS_SEGUIMIENTOS,
	DEPARTAMENTO_DE_QUEJAS

}
public enum DefaultServices_Default
{
	ASISTENCIA_GENERAL,
	CONSULTA_DE_HORARIOS,
	CONSULTA_DE_CONTACTO,
	CONSULTA_SOBRE_EVENTOS,
	SOLICITUD_DE_ASISTENCIA
}
public enum DefaultServices_DptConsultasSeguimientos
{
	RASTREO_Y_SEGUIMIENTOS,
	INFORMACION_ENTREGAS_SEGUIMIENTOS,
	INFORMACION_SOBRE_DOCUMENTOS
}
public enum DefaultServices_DptQuejas
{
	QUEJAS_POR_RETRASOS,
	QUEJAS_POR_IMPORTES,
	QUEJAS_POR_ESTAFA,
	QUEJAS_GENERALES
}
