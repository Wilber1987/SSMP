//@ts-check
import { EntityClass } from "../../WDevCore/WModules/EntityClass.js";
//@ts-ignore
import { Cat_Valor_Preguntas } from './Cat_Valor_Preguntas.js';
import { Pregunta_Tests } from './Pregunta_Tests.js';
class Resultados_Pregunta_Tests extends EntityClass {
    /** @param {Partial<Resultados_Pregunta_Tests>} [props] */
    constructor(props) {
        super(props, 'EntityQuestionnaires');
        for (const prop in props) {
            this[prop] = props[prop];
        }
    }
   /**@type {Number}*/ Id_Perfil;
   /**@type {Number}*/ Resultado;
   /**@type {String}*/ Respuesta;
   /**@type {String}*/ Estado;
   /**@type {Date}*/ Fecha;
   /**@type {Cat_Valor_Preguntas} ManyToOne*/ Cat_Valor_Preguntas;
   /**@type {Pregunta_Tests} ManyToOne*/ Pregunta_Tests;
}
export { Resultados_Pregunta_Tests };
