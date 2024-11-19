//@ts-check
import { EntityClass } from "../../../WDevCore/WModules/EntityClass.js";
// @ts-ignore
import { ModelProperty } from "../../../WDevCore/WModules/CommonModel.js";
import { Cat_Valor_Preguntas_ModelComponent } from './Cat_Valor_Preguntas_ModelComponent.js';
class Cat_Tipo_Preguntas_ModelComponent extends EntityClass {
    /** @param {Partial<Cat_Tipo_Preguntas_ModelComponent>} [props] */
    constructor(props) {
        super(props, 'EntityQuestionnaires');
        for (const prop in props) {
            this[prop] = props[prop];
        }
    }
   /**@type {ModelProperty}*/ Id_tipo_pregunta = { type: 'number', primary: true };
   /**@type {ModelProperty}*/ Tipo_pregunta = { type: 'text' };
   /**@type {ModelProperty}*/ Descripcion = { type: 'text' };
   /**@type {ModelProperty}*/ Estado = { type: 'SELECT', Dataset: ["ACTIVO", "INACTIVO"] };
   /**@type {ModelProperty}*/ Fecha_crea = { type: 'date' };
   ///**@type {ModelProperty}*/ Created_at = { type: 'date' };
   ///**@type {ModelProperty}*/ Updated_at = { type: 'date' };
   /**@type {ModelProperty}*/ Editable = { type: 'checkbox', require: false };
   /**@type {ModelProperty}*/ Descripcion_general = { type: 'text' };
   /**@type {ModelProperty}*/ Cat_Valor_Preguntas = { type: 'MasterDetail', ModelObject: () => new Cat_Valor_Preguntas_ModelComponent() };
    ///**@type {ModelProperty}*/ Pregunta_Tests = { type: 'MasterDetail',  ModelObject: ()=> new Pregunta_Tests_ModelComponent()};
}
export { Cat_Tipo_Preguntas_ModelComponent };
