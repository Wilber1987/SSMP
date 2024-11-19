//@ts-check
import { EntityClass } from "../../WDevCore/WModules/EntityClass.js";
//@ts-ignore
import { Cat_Tipo_Preguntas } from './Cat_Tipo_Preguntas.js';
import { Resultados_Pregunta_Tests } from './Resultados_Pregunta_Tests.js';
class Cat_Valor_Preguntas extends EntityClass {
    /** @param {Partial<Cat_Valor_Preguntas>} [props] */
    constructor(props) {
        super(props, 'EntityQuestionnaires');
        for (const prop in props) {
            this[prop] = props[prop];
        };
        this.Id_tipo_pregunta = undefined;
    }
   /**@type {Number}*/ Id_valor_pregunta;
   /**@type {String}*/ Descripcion;
   /**@type {String}*/ Estado;
   /**@type {Number}*/ Valor;
   /**@type {Date}*/ Created_at;
   /**@type {Date}*/ Updated_at;
   /**@type {Cat_Tipo_Preguntas} ManyToOne*/ Cat_Tipo_Preguntas;
   /**@type {Array<Resultados_Pregunta_Tests>} OneToMany*/ Resultados_Pregunta_Tests;
}
export { Cat_Valor_Preguntas };
