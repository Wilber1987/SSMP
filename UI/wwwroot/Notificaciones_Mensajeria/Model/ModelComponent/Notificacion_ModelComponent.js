//@ts-check

//@ts-ignore
import { ModelProperty } from "../../../WDevCore/WModules/CommonModel.js";
import { EntityClass } from "../../../WDevCore/WModules/EntityClass.js";
class Notificaciones_ModelComponent extends EntityClass {
	/** @param {Partial<Notificaciones_ModelComponent>} [props] */
	constructor(props) {
		super(props, 'Notificaciones');
		for (const prop in props) {
			this[prop] = props[prop];
		}
	}
   /**@type {ModelProperty}*/ Id = { type: 'number', primary: true , hiddenFilter: true};
   /**@type {ModelProperty}*/ Mensaje = { type: 'richtext' };
   /**@type {ModelProperty}*/ Fecha = { type: 'date' };
   /**@type {ModelProperty}*/ Media = { type: 'masterdetail', hidden: true };
   /**@type {ModelProperty}*/ Enviado = { type: 'checkbox', hidden: true };
   /**@type {ModelProperty}*/ Leido = { type: 'checkbox', hidden: true };
   /**@type {ModelProperty}*/ Tipo = { type: 'wselect', hidden: true };
   /**@type {ModelProperty}*/ NotificationData = { type: 'Model', ModelObject: new Notificaciones_Data() };
   
	ReenvioNotificaciones = async (Notificaciones) => {
		return await this.SaveData("ApiNotificacionesManage/ReenvioNotificaciones", { Notificaciones: Notificaciones })
	}

}
export { Notificaciones_ModelComponent }

export class Notificaciones_Data {
	Departamento = { type: "text" };
	Direccion = { type: "text" };
	Destinatario = { type: "text" };
	Identificacion = { type: "text" };
	Correlativo = { type: "text" };
	Fecha = { type: "text" };
	Municipio = { type: "text" };
	Agencia = { type: "text" };
	Correo = { type: "text" };
	Telefono = { type: "text" };
	Dpi = { type: "text" };
	NumeroPaquete = { type: "text" };
	Reenvios = { type: "text" };
}
