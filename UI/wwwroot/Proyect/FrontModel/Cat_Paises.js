import {EntityClass} from "../../WDevCore/WModules/EntityClass.js";

class Cat_Paises_ModelComponent extends EntityClass {
    constructor(props) {
        super(props, 'EntityHelpdesk');
        for (const prop in props) {
            this[prop] = props[prop];
        }
    }

    Id_Pais = {type: 'number', primary: true};
    Estado = {type: "Select", Dataset: ["Activo", "Inactivo"]};
    Descripcion = {type: 'text'};
}

export {Cat_Paises_ModelComponent};

class Cat_Paises extends EntityClass {
    /** @param {Partial<Cat_Paises>} [props] */
    constructor(props) {
        super(props, 'EntityHelpdesk');
        for (const prop in props) {
            this[prop] = props[prop];
        }
    }
    /**@type {Number}*/ Id_Pais;
    /**@type {String}*/ Estado;
    /**@type {String}*/ Descripcion;
    /**@type {Array<Tbl_Profile>} OneToMany*/ Tbl_Profile;
 }
 export { Cat_Paises }
 