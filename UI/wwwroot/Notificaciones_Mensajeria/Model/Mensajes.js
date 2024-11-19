//@ts-check
import { Security_Users } from '../../WDevCore/Security/SecurityModel.js';
import { EntityClass } from '../../WDevCore/WModules/EntityClass.js';
import { Conversacion } from './Conversacion.js';
class Mensajes extends EntityClass {
    /** @param {Partial<Mensajes>} [props] */
    constructor(props) {
        super(props, 'MessageManager');
        for (const prop in props) {
            this[prop] = props[prop];
        }
    }
    /**@type {Number}*/ Id_mensaje;
    /**@type {String}*/ Remitente;
    /**@type {String}*/ Destinatarios;
    /**@type {String}*/ Asunto;
    /**@type {String}*/ Body;
    /**@type {Date}*/ Created_at;
    /**@type {Date}*/ Updated_at;
    /**@type {Boolean}*/ Enviado;
    /**@type {Boolean}*/ Leido;
    /**@type {Conversacion} ManyToOne*/ Conversacion;
    /**@type {Security_Users} ManyToOne*/ Security_Users;
 }
 export { Mensajes }
