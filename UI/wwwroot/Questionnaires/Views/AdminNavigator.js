//@ts-check
import { StylesControlsV2 } from "../../WDevCore/StyleModules/WStyleComponents.js";
import { WAppNavigator } from "../../WDevCore/WComponents/WAppNavigator.js";
import { WTableComponent } from "../../WDevCore/WComponents/WTableComponent.js";
import { ComponentsManager, WRender } from '../../WDevCore/WModules/WComponentsTools.js';
import { WOrtograficValidation } from "../../WDevCore/WModules/WOrtograficValidation.js";
import { Cat_Categorias_Test } from "../FrontModel/Cat_Categorias_Test.js";
import { Cat_Tipo_Preguntas } from "../FrontModel/Cat_Tipo_Preguntas.js";
import { Cat_Categorias_Test_ModelComponent } from "../FrontModel/ModelComponent/Cat_Categorias_Test_ModelComponent.js";
import { Cat_Tipo_Preguntas_ModelComponent } from "../FrontModel/ModelComponent/Cat_Tipo_Preguntas_ModelComponent.js";
window.addEventListener("load", async () => {
    setTimeout(async () => {
        // @ts-ignore
        const DOMManager = new ComponentsManager({ MainContainer: Main });
        // @ts-ignore
        Main.append(WRender.createElement(StylesControlsV2));
        // @ts-ignore
        Aside.append(WRender.Create({ tagName: "h3", innerText: "Mantenimiento de Tests" }));
        // @ts-ignore
        Aside.append(new WAppNavigator({
            Direction: "column",
            Elements: [
                ElementTab(DOMManager, new Cat_Tipo_Preguntas_ModelComponent(), new Cat_Tipo_Preguntas()),
                ElementTab(DOMManager, new Cat_Categorias_Test_ModelComponent(), new Cat_Categorias_Test())
            ]
        }));
    }, 100);
});
function ElementTab(DOMManager, ModelComponent, ModelEntity) {
    return {
        name: WOrtograficValidation.es(ModelEntity.constructor.name), url: "#",
        action: async (ev) => {
            const response = await ModelEntity.Get();
            const Table = new WTableComponent({
                Dataset: response,
                ModelObject: ModelComponent,
                EntityModel: ModelEntity,
                AutoSave: true,
                Options: {
                    Add: true,
                    Filter: true,
                    FilterDisplay: true,
                    Edit: true
                }
            });         
            DOMManager.NavigateFunction(ModelEntity.constructor.name, [WRender.Create({
                tagName: "h2",
                innerText: WOrtograficValidation.es(ModelEntity.constructor.name)
            }), Table]);
        }
    };
}