//@ts-check
import { EntityClass } from "../../WDevCore/WModules/EntityClass.js";
//@ts-ignore
import { Cat_Tipo_Preguntas } from './Cat_Tipo_Preguntas.js';
import { Resultados_Pregunta_Tests } from './Resultados_Pregunta_Tests.js';
import { Tests } from './Tests.js';
class Pregunta_Tests extends EntityClass {
    /** @param {Partial<Pregunta_Tests>} [props] */
   constructor(props) {
       super(props, 'EntityQuestionnaires');
       for (const prop in props) {
           this[prop] = props[prop];
       };
       this.Id_tipo_pregunta = undefined;
   }
   /**@type {Number}*/ Id_pregunta_test;
   /**@type {String}*/ Estado;
   /**@type {String}*/ Descripcion_pregunta;
   /**@type {Date}*/ Fecha;
   /**@type {Date}*/ Created_at;
   /**@type {Date}*/ Updated_at;
   /**@type {String}*/ Seccion;
   /**@type {String}*/ Descripcion_general;
   /**@type {Cat_Tipo_Preguntas} ManyToOne*/ Cat_Tipo_Preguntas;
   /**@type {Tests} ManyToOne*/ Tests;
   /**@type {Array<Resultados_Pregunta_Tests>} OneToMany*/ Resultados_Pregunta_Tests;
}
export { Pregunta_Tests };
