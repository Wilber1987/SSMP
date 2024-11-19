//@ts-check
import { EntityClass } from "../../WDevCore/WModules/EntityClass.js";

class Contacto extends EntityClass {
    /** @param {Partial<Contacto>} [props] */
    constructor(props) {
        super(props, 'MessageManager');
        for (const prop in props) {
            this[prop] = props[prop];
        };
    }
    /**@type {Number}*/ Id_User;
    /**@type {String}*/ Nombre_Completo;
    /**@type {String}*/ Foto; 
    /**@type {Number}*/ Mensajes;  
}
export {Contacto}