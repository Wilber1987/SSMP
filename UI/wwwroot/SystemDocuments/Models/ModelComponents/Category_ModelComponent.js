//@ts-check
import { EntityClass } from "../../../WDevCore/WModules/EntityClass.js";
// @ts-ignore
import { ModelProperty } from "../../../WDevCore/WModules/CommonModel.js";

class Category_ModelComponent extends EntityClass {
    /** @param {Partial<Category_ModelComponent>} [props] */
    constructor(props) {
        super(props, 'ArticleManager');
        for (const prop in props) {
            this[prop] = props[prop];
        }
    }
    /**@type {ModelProperty}*/ Category_Id = { type: 'number', primary: true };
    /**@type {ModelProperty}*/ Descripcion = { type: 'text' };
}

export { Category_ModelComponent };
