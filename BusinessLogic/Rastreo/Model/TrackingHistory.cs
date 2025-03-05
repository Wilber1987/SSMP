using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CAPA_DATOS;

namespace BusinessLogic.Rastreo.Model
{
	public class TrackingHistory : QueryClass
	{
		public TrackingHistory()
		{
			this.MDataMapper = new BDConnection().DataMapperSeguimiento;
		}
		public string? Tracking { get; set; }
		public string? Nombre_Destinatario { get; set; }
		public string? Numero_de_Telefono { get; set; }
		public string? Direccion { get; set; }
		public string? Peso { get; set; }
		public string? Categoria { get; set; }
		public string? Pais_Origen { get; set; }
		public string? Citacion_Entregado { get; set; }
		public DateTime? Fecha_Evento { get; set; }
		public string? Type { get; set; }
		public string? Oficina_Destino { get; set; }
		public string? Entregado_misc1 { get; set; }
		public string? Entregado { get; set; }

		public override List<TrackingHistory> Get<TrackingHistory>()
		{
			var dt = this.MDataMapper?.GDatos.TraerDatosSQL(GetQuery()) ;
			if (dt != null && dt.Rows.Count > 0) 
			{
				return AdapterUtil.ConvertDataTable<TrackingHistory>(dt, this);
			} else 
			{
				return [];	
			}
			
		}

		public override string GetQuery()
		{
			if (Tracking == null)
			{
				throw new ArgumentNullException($"Traking number not found is null");
			}
			return @"SELECT
				L_MAILITMS.MAILITM_FID AS Tracking,
				L_MAILITM_CUSTOMERS.CUSTOMER_NAME AS [Nombre_Destinatario],
				L_MAILITM_CUSTOMERS.CUSTOMER_PHONE_NO AS [Numero_de_Telefono],
				L_MAILITM_CUSTOMERS.CUSTOMER_ADDRESS AS Direccion,
				L_MAILITMS.MAILITM_WEIGHT AS Peso,
				C_MAIL_CLASSES.MAIL_CLASS_NM AS Categoria,
				C_COUNTRIES.COUNTRY_NM AS [Pais_Origen],
				L_MAILITMS.CUSTOMS_NO AS [Citacion_Entregado],
				L_MAILITMS.EVT_GMT_DT AS [Fecha_Evento],
				L_MAILITMS.EVT_TYPE_CD as Type,
				N_OWN_OFFICES.OFFICE_NM AS [Oficina_Destino],
				L_MAILITMS.MISC1 AS Entregado_misc1,
				C_EVENT_TYPES.EVENT_TYPE_NM AS Entregado
				FROM L_MAILITMS
				INNER JOIN C_MAIL_CLASSES ON L_MAILITMS.MAIL_CLASS_CD = C_MAIL_CLASSES.MAIL_CLASS_CD
				INNER JOIN N_OWN_OFFICES ON L_MAILITMS.EVT_OFFICE_CD = N_OWN_OFFICES.OWN_OFFICE_CD
				LEFT OUTER JOIN C_EVENT_TYPES ON L_MAILITMS.EVT_TYPE_CD = C_EVENT_TYPES.EVENT_TYPE_CD
				LEFT OUTER JOIN C_COUNTRIES ON L_MAILITMS.ORIG_COUNTRY_CD = C_COUNTRIES.COUNTRY_CD
				LEFT OUTER JOIN L_MAILITM_CUSTOMERS ON L_MAILITMS.MAILITM_PID = L_MAILITM_CUSTOMERS.MAILITM_PID
				WHERE L_MAILITMS.MAILITM_FID = '" + Tracking + "'";
		}

        internal void TestConection()
        {
            this.MDataMapper.GDatos.TestConnection();
        }
    }
}