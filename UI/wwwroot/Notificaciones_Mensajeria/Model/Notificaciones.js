//@ts-check

//@ts-ignore
import { ModelProperty } from "../../WDevCore/WModules/CommonModel.js";
import { ModelFiles } from "../../WDevCore/WModules/CommonModel.js";
import { EntityClass } from "../../WDevCore/WModules/EntityClass.js";
import { DateTime } from "../../WDevCore/WModules/Types/DateTime.js";
class Notificaciones extends EntityClass {
	/** @param {Partial<Notificaciones>} [props] */
	constructor(props) {
		super(props, 'Notificaciones');
		for (const prop in props) {
			this[prop] = props[prop];
		}
	}
	/**@type {Number}*/ Id;
	/**@type {String}*/ Titulo;
	/**@type {String}*/ Mensaje;
	/**@type {Date}*/ Fecha;
	/**@type {Date}*/ Fecha_Envio;
	/**@type {Array<ModelFiles>}*/ Media;
	/**@type {boolean}*/ Enviado;
	/**@type {boolean}*/ Leido;
	/**@type {string}*/ Tipo;
	/**@type {string}*/ Month;
	/**@type {string}*/ Year;
   	/**@type {NotificationData}*/ NotificationData;

	//get Month() {return new DateTime(this.Fecha).getMonthFormatEs();}
	//get Year() {return new DateTime(this.Fecha).getFullYear();}
	MarcarComoLeido = async () => {
		return await this.GetData("ApiNotificaciones/MarcarComoLeido");
	}
	GetParam(value) {
		return this.NotificationData?.Params?.find(p => p.Name.toLowerCase() == value.toLowerCase())?.Value ?? "";
	}
}
export { Notificaciones }

export class NotificationData {
	/**@type {String} */Departamento;
	/**@type {String} */Direccion;
	/**@type {String} */Destinatario;
	/**@type {String} */Identificacion;
	/**@type {String} */Correlativo;
	/**@type {String} */Fecha;
	/**@type {String} */Municipio;
	/**@type {String} */Agencia;
	/**@type {String} */Correo;
	/**@type {String} */Telefono;
	/**@type {String} */Dpi;
	/**@type {String} */Nit;
	/**@type {String} */NumeroPaquete;
	/**@type {String} */NumeroAduana;
	/**@type {Number}*/ Reenvios
	/**@type {Array<NotificationsParams>} */  Params;
}

export class NotificationsParams {
	/**@type {String} */ Name;
	/**@type {String} */ Type;
	/**@type {String} */ Value;
}