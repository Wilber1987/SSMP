//@ts-check
import {EntityClass} from "../../WDevCore/WModules/EntityClass.js";
import { Tbl_Calendario, Tbl_Calendario_ModelComponent } from "./Tbl_Calendario.js";
import { Tbl_Case } from "./Tbl_CaseModule.js";
import { Tbl_Evidencias } from "./Tbl_Evidencias.js";
import { Tbl_Participantes, Tbl_Participantes_ModelComponent } from "./Tbl_Participantes.js";


class Tbl_Tareas_ModelComponent extends EntityClass {
    constructor(props) {
        super(props, 'EntityHelpdesk');
        //this.Fecha_Inicio = undefined;
        this.Tbl_Case = undefined;
        for (const prop in props) {
            this[prop] = props[prop];
        }
    }

    Id_Tarea = {type: 'number', primary: true};
    Titulo = {type: 'text'};
    Id_Case = {type: 'number', hidden: true, value: undefined};
    Fecha_Inicio = {type: "date"}
    Estado = {type: "Select", Dataset: ["Activo", "Proceso", "Finalizado", "Espera", "Inactivo"]};
    Tbl_Tarea = {
        type: 'WSelect', hiddenFilter: true, label: "Tarea principal", SelfChargeDataset: "Tbl_Tareas",
        ModelObject: () => new Tbl_Tareas_ModelComponent(), require: false
    }; 
    Descripcion = {type: 'richtext', hiddenInTable: true};
    
    //Tbl_TareasHijas = { type: 'MULTISELECT', hiddenFilter: true, ModelObject: () => new Tbl_Tareas() };
    Tbl_Participantes = {type: 'MasterDetail', ModelObject: () => new Tbl_Participantes_ModelComponent()};
    //Tbl_Evidencias = { type: 'MasterDetail', require: false, ModelObject: () => new Tbl_Evidencias() };
    Tbl_Calendario = {
        type: 'CALENDAR',
        ModelObject: () => new Tbl_Calendario_ModelComponent(),
        require: true,
        hiddenInTable: true,
        CalendarFunction: () => {
        }
    };
    GetOwParticipations = async () => {
        return await this.GetData("Proyect/GetOwParticipations");
    }
}

export {Tbl_Tareas_ModelComponent};

class Tbl_Tareas extends EntityClass {
    /** @param {Partial<Tbl_Tareas>} [props] */
    constructor(props) {
        super(props, 'EntityHelpdesk');
        for (const prop in props) {
            this[prop] = props[prop];
        }
    }
    /**@type {Number}*/ Id_Tarea;
    /**@type {String}*/ Titulo;
    /**@type {String}*/ Descripcion;
    /**@type {String}*/ Estado;
    /**@type {Date}*/ Fecha_Inicio;
    /**@type {Date}*/ Fecha_Finalizacion;
    /**@type {Date}*/ Fecha_Inicio_Proceso;
    /**@type {Date}*/ Fecha_Finalizacion_Proceso;
    /**@type {Tbl_Tareas} ManyToOne*/ Tbl_Tarea;
    /**@type {Tbl_Case} ManyToOne*/ Tbl_Case;
    /**@type {Array<Tbl_Calendario>} OneToMany*/ Tbl_Calendario;
    /**@type {Array<Tbl_Evidencias>} OneToMany*/ Tbl_Evidencias;
    /**@type {Array<Tbl_Participantes>} OneToMany*/ Tbl_Participantes;
    /**@type {Array<Tbl_Tareas>} OneToMany*/ Tbl_Tareas;
 }
 export { Tbl_Tareas }