//@ts-check
import { EntityClass } from "../../WDevCore/WModules/EntityClass.js";
import { Category } from "./Category.js";

class Article extends EntityClass {
    /** @param {Partial<Article>} [props] */
    constructor(props) {
        super(props, 'ArticleManager');
        for (const prop in props) {
            this[prop] = props[prop];
        }
    }
    /** @type {Number} */ Article_Id;
    /** @type {String} */ Title;
    /** @type {String} */ Author;
    /** @type {Number} */ Id_User;
    /** @type {String} */ Body;
    /** @type {Date} */ Publish_Date;
    /** @type {Date} */ Update_Date;
    /** @type {Boolean} */ Status;
    /** @type {Number} */ Category_Id;
    /** @type {Category} ManyToOne */ Category;
}

export { Article };
