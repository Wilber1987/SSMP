import { WRender, ComponentsManager } from '../WDevCore/WModules/WComponentsTools.js';
import { WAppNavigator } from "../WDevCore/WComponents/WAppNavigator.js";
import { Transactional_ConfiguracionesView } from './Transactional_ConfiguracionesView.js';
const DOMManager = new ComponentsManager({ MainContainer: Main, SPAManage: true });
window.addEventListener("load", async () => {
    Aside.append(WRender.Create({ tagName: "h3", innerText: "Mantenimiento" }));
    Aside.append(new WAppNavigator({
        DarkMode: false,
        NavStyle: "tab",
        Elements: [
            {
                name: "Config", action: () => {
                    return  new Transactional_ConfiguracionesView();
                }
            }
        ]
    }));
});