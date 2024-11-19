import {EntityClass} from "../../WDevCore/WModules/EntityClass.js";

class Tbl_Calendario_ModelComponent extends EntityClass {
    constructor(props) {
        super(props, 'EntityHelpdesk');
        for (const prop in props) {
            this[prop] = props[prop];
        }
    }

    Id_Dependencia = {type: 'number', hidden: true};
    IdCalendario = {type: 'number', primary: true};
    Estado = {type: "Select", Dataset: ["Activo", "Inactivo"]};
    Fecha_Inicio = {type: 'date'};
    Fecha_Final = {type: 'date'};
}

export {Tbl_Calendario_ModelComponent};

class Tbl_Calendario extends EntityClass {
    /** @param {Partial<Tbl_Calendario>} [props] */
    constructor(props) {
        super(props, 'EntityHelpdesk');
        for (const prop in props) {
            this[prop] = props[prop];
        }
    }
    /**@type {Number}*/ IdCalendario;
    /**@type {String}*/ Estado;
    /**@type {Date}*/ Fecha_Inicio;
    /**@type {Date}*/ Fecha_Final;
    /**@type {Tbl_Tareas} ManyToOne*/ Tbl_Tareas;
    /**@type {Cat_Dependencias} ManyToOne*/ Cat_Dependencias;
 }
 export { Tbl_Calendario }