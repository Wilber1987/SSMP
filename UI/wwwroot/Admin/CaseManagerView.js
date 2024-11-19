import { Cat_Dependencias_ModelComponent } from "../Proyect/FrontModel/Cat_Dependencias.js";
import { Tbl_Case_ModelComponent } from "../Proyect/FrontModel/Tbl_CaseModule.js";
import { Tbl_Tareas_ModelComponent } from "../Proyect/FrontModel/Tbl_Tareas.js";
import { CaseDashboardComponent } from "../Proyect/ProyectViews/Proyectos/CaseDashboardComponent.js";
import { CaseManagerComponent } from "../Proyect/ProyectViews/Proyectos/CaseManagerComponent.js";
import { TaskManagers } from "../Proyect/ProyectViews/Proyectos/TaskManager.js";
import { SolicitudesPendientesComponent } from "../Proyect/Solicitudes/SolicitudesPendientesComponent.js";
import { StylesControlsV2 } from "../WDevCore/StyleModules/WStyleComponents.js";
import { WAppNavigator } from "../WDevCore/WComponents/WAppNavigator.js";
import { WFilterOptions } from "../WDevCore/WComponents/WFilterControls.js";
import { ComponentsManager, WRender } from '../WDevCore/WModules/WComponentsTools.js';
window.onload = () => {
    const navigator = new WAppNavigator({
        Inicialize: true,
        NavStyle: "tab",
        Elements: [{
            id: "Tab-dasboard", name: "Dashboard", action: async (ev) => {
                //const dataset = await new Tbl_Case().GetOwCase();
                //const dependencias = await new Cat_Dependencias().GetOwDependencies();
                return new CaseDashboardComponent();
            }
        }, {
            id: "Tab-Generales", name: "Administrador de Casos",
            action: async (ev) => {
                //const dataset = await new Tbl_Case().Get();
                const dependencias = await new Cat_Dependencias_ModelComponent().Get();
                return new CaseManagerComponent(dependencias);
            }
        }, {
            id: "Tab-Solicitudes", name: "Administrador de Solicitudes",
            action: async (ev) => {
                const Solicitudes = await new Tbl_Case_ModelComponent().GetSolicitudesPendientesAprobarAdmin();
                return new SolicitudesPendientesComponent(Solicitudes);
            }
        }, {
            id: "Tab-Tasks-Manager", name: "Administrador de Tareas", action: async (ev) => {
                //const tasks = await new Tbl_Tareas().Get();
                return ChargeTasks();
            }
        }]
    })
    const DOMManager = new ComponentsManager({ MainContainer: Main, SPAManage: true, WNavigator: navigator });
    Main.append(WRender.createElement(StylesControlsV2));
    Aside.append(WRender.Create({ tagName: "h3", innerText: "Administración de Casos" }))
    Aside.append(navigator);
}
const ChargeTasks = () => {
    const tasksManager = new TaskManagers([], new Tbl_Tareas_ModelComponent(), { ImageUrlPath: "" });
    const filterOptions = new WFilterOptions({
        Display: true,
        AutoSetDate: true,
        ModelObject: new Tbl_Tareas_ModelComponent(),
        UseEntityMethods: true,
        AutoFilter: true,
        //DisplayFilts: [],
        FilterFunction: (DFilt) => {
            tasksManager.Dataset = DFilt;
            tasksManager.DrawTaskManagers();
        }
    })
    return WRender.Create({
        className: "task-container",
        children: [filterOptions, tasksManager]
    })
}
