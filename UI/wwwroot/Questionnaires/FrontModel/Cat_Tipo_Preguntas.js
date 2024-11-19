//@ts-check
import { EntityClass } from "../../WDevCore/WModules/EntityClass.js";
//@ts-ignore
import { Cat_Valor_Preguntas } from './Cat_Valor_Preguntas.js';
import { Pregunta_Tests } from './Pregunta_Tests.js';
class Cat_Tipo_Preguntas extends EntityClass {
    /** @param {Partial<Cat_Tipo_Preguntas>} [props] */
    constructor(props) {
        super(props, 'EntityQuestionnaires');
        for (const prop in props) {
            this[prop] = props[prop];
        }
    }
   /**@type {Number}*/ Id_tipo_pregunta;
   /**@type {String}*/ Tipo_pregunta;
   /**@type {String}*/ Descripcion;
   /**@type {String}*/ Estado;
   /**@type {Date}*/ Fecha_crea;
   /**@type {Date}*/ Created_at;
   /**@type {Date}*/ Updated_at;
   /**@type {Boolean}*/ Editable;
   /**@type {String}*/ Descripcion_general;
   /**@type {Array<Cat_Valor_Preguntas>} OneToMany*/ Cat_Valor_Preguntas;
   /**@type {Array<Pregunta_Tests>} OneToMany*/ Pregunta_Tests;
}
export { Cat_Tipo_Preguntas };
