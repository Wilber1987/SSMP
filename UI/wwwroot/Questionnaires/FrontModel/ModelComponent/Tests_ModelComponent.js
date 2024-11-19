//@ts-check
import { EntityClass } from "../../../WDevCore/WModules/EntityClass.js";
// @ts-ignore
import { ModelProperty } from "../../../WDevCore/WModules/CommonModel.js";
import { Cat_Categorias_Test_ModelComponent } from './Cat_Categorias_Test_ModelComponent.js'
import { Pregunta_Tests_ModelComponent } from './Pregunta_Tests_ModelComponent.js'
class Tests_ModelComponent extends EntityClass {
    /** @param {Partial<Tests_ModelComponent>} [props] */
    constructor(props) {
        super(props, 'EntityQuestionnaires');
        for (const prop in props) {
            this[prop] = props[prop];
        }
    }
   /**@type {ModelProperty}*/ Id_test = { type: 'number', primary: true };
   /**@type {ModelProperty}*/ Titulo = { type: 'text' };
   ///**@type {ModelProperty}*/ Grado = { type: 'number' };
   ///**@type {ModelProperty}*/ Tipo_test = { type: 'text' };
   /**@type {ModelProperty}*/ Estado = { type: 'SELECT', Dataset: ["ACTIVO", "INACTIVO"] };
   /**@type {ModelProperty}*/ Descripcion = { type: 'TEXTAREA' };
   /**@type {ModelProperty}*/ Fecha_publicacion = { type: 'date' };
   ///**@type {ModelProperty}*/ Created_at = { type: 'date' };
   ///**@type {ModelProperty}*/ Updated_at = { type: 'date' };
   ///**@type {ModelProperty}*/ Referencias = { type: 'text' };
   /**@type {ModelProperty}*/ Tiempo = { type: 'number' };
   /**@type {ModelProperty}*/ Caducidad = { type: 'number' };
   /**@type {ModelProperty}*/ Color = { type: 'COLOR' };
   /**@type {ModelProperty}*/ Image = { type: 'img' };
   /**@type {ModelProperty}*/ Cat_Categorias_Test = { type: 'WSELECT', ModelObject: () => new Cat_Categorias_Test_ModelComponent() };
   /**@type {ModelProperty}*/ Pregunta_Tests = { type: 'MasterDetail', ModelObject: () => new Pregunta_Tests_ModelComponent() };
    ///**@type {ModelProperty}*/ Resultados_Tests = { type: 'MasterDetail',  ModelObject: ()=> new Resultados_Tests_ModelComponent()};
}
export { Tests_ModelComponent }
