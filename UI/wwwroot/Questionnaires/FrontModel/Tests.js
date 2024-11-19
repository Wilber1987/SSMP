//@ts-check
import { EntityClass } from "../../WDevCore/WModules/EntityClass.js";
//@ts-ignore
import { Cat_Categorias_Test } from './Cat_Categorias_Test.js';
import { Pregunta_Tests } from './Pregunta_Tests.js';
import { Resultados_Tests } from './Resultados_Tests.js';
class Tests extends EntityClass {

    /** @param {Partial<Tests>} [props] */
    constructor(props) {
        super(props, 'EntityQuestionnaires');
        for (const prop in props) {
            this[prop] = props[prop];
        }
    }
   /**@type {Number}*/ Id_test;
   /**@type {String}*/ Titulo;
   /**@type {String}*/ Descripcion;
   /**@type {Number}*/ Grado;
   /**@type {String}*/ Tipo_test;
   /**@type {String}*/ Estado;
   /**@type {Date}*/ Fecha_publicacion;
   /**@type {Date}*/ Created_at;
   /**@type {Date}*/ Updated_at;
   /**@type {String}*/ Referencias;
   /**@type {Number}*/ Tiempo;
   /**@type {Number}*/ Caducidad;
   /**@type {String}*/ Color;
   /**@type {String}*/ Image;
   /**@type {Cat_Categorias_Test} ManyToOne*/ Cat_Categorias_Test;
   /**@type {Array<Pregunta_Tests>} OneToMany*/ Pregunta_Tests;
   /**@type {Array<Resultados_Tests>} OneToMany*/ Resultados_Tests;
    SaveResultado() {
       return this.SaveData("QuestionnairesTransactions/SaveResultado", this);
    }
}
export { Tests };
