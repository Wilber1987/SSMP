//@ts-check
// @ts-ignore
import { ModelProperty } from '../../../WDevCore/WModules/CommonModel.js';
import { EntityClass } from '../../../WDevCore/WModules/EntityClass.js';
class Mensaje_adjuntos_ModelComponent extends EntityClass {
   /** @param {Partial<Mensaje_adjuntos_ModelComponent>} [props] */
   constructor(props) {
       super(props, 'EntityDbo');
       for (const prop in props) {
           this[prop] = props[prop];
       }
   }
   /**@type {ModelProperty}*/ Id = { type: 'number', primary: true };
   /**@type {ModelProperty}*/ Mensaje_id = { type: 'number' };
   /**@type {ModelProperty}*/ Archivo = { type: 'text' };
   /**@type {ModelProperty}*/ Created_at = { type: 'date' , label: "Fecha"};
   /**@type {ModelProperty}*/ Updated_at = { type: 'date', hiddenFilter: true };
}
export { Mensaje_adjuntos_ModelComponent }
