//@ts-check

import { ModelFiles } from "../../WDevCore/WModules/CommonModel.js";
import { EntityClass } from "../../WDevCore/WModules/EntityClass.js";
class NotificationRequest extends EntityClass {
    /** @param {Partial<NotificationRequest>} [props] */
    constructor(props) {
        super(props, 'Notificaciones');
        for (const prop in props) {
            this[prop] = props[prop];
        };
    }
   /**@type {Number}*/ Id;
   /**@type {String}*/ Titulo;
   /**@type {String}*/ Mensaje;
   /**@type {Array<ModelFiles>}*/ Files;
   /**@type {String}*/ NotificationType;
   /**@type {Array<Number>}*/ Usuarios;
   /**@type {Array<Number>}*/ Dependencias;
   /**@type {Array<Number>}*/ ToAdress; //correos o numeros de telefono
   /**@type {String}*/ NotificationsServicesEnum;  
   /**@type {Array<NotificactionDestinatarios>}*/  Destinatarios;
}

export { NotificationRequest }

export class NotificactionDestinatarios {
    /**@type {String} */ Correo;
    /**@type {String} */ Telefono;
    /**@type {Object} */ NotificationData;
}