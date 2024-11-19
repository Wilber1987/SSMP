import { EntityClass } from "../../WDevCore/WModules/EntityClass.js";
import {Tbl_Comments_ModelComponent} from "./Tbl_Comments.js";

class Tbl_Comments_Tasks_ModelComponent extends Tbl_Comments_ModelComponent {
    constructor(props) {
        super(props, 'EntityHelpdesk');
        for (const prop in props) {
            this[prop] = props[prop];
        }
    }
    Id_Tarea = {type: "text", hidden: true};
}


export {Tbl_Comments_Tasks_ModelComponent};

class Tbl_Comments_Tasks extends EntityClass {
    /** @param {Partial<Tbl_Comments_Tasks>} [props] */
    constructor(props) {
        super(props, 'EntityHelpdesk');
        for (const prop in props) {
            this[prop] = props[prop];
        }
    }
    /**@type {Number}*/ Id_Comentario;
    /**@type {String}*/ Body;
    /**@type {String}*/ Estado;
    /**@type {Date}*/ Fecha;
    /**@type {Number}*/ Id_Tarea;
    /**@type {Number}*/ Id_User;
    /**@type {String}*/ NickName;
    /**@type {String}*/ Mail;
    /**@type {String}*/ Attach_Files;
    /**@type {String}*/ Mails;
 }
 export { Tbl_Comments_Tasks }