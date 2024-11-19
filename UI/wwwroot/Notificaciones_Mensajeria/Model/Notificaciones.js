//@ts-check

//@ts-ignore
import { ModelProperty } from "../../WDevCore/WModules/CommonModel.js";
import { ModelFiles } from "../../WDevCore/WModules/CommonModel.js";
import { EntityClass } from "../../WDevCore/WModules/EntityClass.js";
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
   /**@type {Array<ModelFiles>}*/ Media;
   /**@type {boolean}*/ Enviado;
   /**@type {boolean}*/ Leido;
   /**@type {string}*/ Tipo;
    MarcarComoLeido = async () => {
        return await this.GetData("ApiNotificaciones/MarcarComoLeido");
    }
}
export { Notificaciones }
