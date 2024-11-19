import { ModelProperty } from "../../WDevCore/WModules/CommonModel.js";
import { EntityClass } from "../../WDevCore/WModules/EntityClass.js";
import { Tbl_Profile } from "./Tbl_Profile.js";
import {Cat_Tipo_Participaciones} from "./Cat_Tipo_Participaciones.js";

class Tbl_Profile_CasosAsignados extends EntityClass {
    /**
    * @param {Partial<Tbl_Profile_CasosAsignados>} props 
    */
    constructor(props) {
        super(props, 'EntityHelpdesk');
        for (const prop in props) {
            this[prop] = props[prop];
        }

    }
    /**@type {Number} */
    Id_Case;
    /**@type {Tbl_Profile} */
    Tbl_Profile;
    /**@type {Object} */
    Cat_Tipo_Participaciones;

}
export { Tbl_Profile_CasosAsignados }
class Tbl_Profile_CasosAsignados_ModelComponent extends EntityClass {
    /**
    * @param {Partial<Tbl_Profile_CasosAsignados_ModelComponent>} [props] 
    */
    constructor(props) {
        super(props, 'EntityHelpdesk');
        for (const prop in props) {
            this[prop] = props[prop];
        }

    }
    /**@type {ModelProperty} */
    Tbl_Profile = { type: "Model", label: "Perfil", ModelObject: () => new Tbl_Profile() };
    /**@type {ModelProperty} */
    Cat_Tipo_Participaciones = { type: 'WSelect', label: "Tipo de Participación" , ModelObject: () => new Cat_Tipo_Participaciones() }
}
export { Tbl_Profile_CasosAsignados_ModelComponent }