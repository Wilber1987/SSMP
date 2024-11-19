//@ts-check
// @ts-ignore
import { ModelProperty } from "../WDevCore/WModules/CommonModel.js";
import { EntityClass } from "../WDevCore/WModules/EntityClass.js";
class ProfileRequest_ModelComponent extends EntityClass {
    /** @param {Partial<ProfileRequest_ModelComponent>} [props] */
    constructor(props) {
        super(props, 'ProfileManager');
        for (const prop in props) {
            this[prop] = props[prop];
        }
    }
    /**@type {ModelProperty}*/ User_id = { type: 'number', hidden: true };
    //datos perfil
    /**@type {ModelProperty}*/ Foto = { type: 'img' };
    /**@type {ModelProperty}*/ Nombre = { type: 'text' };

    /**@type {ModelProperty}*/ Correo_Anterior = { type: 'text' };
    /**@type {ModelProperty}*/ Telefono_Anterior = { type: 'text' };
    /**@type {ModelProperty}*/ Celular_Anterior = { type: 'text' };

    /**@type {ModelProperty}*/ Id = { type: 'text', primary: true, hiddenFilter: true };    
    /**@type {ModelProperty}*/ Estado = { type: 'Select', Dataset: ["PENDIENTE", "APROBADO", "RECHAZADO"] }
    /**@type {ModelProperty}*/ Correo = { type: 'text' };
    /**@type {ModelProperty}*/ Telefono = { type: 'text' };
    /**@type {ModelProperty}*/ Celular = { type: 'text' };
    /**@type {ModelProperty}*/ Direccion = { type: 'text' };    
    /**@type {ModelProperty}*/ Observacion = { type: 'textarea' }; 
}

export { ProfileRequest_ModelComponent }

class ProfileRequest extends EntityClass {

    /** @param {Partial<ProfileRequest>} [props] */
    constructor(props) {
        super(props, 'ProfileManager');
        for (const prop in props) {
            this[prop] = props[prop];
        }
    }
    /** @type {Number}*/ User_id
    /** @type {String}*/ Id
    /** @type {String}*/ Correo
    /** @type {String}*/ Telefono
    /** @type {String}*/ Celular
    /** @type {String}*/ Estado
    /** @type {String}*/ Direccion
    /** @type {String}*/ Observacion
    /** @type {String}*/ Foto
}
export { ProfileRequest }

const ProfileRequestsStatus = {
    PENDIENTE: "PENDIENTE",
    APROBADO: "APROBADO",
    RECHAZADO: "RECHAZADO"
}

export { ProfileRequestsStatus }