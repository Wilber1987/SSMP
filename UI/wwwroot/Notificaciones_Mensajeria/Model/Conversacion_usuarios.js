//@ts-check
// @ts-ignore
import { Security_Users } from '../../WDevCore/Security/SecurityModel.js';
import { EntityClass } from '../../WDevCore/WModules/EntityClass.js';
import { Conversacion } from './Conversacion.js';
class Conversacion_usuarios extends EntityClass {
    /** @param {Partial<Conversacion_usuarios>} [props] */
    constructor(props) {
        super(props, 'MessageManager');
        for (const prop in props) {
            this[prop] = props[prop];
        }
    }
    /**@type {Number}*/ Id_conversacion;
    /**@type {Number}*/ Id_usuario;
    /**@type {String} */ Name;
    /**@type {String} */ Avatar;
    /**@type {Conversacion} ManyToOne*/ Conversacion;
    /**@type {Security_Users} ManyToOne*/ Security_Users;
}
export { Conversacion_usuarios }
