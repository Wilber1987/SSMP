import {EntityClass} from "../../WDevCore/WModules/EntityClass.js";

class Cat_Tipo_Servicio_ModelComponent extends EntityClass {
    constructor(props) {
        super(props, 'EntityHelpdesk');
        for (const prop in props) {
            this[prop] = props[prop];
        }
    }

    Id_Tipo_Servicio = {type: 'number', primary: true};
    Descripcion = {type: 'text'};
    Estado = {type: "Select", Dataset: ["Activo", "Inactivo"]};
    Icon = {type: 'img'};
    //Tbl_Servicios = { type: 'MasterDetail', ModelObject: () => new Tbl_Servicios() };
}

export {Cat_Tipo_Servicio_ModelComponent};

class Cat_Tipo_Servicio extends EntityClass {
    /** @param {Partial<Cat_Tipo_Servicio>} [props] */
    constructor(props) {
        super(props, 'EntityHelpdesk');
        for (const prop in props) {
            this[prop] = props[prop];
        }
    }
    /**@type {Number}*/ Id_Tipo_Servicio;
    /**@type {String}*/ Descripcion;
    /**@type {String}*/ Estado;
    /**@type {String}*/ Icon;
    /**@type {Array<Tbl_Servicios>} OneToMany*/ Tbl_Servicios;
 }
 export { Cat_Tipo_Servicio }