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
   /**@type {ModelProperty}*/ Id = { type: 'number', primary: true };
   /**@type {ModelProperty}*/ Mensaje = { type: 'richtext' };
   /**@type {ModelProperty}*/ Fecha = { type: 'date' };
   /**@type {ModelProperty}*/ Media = { type: 'masterdetail', hidden: true };
   /**@type {ModelProperty}*/ Enviado = { type: 'checkbox', hidden: true  };
   /**@type {ModelProperty}*/ Leido = { type: 'checkbox', hidden: true };
   /**@type {ModelProperty}*/ Tipo = { type: 'wselect', hidden: true };
}
export {Notificaciones_ModelComponent}
