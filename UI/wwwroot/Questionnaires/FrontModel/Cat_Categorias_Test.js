//@ts-check
import { EntityClass } from "../../WDevCore/WModules/EntityClass.js";
//@ts-ignore
import { Tests } from './Tests.js';
class Cat_Categorias_Test extends EntityClass {
    /** @param {Partial<Cat_Categorias_Test>} [props] */
    constructor(props) {
        super(props, 'EntityQuestionnaires');
        for (const prop in props) {
            this[prop] = props[prop];
        }
    }
   /**@type {Number}*/ Id_categoria;
   /**@type {String}*/ Descripcion;
   /**@type {String}*/ Imagen;
   /**@type {String}*/ Estado;
   /**@type {Date}*/ Created_at;
   /**@type {Date}*/ Updated_at;
   /**@type {Array<Tests>} OneToMany*/ Tests;
}
export { Cat_Categorias_Test };
