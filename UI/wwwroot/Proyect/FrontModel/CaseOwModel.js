import { Tbl_Case_ModelComponent } from "./Tbl_CaseModule.js";
import { Cat_Dependencias_ModelComponent } from "./Cat_Dependencias.js";
import { Tbl_Servicios_ModelComponent } from "./Tbl_Servicios.js";

const CaseOwModel = async (Id_Dependencia) => {
    const dep = await new Cat_Dependencias_ModelComponent({ Id_Dependencia: Id_Dependencia }).GetOwDependencies();
    const tbl_servicios = dep.flatMap(d => d.Tbl_Servicios);
    const ModelObject = new Tbl_Case_ModelComponent({
        Tbl_Tareas: {
            type: "text", hidden: true
        }, Estado: {
            type: "text", hidden: true
        }, Cat_Dependencias: {
            type: "WSELECT", hidden: true
        }, Tbl_Servicios: {
            type: 'WSelect', hiddenFilter: true, ModelObject: new Tbl_Servicios_ModelComponent(), Dataset: tbl_servicios
        }
    });
    return ModelObject;
}
export { CaseOwModel }