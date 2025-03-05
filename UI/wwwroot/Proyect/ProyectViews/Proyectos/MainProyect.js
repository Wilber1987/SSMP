
//@ts-check
import { WAppNavigator } from '../../../WDevCore/WComponents/WAppNavigator.js';
import { WFilterOptions } from '../../../WDevCore/WComponents/WFilterControls.js';
import { ComponentsManager, WRender } from '../../../WDevCore/WModules/WComponentsTools.js';
import { WCssClass, WStyledRender } from '../../../WDevCore/WModules/WStyledRender.js';
import { Cat_Dependencias_ModelComponent } from "../../FrontModel/Cat_Dependencias.js";
import { Tbl_Tareas_ModelComponent } from "../../FrontModel/Tbl_Tareas.js";
import { CaseForm, CaseManagerComponent } from './CaseManagerComponent.js';
import { TaskManagers } from './TaskManager.js';
import { Permissions, WSecurity } from '../../../WDevCore/Security/WSecurity.js';
import { ModalMessage } from '../../../WDevCore/WComponents/ModalMessage.js';
import { Tbl_Case, Tbl_Case_ModelComponent } from '../../FrontModel/Tbl_CaseModule.js';
import { WForm } from '../../../WDevCore/WComponents/WForm.js';


const OnLoad = async () => {
    // @ts-ignore
    Main.append(WRender.Create({ tagName: "h3", innerText: "Administración de Actividades" }));
    const AdminPerfil = new MainProyect();
    // @ts-ignore
    //Main.append(AdminPerfil.MainNav);
    // @ts-ignore
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
        this.Dependencias  = []
        this.DrawComponent();
    }
    EditarPerfilNav = () => { }
    
    BuildNavElements() {
        const nav = [
            {
                name: "Mis Actividades", action: async (ev) => { this.NavChargeActividades(); }
            },
            // {
            //     name: "Tareas", action: async (ev) => { this.NavChargeTasks(); }
            // }, 
            {
                name: "Mis Tareas", action: async (ev) => { this.NavChargeOWTasks(); }
            }
        ];

        // @ts-ignore
        console.log(this.Dependencias);
        
        if (this.Dependencias.length != 0 && WSecurity.HavePermission(Permissions.ADMINISTRAR_CASOS_DEPENDENCIA)) {
            nav.push({
                name: 'Nuevo Proyecto', action: ()=> this.CaseForm()
            })
        }
        if (WSecurity.HavePermission(Permissions.ADMINISTRAR_CASOS_PROPIOS)) {
            nav.push({
                name: 'Nuevo Proyecto Propio', action: async () => {
                    this.TabManager.NavigateFunction("Tab-CaseFormBasicView",
                        basicCaseForm(new Tbl_Case(), async (/**@type {Tbl_Case} */ entity) => {
                            const response = await entity.SaveOwCase();
                            this.append(ModalMessage("Caso guardado correctamente", undefined, true))
                        }))
                }
            })
        }
        return nav;
    }
    CaseForm = async () => {
        this.TabManager.NavigateFunction("Tab-CaseFormView",
            WRender.Create({
                className: "CaseFormView", children: [CaseForm(undefined, await this.Dependencias, () => {
                    console.log(false);
                    this.append(ModalMessage("Caso guardado correctamente", undefined, true))
                })]
            }));
    }

    connectedCallback() { }
    DrawComponent = async () => {
        this.Dependencias = await new Cat_Dependencias_ModelComponent().GetOwDependencies();
        this.MainNav = new WAppNavigator({
            NavStyle: "tab",
            Inicialize: true,
            Elements: this.BuildNavElements()
        });

        this.append(this.MainNav, this.OptionContainer, this.TabContainer);
    }
    NavChargeActividades = async () => {
        //const dataset = await new Tbl_Case().GetOwCase();        
        this.TabManager.NavigateFunction("Tab-OwActividades",
            new CaseManagerComponent(this.Dependencias));
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
        // @ts-ignore
        const tasksManager = new TaskManagers(tasks);
        const filterOptions = new WFilterOptions({
            Dataset: tasks,
            AutoSetDate: true,
            Display: true,
            ModelObject: new Tbl_Tareas_ModelComponent(),
            //DisplayFilts: [],
            FilterFunction: (DFilt) => {
                // @ts-ignore
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
/**
 * @param {Tbl_Case} [entity]
 * @param {Function} [action] 
 * @returns {WForm}
 */
const basicCaseForm = (entity, action) => {
    const form = new WForm({
        EditObject: entity,
        SaveFunction: action,
        ImageUrlPath: "",
        AutoSave: false,
        ModelObject: new Tbl_Case_ModelComponent({
            //Tbl_Tareas: { type: "text", hidden: true },
            Id_Vinculate: { type: "text", hidden: true },
            Titulo: { type: "text" },
            Tbl_Servicios: undefined,
            Fecha_Inicio: { type: "DATE" },
            Estado: { type: "text", hidden: true },
            Fecha_Final: { type: "text", hidden: true },
            // @ts-ignore
            Tbl_Comments: undefined,
            Cat_Dependencias: undefined,
        })
    })
    return form;
}