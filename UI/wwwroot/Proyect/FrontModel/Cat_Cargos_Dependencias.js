import {EntityClass} from "../../WDevCore/WModules/EntityClass.js";

class Cat_Cargos_Dependencias_ModelComponent extends EntityClass {
    constructor(props) {
        super(props, 'EntityHelpdesk');
        for (const prop in props) {
            this[prop] = props[prop];
        }
    }

    IdCargo = {type: 'number', primary: true};
    Descripcion = {type: 'text'};
    //Tbl_Dependencias_Usuarios = { type: 'MasterDetail', ModelObject: () => new Tbl_Dependencias_Usuarios() };
}

export {Cat_Cargos_Dependencias_ModelComponent};


 class Cat_Cargos_Dependencias extends EntityClass {
    /** @param {Partial<Cat_Cargos_Dependencias>} [props] */
    constructor(props) {
        super(props, 'EntityHelpdesk');
        for (const prop in props) {
            this[prop] = props[prop];
        }
    }
    /**@type {Number}*/ Id_Cargo;
    /**@type {String}*/ Descripcion;
    /**@type {Array<Tbl_Dependencias_Usuarios>} OneToMany*/ Tbl_Dependencias_Usuarios;
 }
 export { Cat_Cargos_Dependencias }
 