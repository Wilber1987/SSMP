

import { Tbl_Case_ModelComponent } from '../Proyect/FrontModel/Tbl_CaseModule.js';
import { WFilterOptions } from '../WDevCore/WComponents/WFilterControls.js';
import { WTableComponent } from "../WDevCore/WComponents/WTableComponent.js";
import { WRender } from '../WDevCore/WModules/WComponentsTools.js';
/**
 * 
 * @param {Tbl_Case_ModelComponent} caseToVinculate
 * @param {String} actionName 
 * @param {Function} action 
 * @returns 
 */
const CaseSearcherToVinculate = (caseToVinculate, actionName = null, action = null) => {
    const model = new Tbl_Case_ModelComponent();
    model.Fecha_Final.hiddenFilter = true;
    model.Estado.hiddenFilter = true;
    model.Cat_Dependencias.hiddenFilter = true;
    model.Vinculado = { type: "color"}


    model.Get = async () => {
        const response = await new Tbl_Case_ModelComponent(caseToVinculate).GetData("Proyect/GetCasosToVinculate");
        return response.map(c => { 
            c.Vinculado = c.Id_Vinculate != null ? "#28a745" : "rgba(0, 0, 0, 0.2)";
            return c;
        })
    }
    const TableComponent = new WTableComponent({
        ModelObject: model, Dataset: [],
        maxElementByPage: 5, Options: {
            UserActions: action != null ? [{
                name: actionName ?? "Selecionar",
                action: async (caso) => {
                    await action(caso, TableComponent, model);

                }
            }]: undefined
        }
    })
    const FilterOptions = new WFilterOptions({
        Dataset: [],
        ModelObject: model,
        AutoSetDate: true,
        Display: true,
        FilterFunction: (DFilt) => {
            TableComponent.Dataset = DFilt;
            TableComponent?.DrawTable();
        }
    });
    return WRender.Create({ className: "main-container", children: [FilterOptions, TableComponent] });
}
export { CaseSearcherToVinculate };
