//@ts-check
import { EntityClass } from "../../../WDevCore/WModules/EntityClass.js";
// @ts-ignore
import { ModelProperty } from "../../../WDevCore/WModules/CommonModel.js";
import { Category_ModelComponent } from './Category_ModelComponent.js';

class Article_ModelComponent extends EntityClass {
    /** @param {Partial<Article_ModelComponent>} [props] */
    constructor(props) {
        super(props, 'ArticleManager');
        for (const prop in props) {
            this[prop] = props[prop];
        }
    }
    /**@type {ModelProperty}*/ Category = { type: 'WSELECT', ModelObject: () => new Category_ModelComponent() };
    /**@type {ModelProperty}*/ Article_Id = { type: 'number', primary: true };
    /**@type {ModelProperty}*/ Title = { type: 'text' };
    /**@type {ModelProperty}*/ Author = { type: 'text', hidden: true };
    /**@type {ModelProperty}*/ Id_User = { type: 'number', hidden: true };
    /**@type {ModelProperty}*/ Publish_Date = { type: 'date', hidden: true };
    //**@type {ModelProperty}*/ Update_Date = { type: 'date' };
    //**@type {ModelProperty}*/ Status = { type: 'SELECT', Dataset: ["true", "false"] };
    /**@type {ModelProperty}*/ Category_Id = { type: 'number', ForeignKeyColumn: "Category_Id", hidden: true };
    /**@type {ModelProperty}*/ Body = { type: 'richtext' };
}

export { Article_ModelComponent };
