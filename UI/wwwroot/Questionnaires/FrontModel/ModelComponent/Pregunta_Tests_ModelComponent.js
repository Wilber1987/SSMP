//@ts-check
import { EntityClass } from "../../../WDevCore/WModules/EntityClass.js";
// @ts-ignore
import { ModelProperty } from "../../../WDevCore/WModules/CommonModel.js";
import { Cat_Tipo_Preguntas_ModelComponent } from './Cat_Tipo_Preguntas_ModelComponent.js';
class Pregunta_Tests_ModelComponent extends EntityClass {
    /** @param {Partial<Pregunta_Tests_ModelComponent>} [props] */
    constructor(props) {
        super(props, 'EntityQuestionnaires');
        for (const prop in props) {
            this[prop] = props[prop];
        }
    }
   /**@type {ModelProperty}*/ Id_pregunta_test = { type: 'number', primary: true };
   /**@type {ModelProperty}*/ Estado = { type: 'SELECT', Dataset: ["ACTIVO", "INACTIVO"] };
   /**@type {ModelProperty}*/ Descripcion_pregunta = { type: 'text' };
   /**@type {ModelProperty}*/ Fecha = { type: 'date' };
   ///**@type {ModelProperty}*/ Created_at = { type: 'date' };
   ///**@type {ModelProperty}*/ Updated_at = { type: 'date' };
   /**@type {ModelProperty}*/ Seccion = { type: 'text', require: false };
   /**@type {ModelProperty}*/ Descripcion_general = { type: 'text' };
   /**@type {ModelProperty}*/ Cat_Tipo_Preguntas = { type: 'WSELECT', ModelObject: () => new Cat_Tipo_Preguntas_ModelComponent() };
    ///**@type {ModelProperty}*/ Tests = { type: 'WSELECT',  ModelObject: ()=> new Tests_ModelComponent()};
    ///**@type {ModelProperty}*/ Resultados_Pregunta_Tests = { type: 'MasterDetail',  ModelObject: ()=> new Resultados_Pregunta_Tests_ModelComponent()};
}
export { Pregunta_Tests_ModelComponent };
