import {EntityClass} from "../../WDevCore/WModules/EntityClass.js";

class Tbl_Evidencias extends EntityClass {
    constructor(props) {
        super(props, 'EntityHelpdesk');
        for (const prop in props) {
            this[prop] = props[prop];
        }
    }

    IdEvidencia = {type: 'number', primary: true};
    Cat_Tipo_Evidencia = {type: 'WSelect', hiddenFilter: true, ModelObject: () => new Cat_Tipo_Evidencia_ModelComponent()};
    Data = {type: 'file'};
    Id_Tarea = {type: 'number', hidden: true};
}

export {Tbl_Evidencias};