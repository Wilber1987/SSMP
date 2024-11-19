import {EntityClass} from "../../WDevCore/WModules/EntityClass.js";

class Cat_Tipo_Evidencia_ModelComponent extends EntityClass {
    constructor(props) {
        super(props, 'EntityHelpdesk');
        for (const prop in props) {
            this[prop] = props[prop];
        }
    }

    IdTipo = {type: 'number', primary: true};
    Descripcion = {type: 'text'};
    Estado = {type: "Select", Dataset: ["Activo", "Inactivo"]};
}

export {Cat_Tipo_Evidencia_ModelComponent};

class Cat_Tipo_Evidencia extends EntityClass {
    /** @param {Partial<Cat_Tipo_Evidencia>} [props] */
    constructor(props) {
        super(props, 'EntityHelpdesk');
        for (const prop in props) {
            this[prop] = props[prop];
        }
    }
    /**@type {Number}*/ IdTipo;
    /**@type {String}*/ Descripcion;
    /**@type {String}*/ Estado;
    /**@type {Array<Tbl_Evidencias>} OneToMany*/ Tbl_Evidencias;
 }
 export { Cat_Tipo_Evidencia }