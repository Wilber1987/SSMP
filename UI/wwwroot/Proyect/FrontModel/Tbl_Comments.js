import {EntityClass} from "../../WDevCore/WModules/EntityClass.js";

class Tbl_Comments_ModelComponent extends EntityClass {
    constructor(props) {
        super(props, 'EntityHelpdesk');
        for (const prop in props) {
            this[prop] = props[prop];
        }
        ;
        this.Id_Tarea = undefined;
    }

    Id_Comentario = {type: "number", primary: true};
    Estado = {type: "text", hidden: true};
    NickName = {type: "text", hidden: true};
    Body = {type: "textarea", label: "Mensaje"};
    Id_Case = {type: "text", hidden: true};
}

export {Tbl_Comments_ModelComponent};

class Tbl_Comments extends EntityClass {
    /** @param {Partial<Tbl_Comments>} [props] */
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
    /**@type {Number}*/ Id_User;
    /**@type {String}*/ NickName;
    /**@type {String}*/ Mail;
    /**@type {String}*/ Attach_Files;
    /**@type {String}*/ Mails;
    /**@type {Tbl_Case} ManyToOne*/ Tbl_Case;
 }
 export { Tbl_Comments }
 