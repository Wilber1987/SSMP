
import { WAppNavigator } from '../../../WDevCore/WComponents/WAppNavigator.js';
import { WFilterOptions } from '../../../WDevCore/WComponents/WFilterControls.js';
import { ComponentsManager, WRender } from '../../../WDevCore/WModules/WComponentsTools.js';
import { WCssClass, WStyledRender } from '../../../WDevCore/WModules/WStyledRender.js';
import { Cat_Dependencias_ModelComponent } from "../../FrontModel/Cat_Dependencias.js";
import { Tbl_Tareas_ModelComponent } from "../../FrontModel/Tbl_Tareas.js";
import { CaseManagerComponent } from './CaseManagerComponent.js';
import { TaskManagers } from './TaskManager.js';

const OnLoad = async () => {
    Main.append(WRender.Create({ tagName: "h3", innerText: "Administración de Actividades" }));
    const AdminPerfil = new MainProyect();
    Main.append(AdminPerfil.MainNav);
    Main.appendChild(AdminPerfil);
}
window.onload = OnLoad;

class MainProyect extends HTMLElement {
    constructor() {
        super();
        this.id = "MainProyect";
        this.className = "MainProyect DivContainer";
        this.TabContainer = WRender.createElement({ type: 'div', props: { class: '', id: "TabContainer" } });
        this.TabManager = new ComponentsManager({ MainContainer: this.TabContainer });
        this.OptionContainer = WRender.Create({ className: "" });
        this.DrawComponent();
    }
    EditarPerfilNav = () => { }
    MainNav = new WAppNavigator({
        NavStyle: "tab",
        Inicialize: true,
        Elements: [
            // {
            //     name: "Datos Generales",
            //     action: async (ev) => {
            //         const dataset = await new Tbl_Case().Get();
            //         const dependencias = await new Cat_Dependencias().Get();
            //         this.TabManager.NavigateFunction("Tab-Generales",
            //             new CaseManagerComponent(dataset, dependencias));
            //     }
            // },
            {
                name: "Mis Actividades", action: async (ev) => { this.NavChargeActividades(); }
            },
            // {
            //     name: "Tareas", action: async (ev) => { this.NavChargeTasks(); }
            // }, 
            {
                name: "Mis Tareas", action: async (ev) => { this.NavChargeOWTasks(); }
            }]
    });
    connectedCallback() { }
    DrawComponent = async () => {
        this.append(this.OptionContainer, this.TabContainer);
    }
    NavChargeActividades = async () => {
        const dependencias = await new Cat_Dependencias_ModelComponent().GetOwDependencies();
        //const dataset = await new Tbl_Case().GetOwCase();        
        this.TabManager.NavigateFunction("Tab-OwActividades",
            new CaseManagerComponent( dependencias));
    }
    NavChargeTasks = async () => {
        const tasks = await new Tbl_Tareas_ModelComponent().Get();
        this.TabManager.NavigateFunction("Tab-Tasks-Manager", this.ChargeTasks(tasks));
    }
    NavChargeOWTasks = async () => {
        const tasks = await new Tbl_Tareas_ModelComponent().GetOwParticipations();
        this.TabManager.NavigateFunction("Tab-OWTasks-Manager", this.ChargeTasks(tasks));
    }
    ChargeTasks(tasks) {
        const tasksManager = new TaskManagers(tasks);
        const filterOptions = new WFilterOptions({
            Dataset: tasks,
            AutoSetDate: true,
            Display: true,
            ModelObject: new Tbl_Tareas_ModelComponent(),
            //DisplayFilts: [],
            FilterFunction: (DFilt) => {
                tasksManager.DrawTaskManagers(DFilt);
            }
        })
        return WRender.Create({ className: "task-container", children: [filterOptions, tasksManager] })
    }

    WStyle = new WStyledRender({
        ClassList: [
            new WCssClass(`.MainProyect`, {
            }), new WCssClass(`.OptionContainer`, {
                display: 'flex',
                "justify-content": "center",
                margin: "0 0 20px 0"
            }), new WCssClass(`.OptionContainer img`, {
                "box-shadow": "0 0 4px rgb(0,0,0/50%)",
                height: 100,
                width: 100,
                margin: 10
            }), new WCssClass(`.TabContainer`, {
                overflow: "hidden",
                "overflow-y": "auto"
            }), new WCssClass(`.FormContainer`, {
                "background-color": '#fff',
            })
        ], MediaQuery: [{
            condicion: '(max-width: 600px)',
            ClassList: []
        }]
    });
    Icons = {
        New: "",
        View: "",
    }
}

customElements.define('w-proyect-class', MainProyect);
