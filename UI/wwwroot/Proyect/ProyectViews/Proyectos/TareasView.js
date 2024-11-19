import { StylesControlsV2, StylesControlsV3, StyleScrolls } from "../../../WDevCore/StyleModules/WStyleComponents.js";
import { WFilterOptions } from "../../../WDevCore/WComponents/WFilterControls.js";
import { ComponentsManager, WRender } from "../../../WDevCore/WModules/WComponentsTools.js";
import { css } from "../../../WDevCore/WModules/WStyledRender.js";
import { Tbl_Tareas_ModelComponent } from "../../FrontModel/Tbl_Tareas.js";
import { TaskManagers } from "./TaskManager.js";
/**
 * @typedef {Object} ComponentConfig
 * * @property {Object} [propierty]
 */
class TareasComponentView extends HTMLElement {
    /**
     * 
     * @param {ComponentConfig} props 
     */
    constructor(props) {
        super();        
        this.OptionContainer = WRender.Create({ className: "" });
        this.TabContainer = WRender.Create({ className: "TabContainer", id: 'TabContainer' });
        this.Manager = new ComponentsManager({ MainContainer: this.TabContainer, SPAManage: false });
        this.append(this.CustomStyle);
        this.append(
            StylesControlsV2.cloneNode(true),
            StyleScrolls.cloneNode(true),
            StylesControlsV3.cloneNode(true),
            this.OptionContainer,
            this.TabContainer
        );
        this.Model = new Tbl_Tareas_ModelComponent({
            Get: async () => {
                return this.Model.GetOwParticipations();
            }
        });
        this.Draw();
    }
    Draw = async () => {
        this.SetOption();
        //const tasks = await new Tbl_Tareas().GetOwParticipations();
        this.Manager.NavigateFunction("Tab-OWTasks-Manager", this.ChargeTasks());
    }
    SetOption() {
        /*  this.OptionContainer.append(WRender.Create({
             tagName: 'button', className: 'Block-Primary', innerText: 'Datos Tareas',
             onclick: async () => {
                 
             } 
         }))       */
    }
    ChargeTasks() {
        const tasksManager = new TaskManagers([]);
        const filterOptions = new WFilterOptions({
            AutoSetDate: true,
            Display: true,
            ModelObject: this.Model,
            UseEntityMethods: false,
            AutoFilter: true,
            FilterFunction: async (DFilt) => {
                const tasks = await new Tbl_Tareas_ModelComponent({FilterData: DFilt}).GetOwParticipations();
                tasksManager.Dataset = tasks;
                tasksManager.DrawTaskManagers();
            }
        })
        return WRender.Create({ className: "task-container", children: [filterOptions, tasksManager] })
    }
    CustomStyle = css`
        .component{
           display: block;
        }           
        .task-container {
            height: 100%;
        }
    `
}
window.onload = async () => {
    Main.appendChild(new TareasComponentView());
}
customElements.define('w-tareas-view', TareasComponentView);
export { TareasComponentView };
