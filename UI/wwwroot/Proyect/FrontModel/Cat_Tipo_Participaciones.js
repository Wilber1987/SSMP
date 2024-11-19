import {EntityClass} from "../../WDevCore/WModules/EntityClass.js";
import { Tbl_Participantes_ModelComponent } from "./Tbl_Participantes.js";

class Cat_Tipo_Participaciones extends EntityClass {
    constructor(props) {
        super(props, 'EntityHelpdesk');
        for (const prop in props) {
            this[prop] = props[prop];
        }
    }

    Id_Tipo_Participacion = {type: 'number', primary: true};
    Descripcion = {type: 'text'};
    Tbl_Participantes = {type: 'MasterDetail', ModelObject: () => new Tbl_Participantes_ModelComponent()};
}

export {Cat_Tipo_Participaciones};