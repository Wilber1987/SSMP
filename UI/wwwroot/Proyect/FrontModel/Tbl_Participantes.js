import {EntityClass} from "../../WDevCore/WModules/EntityClass.js";
import { Cat_Tipo_Participaciones } from "./Cat_Tipo_Participaciones.js";
import { Tbl_Profile } from "./Tbl_Profile.js";

class Tbl_Participantes_ModelComponent extends EntityClass {
    constructor(props) {
        super(props, 'EntityHelpdesk');
        for (const prop in props) {
            this[prop] = props[prop];
        }
        this.Id_Dependencia = undefined;
    }

    Tbl_Profile = {type: 'WSelect', hiddenFilter: true, ModelObject: () => new Tbl_Profile()}
    Cat_Tipo_Participaciones = {type: 'WSelect', hiddenFilter: true, ModelObject: () => new Cat_Tipo_Participaciones()}
}

export {Tbl_Participantes_ModelComponent};

class Tbl_Participantes extends EntityClass {
    /** @param {Partial<Tbl_Participantes>} [props] */
    constructor(props) {
        super(props, 'EntityHelpdesk');
        for (const prop in props) {
            this[prop] = props[prop];
        }
    }
    /**@type {Tbl_Profile} ManyToOne*/ Tbl_Profile;
    /**@type {Tbl_Tareas} ManyToOne*/ Tbl_Tareas;
    /**@type {Cat_Tipo_Participaciones} ManyToOne*/ Cat_Tipo_Participaciones;
 }
 export { Tbl_Participantes }