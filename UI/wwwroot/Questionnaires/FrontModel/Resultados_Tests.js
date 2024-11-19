//@ts-check
import { EntityClass } from "../../WDevCore/WModules/EntityClass.js";
//@ts-ignore
import { Resultados_Pregunta_Tests } from "./Resultados_Pregunta_Tests.js";
import { Tests } from './Tests.js';
class Resultados_Tests extends EntityClass {
    /** @param {Partial<Resultados_Tests>} [props] */
   constructor(props) {
       super(props, 'EntityQuestionnaires');
       for (const prop in props) {
           this[prop] = props[prop];
       }
   }
   /**@type {Number}*/ Id_Perfil;
   /**@type {Date}*/ Fecha;
   /**@type {String}*/ Seccion;
   /**@type {String}*/ Valor;
   /**@type {String}*/ Categoria_valor;
   /**@type {String}*/ Tipo;
   /**@type {Tests} ManyToOne*/ Tests;
   /**@type {Array<Resultados_Pregunta_Tests>} OneToMany*/ Resultados_Pregunta_Tests;
}
export { Resultados_Tests };
