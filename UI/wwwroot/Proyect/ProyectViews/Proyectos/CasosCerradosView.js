

import { priorityStyles } from '../../../AppComponents/Styles.js';
import { Tbl_Case_ModelComponent } from '../../FrontModel/Tbl_CaseModule.js';
import { StylesControlsV2, StylesControlsV3 } from "../../../WDevCore/StyleModules/WStyleComponents.js";
import { WFilterOptions } from '../../../WDevCore/WComponents/WFilterControls.js';
import { ModalMessege, ModalVericateAction } from "../../../WDevCore/WComponents/WForm.js";
import { WPaginatorViewer } from '../../../WDevCore/WComponents/WPaginatorViewer.js';
import { WTableComponent } from "../../../WDevCore/WComponents/WTableComponent.js";
import { ComponentsManager, WRender } from '../../../WDevCore/WModules/WComponentsTools.js';
import { ControlBuilder } from '../../../WDevCore/WModules/WControlBuilder.js';
import { css } from '../../../WDevCore/WModules/WStyledRender.js';
import { activityStyle } from '../../style.js';
import { caseGeneralData } from './CaseDetailComponent.js';
import {Cat_Dependencias_ModelComponent} from "../../FrontModel/Cat_Dependencias.js";

/**
 * @typedef {Object} ComponentConfig
 * * @property {Object} [propierty]
 */
class CasosCerradosView extends HTMLElement {
    /**
    * 
    * @param {Array<Tbl_Case_ModelComponent>} Dataset
    * @param {Array<Cat_Dependencias_ModelComponent>} Dependencias
    */
    constructor(Dataset, Dependencias) {
        super();
        this.Dataset = Dataset;
        this.Dependencias = Dependencias;
        this.append(this.WStyle, StylesControlsV2.cloneNode(true), StylesControlsV3.cloneNode(true));
        this.TabContainer = WRender.createElement({ type: 'div', props: { class: "content-container", id: "TabContainer" } });
        this.TabManager = new ComponentsManager({ MainContainer: this.TabContainer });
        this.OptionContainer = WRender.Create({ className: "" });
        this.ModelObject = new Tbl_Case_ModelComponent({
            Tbl_Tareas: undefined, Estado: undefined, Cat_Dependencias: {
                type: "WSELECT", hiddenFilter: true, ModelObject: () => new Cat_Dependencias_ModelComponent()
            }, Get: async () => {
                return await this.ModelObject.GetData("Proyect/GetOwCloseCase");
            }
        });
        this.DrawCaseManagerComponent();
    }
    connectedCallback() { }
    DrawCaseManagerComponent = async () => {
        this.innerHTML= "";
        //this.OptionContainer.append(WRender.Create({ tagName: 'input', type: 'button', className: 'Block-Basic', value: 'Estadística', onclick: this.dashBoardView }))
        this.append(this.WActivityStyle, this.OptionContainer, this.TabContainer);
        //this.dashBoardView();
        this.actividadesManager();
    }
    actividadesManager = async () => {
        const datasetMap = this.generatePaginatorList(this.Dataset);
        this.paginator = new WPaginatorViewer({ Dataset: datasetMap, userStyles: [StylesControlsV2] });
        this.FilterOptions = new WFilterOptions({
            Dataset: this.Dataset,
            AutoSetDate: true,
            ModelObject: this.ModelObject,
            FilterFunction: (DFilt) => {
                this.paginator?.Draw(this.generatePaginatorList(DFilt));
            }
        });
        this.TabManager.NavigateFunction("Tab-Actividades-Manager",
            WRender.Create({ className: "actividadesView", children: [this.FilterOptions, this.paginator] }));
    }
    generatePaginatorList = (Dataset) => {
        return Dataset.filter(x => x.Estado != "Vinculado").map(actividad => {
            actividad.Dependencia = actividad.Cat_Dependencias?.Descripcion;
            actividad.Progreso = actividad.Tbl_Tareas?.filter(tarea => tarea.Estado?.includes("Finalizado")).length;
            return this.actividadElement(actividad);
        });
    }

