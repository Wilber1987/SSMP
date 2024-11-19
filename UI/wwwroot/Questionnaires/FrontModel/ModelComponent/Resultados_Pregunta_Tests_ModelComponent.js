//@ts-check
import { EntityClass } from "../../../WDevCore/WModules/EntityClass.js";
// @ts-ignore
import { ModelProperty } from "../../../WDevCore/WModules/CommonModel.js";
import { Cat_Valor_Preguntas_ModelComponent } from './Cat_Valor_Preguntas_ModelComponent.js';
import { Pregunta_Tests_ModelComponent } from './Pregunta_Tests_ModelComponent.js';
class Resultados_Pregunta_Tests_ModelComponent extends EntityClass {
    /** @param {Partial<Resultados_Pregunta_Tests_ModelComponent>} [props] */
    constructor(props) {
        super(props, 'EntityQuestionnaires');
        for (const prop in props) {
            this[prop] = props[prop];
        }
    }
   /**@type {ModelProperty}*/ Id_Perfil = { type: 'number', primary: true };
   /**@type {ModelProperty}*/ Resultado = { type: 'number' };
   /**@type {ModelProperty}*/ Respuesta = { type: 'text' };
   /**@type {ModelProperty}*/ Estado = { type: 'SELECT', Dataset: ["ACTIVO", "INACTIVO"] };
   ///**@type {ModelProperty}*/ Created_at = { type: 'date' };
   ///**@type {ModelProperty}*/ Updated_at = { type: 'date' };
   /**@type {ModelProperty}*/ Fecha = { type: 'date', primary: true };
   /**@type {ModelProperty}*/ Cat_Valor_Preguntas = { type: 'WSELECT', ModelObject: () => new Cat_Valor_Preguntas_ModelComponent() };
   /**@type {ModelProperty}*/ Pregunta_Tests = { type: 'WSELECT', ModelObject: () => new Pregunta_Tests_ModelComponent() };
}
export { Resultados_Pregunta_Tests_ModelComponent };
