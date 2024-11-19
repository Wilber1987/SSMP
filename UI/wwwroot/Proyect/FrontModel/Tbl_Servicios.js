//@ts-check
import {EntityClass} from "../../WDevCore/WModules/EntityClass.js";
import { Cat_Dependencias, Cat_Dependencias_ModelComponent } from "./Cat_Dependencias.js";
import { Tbl_Case } from "./Tbl_CaseModule.js";

class Tbl_Servicios_ModelComponent extends EntityClass {
    constructor(props) {
        super(props, 'EntityHelpdesk');
        for (const prop in props) {
            this[prop] = props[prop];
        }
    }

    Id_Servicio = {type: 'number', primary: true};
    //Nombre_Proyecto = { type: 'text', label: "Nombre" };
    Descripcion_Servicio = {type: 'text'};
    //Visibilidad = { type: 'text' };
    Estado = {type: "Select", Dataset: ["Activo", "Inactivo"]};
    //Cat_Tipo_Servicio = { type: 'WSelect', hiddenFilter: true, ModelObject: () => new Cat_Tipo_Servicio() };
    Cat_Dependencias = {type: 'WSelect', hiddenFilter: true, ModelObject: () => new Cat_Dependencias_ModelComponent()}
    //Fecha_Inicio = { type: 'date' };
    //Fecha_Finalizacion = { type: 'date' };
    //Tbl_Case = { type: 'MasterDetail', ModelObject: () => new Tbl_Case() };
}

export {Tbl_Servicios_ModelComponent};

class Tbl_Servicios extends EntityClass {
    /** @param {Partial<Tbl_Servicios>} [props] */
    constructor(props) {
        super(props, 'EntityHelpdesk');
        for (const prop in props) {
            this[prop] = props[prop];
        }
    }
    /**@type {Number}*/ Id_Servicio;
    /**@type {String}*/ Nombre_Servicio;
    /**@type {String}*/ Descripcion_Servicio;
    /**@type {String}*/ Visibilidad;
    /**@type {String}*/ Estado;
    /**@type {Date}*/ Fecha_Inicio;
    /**@type {Date}*/ Fecha_Finalizacion;
    //**@type {Cat_Tipo_Servicio} ManyToOne*/ Cat_Tipo_Servicio;
    /**@type {Cat_Dependencias} ManyToOne*/ Cat_Dependencias;
    /**@type {Array<Tbl_Case>} OneToMany*/ Tbl_Case;
    //**@type {Array<Tbl_Servicios_Profile>} OneToMany*/ Tbl_Servicios_Profile;
 }
 export { Tbl_Servicios }