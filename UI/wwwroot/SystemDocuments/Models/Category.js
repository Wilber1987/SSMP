//@ts-check
import { EntityClass } from "../../WDevCore/WModules/EntityClass.js";

class Category extends EntityClass {
    /** @param {Partial<Category>} [props] */
    constructor(props) {
        super(props, 'ArticleManager');
        for (const prop in props) {
            this[prop] = props[prop];
        }
    }
    /** @type {Number} */ Category_Id;
    /** @type {String} */ Descripcion;
}

export { Category };