    actividadElement = (actividad) => {
        this.append(priorityStyles.cloneNode(true));
        return WRender.Create({
            className: "actividad", object: actividad, children: [
                {
                    tagName: 'h4', innerText: `#${actividad.Id_Case} - ${actividad.Titulo} (${actividad.Tbl_Servicios?.Descripcion_Servicio ?? ""})`,
                    children: [
                        {
                            className: "options", children: [
                                { tagName: 'button', className: 'Btn-Mini', innerText: "Detalle", onclick: async () => await this.actividadDetail(actividad) },
                                { tagName: 'button', className: 'Btn-Mini', innerText: 'Reabrir Caso', onclick: () => this.Reabrir(actividad) }
                            ]
                        }
                    ]
                }, caseGeneralData(actividad),
                { tagName: 'h4', innerText: "Progreso" },
                ControlBuilder.BuildProgressBar(actividad.Progreso,
                    actividad.Tbl_Tareas?.filter(tarea => !tarea.Estado?.includes("Inactivo"))?.length)
            ]
        })
    }
    actividadElementDetail = (actividad) => {
        return WRender.Create({
            className: "actividadDetail", object: actividad, children: [
                this.actividadElement(actividad)
            ]
        })
    }

    actividadDetail = async (actividad = (new Tbl_Case_ModelComponent())) => {
        sessionStorage.setItem("detailCase", JSON.stringify(actividad));
        window.location = "/ProyectViews/CaseDetail"
    }
    dependenciasViewer = async () => {
        const dependenciasDetailView = WRender.Create({ className: "", children: [] });
        //const tareasActividad = await new Cat_Dependencias().Get();
        dependenciasDetailView.append(new WTableComponent({
            ModelObject: new Cat_Dependencias_ModelComponent({}),
            Options: {
                Add: true, UrlAdd: "../api/ApiEntityDBO/saveCat_Dependencias",
                Edit: true, UrlUpdate: "../api/ApiEntityDBO/updateCat_Dependencias",
                Search: true, UrlSearch: "../api/ApiEntityDBO/getCat_Dependencias",
                UserActions: []
            }
        }))
        this.TabManager.NavigateFunction("Tab-Dependencias-Viewer", dependenciasDetailView);
    }
    CaseForm = async () => {
        this.TabManager.NavigateFunction("Tab-CaseFormView",
            WRender.Create({ className: "CaseFormView", children: [CaseForm(undefined, this.Dependencias)] }));
    }
    Reabrir = async (actividad) => {
        const modal = ModalVericateAction(async () => {
            actividad.Estado = "Activo"
            const response = await new Tbl_Case_ModelComponent(actividad).Update();
            if (response.status == 200) {
                this.append(ModalMessege("Caso reabierto exitosamente")); 
                this.removeChild(modal)   
                //this.update()           
            } else {
                this.append(ModalMessege(response.message))
            }
        }, "¿Está seguro que desea reabrir este caso?")
        this.append(modal)
       
    }
    update = async () => {
        // const find = actividad.Tbl_Tareas.find(t => t.Id_Tarea == task.Id_Tarea);
        // for (const prop in task) {
        //     find[prop] = task[prop]
        // }
       
        this.DrawCaseManagerComponent();
    }
    WActivityStyle = activityStyle.cloneNode(true);
    WStyle = css`        
        w-coment-component {
            grid-row: span 3;
        }
        .dashBoardView w-colum-chart { 
            grid-column: span 2;
        }        
    `
}
customElements.define('w-caos-cerrados-view', CasosCerradosView);
window.onload = async () => {
    const dependencias = await new Cat_Dependencias_ModelComponent().GetOwDependencies();
    const dataset = await new Tbl_Case_ModelComponent().GetOwCloseCase();
    Main.appendChild(new CasosCerradosView(dataset, dependencias));
}
export { CasosCerradosView };
