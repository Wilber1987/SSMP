import { Tbl_Case_ModelComponent } from "../FrontModel/Tbl_CaseModule.js";
import { EntityClass } from "../../WDevCore/WModules/EntityClass.js";
import {Tbl_Tareas_ModelComponent} from "../FrontModel/Tbl_Tareas.js";
import {Cat_Dependencias_ModelComponent} from "../FrontModel/Cat_Dependencias.js";
import {WAjaxTools} from "../../WDevCore/WModules/WAjaxTools.js";

class Dashboard extends EntityClass {
    constructor(props) {
        super(props, 'Dashboard');
        for (const prop in props) {
            this[prop] = props[prop];
        }
    }
    /**@type {Array<Cat_Dependencias_ModelComponent>} */ dependencies;
    /**@type {Array<Tbl_Case_ModelComponent>} */ caseTickets;
    /**@type {Array<Tbl_Tareas_ModelComponent>} */ task;
    /**@type {Array<Tbl_Comments>} */ comments;

    GetDasboard = async (object) => {
        return await WAjaxTools.PostRequest("/api/Proyect/getDashboardgET", object);
    }
}
export { Dashboard }