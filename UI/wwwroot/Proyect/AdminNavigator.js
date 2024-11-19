//@ts-check
import { LogErrorView } from "../Admin/LogErrorView.js";
import { StylesControlsV2 } from "../WDevCore/StyleModules/WStyleComponents.js";
import { WAppNavigator } from "../WDevCore/WComponents/WAppNavigator.js";
import { WTableComponent } from "../WDevCore/WComponents/WTableComponent.js";
import { WAjaxTools } from "../WDevCore/WModules/WAjaxTools.js";
import { ComponentsManager, WRender } from '../WDevCore/WModules/WComponentsTools.js';
import { WOrtograficValidation } from "../WDevCore/WModules/WOrtograficValidation.js";
import { Cat_Cargos_Dependencias_ModelComponent } from "./FrontModel/Cat_Cargos_Dependencias.js";
import { Cat_Dependencias_ModelComponent } from "./FrontModel/Cat_Dependencias.js";
import { Cat_Paises_ModelComponent } from "./FrontModel/Cat_Paises.js";
import { Tbl_Servicios_ModelComponent } from "./FrontModel/Tbl_Servicios.js";
window.addEventListener("load", async () => {
    setTimeout(async () => {
        // @ts-ignore
        const DOMManager = new ComponentsManager({ MainContainer: Main });
        // @ts-ignore
        Main.append(WRender.createElement(StylesControlsV2));
        // @ts-ignore
        Aside.append(WRender.Create({ tagName: "h3", innerText: "Mantenimiento de Catalogos" }));
        // @ts-ignore
        Aside.append(new WAppNavigator({
            NavStyle: "tab",
            Elements: [
                ElementTab(DOMManager, new Cat_Dependencias_ModelComponent()),
                ElementTab(DOMManager, new Tbl_Servicios_ModelComponent()),
                //ElementTab(DOMManager, new Cat_Tipo_Servicio()),
                ElementTab(DOMManager, new Cat_Cargos_Dependencias_ModelComponent()),
                //ElementTab(DOMManager, new Cat_Tipo_Evidencia()),
                //ElementTab(DOMManager, new Cat_Tipo_Participaciones()),
                ElementTab(DOMManager, new Cat_Paises_ModelComponent())
            ]
        }));
    }, 100);
});
function ElementTab(DOMManager, Model) {
    return {
        name: WOrtograficValidation.es(Model.constructor.name), url: "#",
        action: async (ev) => {
            const Table = new WTableComponent({
                ModelObject: Model,
                AutoSave: true,
                Options: {
                    Filter: true,
                    FilterDisplay: true,
                    Add: true,
                    Edit: true
                }
            });

            return Table;
        }
    };
}