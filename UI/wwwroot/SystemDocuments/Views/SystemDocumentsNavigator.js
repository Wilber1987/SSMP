//@ts-check
import { LogErrorView } from "../../Admin/LogErrorView.js";
import { StylesControlsV2 } from "../../WDevCore/StyleModules/WStyleComponents.js";
import { WAppNavigator } from "../../WDevCore/WComponents/WAppNavigator.js";
import { WTableComponent } from "../../WDevCore/WComponents/WTableComponent.js";
import { ComponentsManager, WRender } from '../../WDevCore/WModules/WComponentsTools.js';
import { WOrtograficValidation } from "../../WDevCore/WModules/WOrtograficValidation.js";
import { Article_ModelComponent } from "../Models/ModelComponents/Article_ModelComponent.js";
import { Category_ModelComponent } from "../Models/ModelComponents/Category_ModelComponent.js";
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
                ElementTab(DOMManager, new Category_ModelComponent()),
                ElementTab(DOMManager, new Article_ModelComponent()),
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