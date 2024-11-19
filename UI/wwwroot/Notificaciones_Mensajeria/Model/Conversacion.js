//@ts-check
// @ts-ignore
import { EntityClass } from '../../WDevCore/WModules/EntityClass.js';
import { Conversacion_usuarios } from './Conversacion_usuarios.js';
import { Mensajes } from './Mensajes.js';
class Conversacion extends EntityClass {
    /** @param {Partial<Conversacion>} [props] */
    constructor(props) {
        super(props, 'MessageManager');
        for (const prop in props) {
            this[prop] = props[prop];
        }
    }
    /**@type {Number}*/ Id_conversacion;
    /**@type {String}*/ Descripcion;
    /**@type {Number}*/ MensajesPendientes;
    /**@type {Date}*/ Fecha_Ultimo_Mensaje;
    /**@type {Array<Conversacion_usuarios>} OneToMany*/ Conversacion_usuarios;
    /**@type {Array<Mensajes>} OneToMany*/ Mensajes;
    /**@type {String}*/ Nombre_Completo;
 }
 export { Conversacion }
