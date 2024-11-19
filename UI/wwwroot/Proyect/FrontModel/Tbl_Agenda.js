import {EntityClass} from "../../WDevCore/WModules/EntityClass.js";

class Tbl_Agenda_ModelComponent extends EntityClass {
    constructor(props) {
        super(props, 'EntityHelpdesk');
        for (const prop in props) {
            this[prop] = props[prop];
        }
    }

    Id_Dependencia = {type: 'number', hidden: true};
    IdAgenda = {type: 'number', primary: true};
    Dia = {type: 'Select', Dataset: ["Domingo", "Lunes", "Martes", "Miércoles", "Jueves", "Viernes", "Sábado"]};
    Hora_Inicial = {type: 'HORA'};
    Hora_Final = {type: 'HORA'};
    Fecha_Caducidad = {type: 'date'};
}

export {Tbl_Agenda_ModelComponent};

class Tbl_Agenda extends EntityClass {
    /** @param {Partial<Tbl_Agenda>} [props] */
    constructor(props) {
        super(props, 'EntityHelpdesk');
        for (const prop in props) {
            this[prop] = props[prop];
        }
    }
    /**@type {Number}*/ IdAgenda;
    /**@type {String}*/ Dia;
    /**@type {String}*/ Hora_Inicial;
    /**@type {String}*/ Hora_Final;
    /**@type {Date}*/ Fecha_Caducidad;
    /**@type {Tbl_Profile} ManyToOne*/ Tbl_Profile;
    /**@type {Cat_Dependencias} ManyToOne*/ Cat_Dependencias;
 }
 export { Tbl_Agenda }