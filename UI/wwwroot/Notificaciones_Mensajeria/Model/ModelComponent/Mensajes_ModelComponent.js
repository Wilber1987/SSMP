//@ts-check
// @ts-ignore
import { ModelProperty } from '../../../WDevCore/WModules/CommonModel.js';
import { EntityClass } from '../../../WDevCore/WModules/EntityClass.js';
class Mensajes_ModelComponent extends EntityClass {
   /** @param {Partial<Mensajes_ModelComponent>} [props] */
   constructor(props) {
       super(props, 'EntityDbo');
       for (const prop in props) {
           this[prop] = props[prop];
       }
   }
   /**@type {ModelProperty}*/ Id_mensaje = { type: 'number', primary: true };
   /**@type {ModelProperty}*/ Remitente = { type: 'text' };
   /**@type {ModelProperty}*/ Usuario_id = { type: 'number' };
   /**@type {ModelProperty}*/ Destinatarios = { type: 'text' };
   /**@type {ModelProperty}*/ Asunto = { type: 'text' };
   /**@type {ModelProperty}*/ Body = { type: 'text' };
   /**@type {ModelProperty}*/ Id_conversacion = { type: 'number' };
   /**@type {ModelProperty}*/ Created_at = { type: 'date' , label: "Fecha"};
   /**@type {ModelProperty}*/ Updated_at = { type: 'date', hiddenFilter: true };
   /**@type {ModelProperty}*/ Enviado = { type: 'checkbox' };
   /**@type {ModelProperty}*/ Leido = { type: 'checkbox' };
}
export { Mensajes_ModelComponent }
