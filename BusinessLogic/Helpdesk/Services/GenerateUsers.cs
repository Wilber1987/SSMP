using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CAPA_NEGOCIO.MAPEO;


namespace SNI_UI2.CAPA_NEGOCIO.Helpdesk.Services
{


	// Mostrando la información de cada persona

	public class GenerateUsers
	{
		public static void Generate(string identfy)
		{
			
			Persona.personas.ForEach(p => 
			{
				if (new Security_Users{Mail = p.CorreoInstitucional}.Find<Security_Users>() != null)
				{
					return;
				}
				new Security_Users 
				{
					Nombres = $"{p.Nombres} {p.Apellidos}",
					Password = p.CorreoInstitucional,
					Mail = p.CorreoInstitucional,										
					Estado = "ACTIVO",
					Descripcion = $"Docente investigador del departamento de {p.Departamento}",
					Tbl_Profiles = [new Tbl_Profile
					{
						Nombres = p.Nombres,
						Apellidos = p.Apellidos,
						Sexo = p.Sexo,
						Correo_institucional = p.CorreoInstitucional,
						ORCID = p.ORCID, 
						Tbl_Dependencias_Usuarios =[new Tbl_Dependencias_Usuarios
						{
							Id_Dependencia = GetDependencia(p.Departamento)
						}],
						Estado = "ACTIVO",
						Foto ="\\Media\\profiles\\avatar.png",
					}], Security_Users_Roles = [ new APPCORE.Security.Security_Users_Roles { Id_Role = 4 } ]
				}.SaveUser(identfy);
			});
		}

		private static int? GetDependencia(string departamento)
		{
			if (departamento == "CTS")
			{
				return 3;
			} else if ( departamento == "C. HUMANIDADES") 
			{
				return 5;
			} else if (departamento == "C. ECONOMICAS")
			{
				return 4;
			} else if (departamento == "TIC") 
			{
				return 6;
			}
			return null;
		}
	}
	public class Persona
	{
		public string MarcaTemporal { get; set; }
		public string Nombres { get; set; }
		public string Apellidos { get; set; }
		public string Departamento { get; set; }
		public string Sexo { get; set; }
		public string CorreoInstitucional { get; set; }
		public string ORCID { get; set; }

		// Constructor opcional para inicializar la clase fácilmente
		public Persona(string marcaTemporal, string nombres, string apellidos, string departamento, string sexo, string correoInstitucional, string orcid)
		{
			MarcaTemporal = marcaTemporal;
			Nombres = nombres;
			Apellidos = apellidos;
			Departamento = departamento;
			Sexo = sexo;
			CorreoInstitucional = correoInstitucional;
			ORCID = orcid;
		}

		// Método opcional para mostrar la información
		public void MostrarInformacion()
		{
			Console.WriteLine($"Marca Temporal: {MarcaTemporal}");
			Console.WriteLine($"Nombres: {Nombres}");
			Console.WriteLine($"Apellidos: {Apellidos}");
			Console.WriteLine($"Departamento: {Departamento}");
			Console.WriteLine($"Sexo: {Sexo}");
			Console.WriteLine($"Correo Institucional: {CorreoInstitucional}");
			Console.WriteLine($"ORCID: {ORCID}");
		}
		public static List<Persona> personas = new List<Persona>
		{
			new Persona("10/10/2024 8:55:52", "Juan José", "Villavicencio Navarro", "CTS", "Masculino", "Juan.villavicencio@unan.edu.ni", ""),
			new Persona("10/10/2024 8:57:43", "Yader Ernesto", "Delgado González", "CTS", "Masculino", "ydelgado@unan.edu.ni", ""),
			new Persona("10/10/2024 9:00:45", "Ángela María", "Gutiérrez Cruz", "CTS", "Femenino", "angela.gutierrez@unan.edu.ni", "Xxxxx"),
			new Persona("10/10/2024 9:01:22", "Rita Catalina", "Matus Chau", "CTS", "Femenino", "rmatus@unan.edu.ni", ""),
			new Persona("10/10/2024 9:05:28", "Álvaro Antonio", "Mejía Quiroz", "CTS", "Masculino", "amejia@unan.edu.ni", "0000000257820961"),
			new Persona("10/10/2024 9:13:13", "Darío Benjamín", "Rodríguez Martínez", "CTS", "Masculino", "dbrodriguez@unan.edu.ni", "No tengo"),
			new Persona("10/10/2024 9:16:01", "Myurell Maryel", "Aburto Aguirre", "CTS", "Femenino", "mmaburto@unan.edu.ni", "0000-0001-6197-9759"),
			new Persona("10/10/2024 9:46:40", "Martha Elizabeth", "Rodríguez Sánchez", "CTS", "Femenino", "martha.rodriguez@unan.edu.ni", ""),
			new Persona("10/10/2024 10:09:27", "José Antonio", "Guido Chávez", "CTS", "Masculino", "jguido@unan.edu.ni", "https://orcid.org/0000-0001-9425-9782"),
			new Persona("10/10/2024 10:11:28", "Cristian Carolina", "José Sánchez", "C. HUMANIDADES", "Femenino", "cristian.jose@unan.edu.ni", "09669403"),
			new Persona("10/10/2024 10:58:59", "Jairo Martin de Jesus", "Gomez Palacio", "CTS", "Masculino", "jairo.gomez@unan.edu.ni", "https://orcid.org/0009-0001-9166-7067"),
			new Persona("10/10/2024 12:00:58", "Ner David", "Aráuz Carrillo", "CTS", "Masculino", "narauz@unan.edu.ni", "Ninguno"),
			new Persona("10/10/2024 14:18:35", "Harold Ramiro", "Gutiérrez Marcenaro", "CTS", "Masculino", "hrgutierrezm@unan.edu.ni", "0000-0003-1893-9975"),
			new Persona("10/10/2024 14:43:33", "Oscar Ramón", "Fletes Calderón", "CTS", "Masculino", "ofletes@unan.edu.ni", ""),
			new Persona("10/10/2024 14:46:01", "Álvaro Antonio", "Mejía Quiroz", "CTS", "Masculino", "amejia@unan.edu.ni", ""),
			new Persona("10/10/2024 14:50:30", "Marlon Javier", "Aguilar Rivera", "CTS", "Masculino", "maguilar@unan.edu.ni", ""),
			new Persona("10/10/2024 14:50:30", "Ixchel Arelly", "Lopez Selva", "CTS", "Femenino", "ixchel.lopez@unan.edu.ni", ""),
			new Persona("10/10/2024 14:50:30", "Wilber Jose", "Matus Gonzalez", "CTS", "Masculino", "wmatus@unan.edu.ni", "")
		};
	}

}