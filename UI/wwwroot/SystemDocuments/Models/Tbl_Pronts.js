//@ts-check
import { EntityClass } from "../../WDevCore/WModules/EntityClass.js";

class Tbl_Pronts extends EntityClass {
    /** @param {Partial<Tbl_Pronts>} [props] */
    constructor(props) {
        super(props, 'ArticleManager');
        for (const prop in props) {
            this[prop] = props[prop];
        }
    }
    /**@type {Number}*/ Id_Pront ;
    /**@type {String}*/ Pront_Line ;
    /**@type {String}*/ Type ;
}

export { Tbl_Pronts };
