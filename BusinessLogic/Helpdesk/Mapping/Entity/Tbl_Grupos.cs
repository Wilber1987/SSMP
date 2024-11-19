using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Controllers;
using CAPA_DATOS;
using CAPA_DATOS.Security;

namespace CAPA_NEGOCIO.MAPEO
{
	public class Tbl_Grupos_Profile : EntityClass
	{
		[PrimaryKey]
		public int? Id_Grupo { get; set; }
		[PrimaryKey]
		public int? Id_Perfil { get; set; }
		public DateTime? Fecha_Incorporacion { get; set; }
		public string? Estado { get; set; }

		[ManyToOne(TableName = "Tbl_Grupo", KeyColumn = "Id_Grupo", ForeignKeyColumn = "Id_Grupo")]
		public Tbl_Grupo? Tbl_Grupo { get; set; }

		[ManyToOne(TableName = "Tbl_Profile", KeyColumn = "Id_Perfil", ForeignKeyColumn = "Id_Perfil")]
		public Tbl_Profile? Tbl_Profile { get; set; }

		public object? SaveGroup(string? identity)
		{
			var grupo = new Tbl_Grupos_Profile { Id_Grupo = Id_Grupo ?? Tbl_Grupo?.Id_Grupo, Id_Perfil = Id_Perfil ?? Tbl_Profile?.Id_Perfil }.SimpleFind<Tbl_Grupos_Profile>();
			if (grupo != null)
			{
				return new ResponseService { status = 403, message = "Tu relación con este grupo ya existe : ESTADO - " + grupo?.Estado?.ToString() };
			}

			if (Estado == GroupState.INVITADO.ToString())
			{
				//Tbl_Profile? profileInvitado = new Tbl_Profile { Id_Perfil =  Id_Perfil }.Find<Tbl_Profile>();
				InvitarInvestigador(Tbl_Profile);
				return new ResponseService { status = 200, message = "Invitación Enviada" };
			}
			else if (Estado == GroupState.SOLICITANTE.ToString())
			{
				UserModel user = AuthNetCore.User(identity);
				Tbl_Profile? profile = new Tbl_Profile { IdUser = user.UserId }.Find<Tbl_Profile>();
				SolicitarUnirse(profile);
				return new ResponseService { status = 200, message = "Solicitud Enviada" };
			}
			return new ResponseService { status = 403, message = "Estado invalido" };
		}

		public object? UpdateGroup(string? identity)
		{
			//UserModel user = AuthNetCore.User(identity);
			//Tbl_Profile? profile = new Tbl_Profile { IdUser = user.UserId }.Find<Tbl_Profile>();
			if (Estado == GroupState.RECHAZADO.ToString())
			{
				RechazarSolicitud(this);
				return new ResponseService { status = 200, message = "Solicitud Rechazada" };
			}
			else if (Estado == GroupState.ACTIVO.ToString())
			{
				AprobarSolicitud(this);
				return new ResponseService { status = 200, message = "Solicitud Aprobada" };
			}
			else if (Estado == GroupState.INACTIVO.ToString())
			{
				AbandonarGrupo(this);
				return new ResponseService { status = 200, message = "Salida del grupo exitosa" };
			}
			return new ResponseService { status = 403, message = "Estado invalido" };
		}

		private void AbandonarGrupo(Tbl_Grupos_Profile Inv)
		{
			Inv.Estado = GroupState.INACTIVO.ToString();
			//Inv.Fecha_Incorporacion = DateTime.Now;
			Inv.Update();
		}

		public ResponseService AprobarSolicitud(Tbl_Grupos_Profile Inv)
		{
			Inv.Estado = GroupState.ACTIVO.ToString();
			Inv.Fecha_Incorporacion = DateTime.Now;
			Inv.Update();
			return new ResponseService
			{
				status = 200,
				message = "Success"
			};
		}
		public ResponseService RechazarSolicitud(Tbl_Grupos_Profile Inv)
		{
			Inv.Estado = GroupState.RECHAZADO.ToString();
			Inv.Update();
			return new ResponseService
			{
				status = 200,
				message = "Success"
			};
		}
		public ResponseService InvitarInvestigador(Tbl_Profile Inv)
		{
			Tbl_Grupos_Profile tbl_InvestigadoresAsociado = BuildInvAsociado(Inv, GroupState.INVITADO.ToString());
			tbl_InvestigadoresAsociado.Save();
			//ENVIAR CORREO
			return new ResponseService
			{
				status = 200,
				message = "Success"
			};
		}
		public ResponseService SolicitarUnirse(Tbl_Profile Inv)
		{
			Tbl_Grupos_Profile tbl_InvestigadoresAsociado = BuildInvAsociado(Inv, GroupState.SOLICITANTE.ToString());
			tbl_InvestigadoresAsociado.Save();
			return new ResponseService
			{
				status = 200,
				message = "Success"
			};
		}
		private Tbl_Grupos_Profile BuildInvAsociado(Tbl_Profile Inv, string Estado)
		{
			Tbl_Grupos_Profile tbl_InvestigadoresAsociado = new Tbl_Grupos_Profile();
			tbl_InvestigadoresAsociado.Id_Grupo = Id_Grupo ?? Tbl_Grupo?.Id_Grupo;
			tbl_InvestigadoresAsociado.Id_Perfil = Id_Perfil ?? Inv.Id_Perfil;
			tbl_InvestigadoresAsociado.Estado = Estado;
			return tbl_InvestigadoresAsociado;
		}
	}


