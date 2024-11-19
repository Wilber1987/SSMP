//@ts-check
import { EntityClass } from '../../WDevCore/WModules/EntityClass.js';
class Mensaje_adjuntos extends EntityClass {
   /** @param {Partial<Mensaje_adjuntos>} [props] */
   constructor(props) {
       super(props, 'MessageManager');
       for (const prop in props) {
           this[prop] = props[prop];
       }
   }
   /**@type {Number}*/ Id;
   /**@type {Number}*/ Mensaje_id;
   /**@type {String}*/ Archivo;
   /**@type {Date}*/ Created_at;
   /**@type {Date}*/ Updated_at;
}
export { Mensaje_adjuntos }
