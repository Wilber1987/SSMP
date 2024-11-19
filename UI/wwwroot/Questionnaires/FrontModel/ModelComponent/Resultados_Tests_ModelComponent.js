//@ts-check
import { EntityClass } from "../../../WDevCore/WModules/EntityClass.js";
// @ts-ignore
import { ModelProperty } from "../../../WDevCore/WModules/CommonModel.js";
import { Tests_ModelComponent } from './Tests_ModelComponent.js';
class Resultados_Tests_ModelComponent extends EntityClass {
    /** @param {Partial<Resultados_Tests_ModelComponent>} [props] */
    constructor(props) {
        super(props, 'EntityQuestionnaires');
        for (const prop in props) {
            this[prop] = props[prop];
        }
    }
   /**@type {ModelProperty}*/ Id_Perfil = { type: 'number', primary: true };
   /**@type {ModelProperty}*/ Fecha = { type: 'date', primary: true };
   /**@type {ModelProperty}*/ Seccion = { type: 'text' };
   ///**@type {ModelProperty}*/ Created_at = { type: 'date' };
   ///**@type {ModelProperty}*/ Updated_at = { type: 'date' };
   /**@type {ModelProperty}*/ Valor = { type: 'text' };
   /**@type {ModelProperty}*/ Categoria_valor = { type: 'text' };
   /**@type {ModelProperty}*/ Tipo = { type: 'text' };
   /**@type {ModelProperty}*/ Tests = { type: 'WSELECT', ModelObject: () => new Tests_ModelComponent() };
}
export { Resultados_Tests_ModelComponent };

