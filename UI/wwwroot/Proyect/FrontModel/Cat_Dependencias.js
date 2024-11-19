//@ts-check
import {EntityClass} from "../../WDevCore/WModules/EntityClass.js";
import { Tbl_Agenda, Tbl_Agenda_ModelComponent } from "./Tbl_Agenda.js";
import { Tbl_Calendario } from "./Tbl_Calendario.js";
import { Tbl_Case, Tbl_Dependencias_Usuarios } from "./Tbl_CaseModule.js";
import { Tbl_Servicios } from "./Tbl_Servicios.js";

class Cat_Dependencias_ModelComponent extends EntityClass {
    constructor(props) {
        super(props, 'EntityHelpdesk');
        for (const prop in props) {
            this[prop] = props[prop];
        }
        this.NCasos = undefined;
        this.NCasosFinalizados = undefined;
    }

    Id_Dependencia = {type: 'number', primary: true, hiddenFilter: true};
    Descripcion = {type: 'text'};
    Username = {type: 'email'};
    Password = {type: 'text', hiddenInTable: true, require: false, hiddenFilter: true};
    Host = {type: 'text'};
    HostService = {type: 'select', Dataset: ["OUTLOOK", 'GMAIL'], hiddenInTable: true, require: false};
    AutenticationType = {
        type: 'select',
        Dataset: ["AUTH2", "BASIC"],
        hiddenInTable: true,
        require: false,
        hiddenFilter: true
    };
    TENAT = {type: 'text', hiddenInTable: true, require: false, hiddenFilter: true};
    CLIENT = {type: 'text', hiddenInTable: true, require: false, hiddenFilter: true};
    OBJECTID = {type: 'text', hiddenInTable: true, require: false, hiddenFilter: true};
    CLIENT_SECRET = {type: 'text', hiddenInTable: true, require: false, hiddenFilter: true};
    SMTPHOST = {type: 'text', hiddenInTable: true, require: false, hiddenFilter: true};

    //Cat_Dependencia = { type: 'WSelect', hiddenFilter: true, ModelObject: () => new Cat_Dependencias(), require: false };
    Cat_Dependencias_Hijas = {
        type: 'Multiselect',
        hiddenFilter: true,
        ModelObject: () => new Cat_Dependencias_ModelComponent(),
        require: false
    };
    Tbl_Agenda = {type: 'MasterDetail', ModelObject: () => new Tbl_Agenda_ModelComponent()};
    Tbl_Dependencias_Usuarios = {
        type: 'MasterDetail',
        ModelObject: () => new Tbl_Dependencias_Usuarios(),
        require: false
    };
    GetOwDependencies = async () => {
        return await this.GetData("Proyect/GetOwDependencies");
    }
}

export {Cat_Dependencias_ModelComponent};

class Cat_Dependencias extends EntityClass {
    /** @param {Partial<Cat_Dependencias>} [props] */
    constructor(props) {
        super(props, 'EntityHelpdesk');
        for (const prop in props) {
            this[prop] = props[prop];
        }
    }
    /**@type {Number}*/ Id_Dependencia;
    /**@type {String}*/ Descripcion;
    /**@type {String}*/ Username;
    /**@type {String}*/ Password;
    /**@type {String}*/ Host;
    /**@type {String}*/ AutenticationType;
    /**@type {String}*/ TENAT;
    /**@type {String}*/ CLIENT;
    /**@type {String}*/ OBJECTID;
    /**@type {String}*/ CLIENT_SECRET;
    /**@type {String}*/ HostService;
    /**@type {String}*/ SMTPHOST;
    /**@type {Boolean}*/ Default;
    /**@type {Array<Cat_Dependencias>} OneToMany*/ Cat_Dependencias;
    /**@type {Array<Tbl_Agenda>} OneToMany*/ Tbl_Agenda;
    /**@type {Array<Tbl_Calendario>} OneToMany*/ Tbl_Calendario;
    /**@type {Array<Tbl_Case>} OneToMany*/ Tbl_Case;
    /**@type {Array<Tbl_Dependencias_Usuarios>} OneToMany*/ Tbl_Dependencias_Usuarios;
    /**@type {Array<Tbl_Servicios>} OneToMany*/ Tbl_Servicios;
 }
 export { Cat_Dependencias }

