//@ts-check
import { EntityClass } from "../../../WDevCore/WModules/EntityClass.js";
// @ts-ignore
import { ModelProperty } from "../../../WDevCore/WModules/CommonModel.js";
class Cat_Valor_Preguntas_ModelComponent extends EntityClass {
    /** @param {Partial<Cat_Valor_Preguntas_ModelComponent>} [props] */
    constructor(props) {
        super(props, 'EntityQuestionnaires');
        for (const prop in props) {
            this[prop] = props[prop];
        }
    }
   /**@type {ModelProperty}*/ Id_valor_pregunta = { type: 'number', primary: true };
   /**@type {ModelProperty}*/ Descripcion = { type: 'text' };
   /**@type {ModelProperty}*/ Estado = { type: 'SELECT', Dataset: ["ACTIVO", "INACTIVO"] };
   /**@type {ModelProperty}*/ Valor = { type: 'number' };
    ///**@type {ModelProperty}*/ Created_at = { type: 'date' };
    ///**@type {ModelProperty}*/ Updated_at = { type: 'date' };
    ///**@type {ModelProperty}*/ Cat_Tipo_Preguntas = { type: 'WSELECT',  ModelObject: ()=> new Cat_Tipo_Preguntas_ModelComponent()};
    ///**@type {ModelProperty}*/ Resultados_Pregunta_Tests = { type: 'MasterDetail',  ModelObject: ()=> new Resultados_Pregunta_Tests_ModelComponent()};
}
export { Cat_Valor_Preguntas_ModelComponent };