	public class Tbl_Grupo : EntityClass
	{
		[PrimaryKey(Identity = true)]
		public int? Id_Grupo { get; set; }
		public string? Nombre { get; set; }
		public int? Id_Perfil_Crea { get; set; }
		public int? Id_TipoGrupo { get; set; }
		public DateTime? Fecha_Creacion { get; set; }
		public string? Estado { get; set; }
		public string? Descripcion { get; set; }
		public string? Color { get; set; }

		[ManyToOne(TableName = "Tbl_Profile", KeyColumn = "Id_Perfil", ForeignKeyColumn = "Id_Perfil_Crea")]
		public Tbl_Profile? Tbl_Profile { get; set; }
		[ManyToOne(TableName = "Cat_Tipo_Grupo", KeyColumn = "Id_Tipo_Grupo", ForeignKeyColumn = "Id_Tipo_Grupo")]
		public Cat_Tipo_Grupo? Cat_Tipo_Grupo { get; set; }

		[OneToMany(TableName = "Tbl_Grupos_Profile", KeyColumn = "Id_Grupo", ForeignKeyColumn = "Id_Grupo")]
		public List<Tbl_Grupos_Profile>? Tbl_Grupos_Profiles { get; set; }

		public object? SaveGroup(string? identity)
		{
			if (AuthNetCore.HavePermission(identity, Permissions.ADMIN_ACCESS))
			{
				return Save();
			}
			UserModel user = AuthNetCore.User(identity);
			Tbl_Profile? profile = new Tbl_Profile { IdUser = user.UserId }.Find<Tbl_Profile>();
			Id_Perfil_Crea = profile?.Id_Perfil;
			Estado = GroupState.ACTIVO.ToString();
			Tbl_Grupos_Profiles = [new Tbl_Grupos_Profile {
				Id_Perfil = profile?.Id_Perfil,Estado = GroupState.ACTIVO.ToString(),Fecha_Incorporacion = DateTime.Now
			}];
			Save();
			return this;
		}

		public object? UpdateGroup(string? identity)
		{
			UserModel user = AuthNetCore.User(identity);
			Tbl_Profile? profile = new Tbl_Profile { IdUser = user.UserId }.Find<Tbl_Profile>();
			var grupo = new Tbl_Grupo { Id_Grupo = Id_Grupo, Id_Perfil_Crea = profile?.Id_Perfil }.Find<Tbl_Grupo>();
			if (grupo != null && grupo.Id_Perfil_Crea == profile?.Id_Perfil)
			{
				if (Estado == GroupState.INACTIVO.ToString())
				{
					//TODO: CAMBIAR ESTADO A INACTIVO DE LOS MIEMBROS
				}
				Update();
				return new ResponseService
				{
					status = 200,
					message = "Grupo actualizado correctamente"
				};
			}
			return new ResponseService
			{
				status = 403,
				message = "Error, el grupo no existe o no tiene acceso a cambiar modificarlo."
			};
		}
	}

	public class Cat_Tipo_Grupo : EntityClass
	{
		[PrimaryKey(Identity = true)]
		public int? Id_Tipo_Grupo { get; set; }
		public string? Descripcion { get; set; }
		public string? Estado { get; set; }
		// [OneToMany(TableName = "Tbl_Grupo", KeyColumn = "Id_Tipo_Grupo", ForeignKeyColumn = "Id_Tipo_Grupo")]
		public List<Tbl_Grupo>? Tbl_Grupo { get; set; }
	}

	public enum GroupState
	{
		ACTIVO, INACTIVO, RECHAZADO, INVITADO, SOLICITANTE
	}

}