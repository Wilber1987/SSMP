//@ts-check
import { CaseSearcherToVinculate } from '../../../AppComponents/CaseSearcherToVinculate.js';
import { priorityStyles } from '../../../AppComponents/Styles.js';
import { Permissions, WSecurity } from '../../../WDevCore/Security/WSecurity.js';
import { StylesControlsV2, StylesControlsV3 } from "../../../WDevCore/StyleModules/WStyleComponents.js";
import { WFilterOptions } from '../../../WDevCore/WComponents/WFilterControls.js';
import { ModalMessege, ModalVericateAction, WForm } from "../../../WDevCore/WComponents/WForm.js";
import { WModalForm } from '../../../WDevCore/WComponents/WModalForm.js';
import { WPaginatorViewer } from '../../../WDevCore/WComponents/WPaginatorViewer.js';
import { WTableComponent } from "../../../WDevCore/WComponents/WTableComponent.js";
import { ComponentsManager, WRender } from '../../../WDevCore/WModules/WComponentsTools.js';
import { ControlBuilder } from '../../../WDevCore/WModules/WControlBuilder.js';
import { Cat_Dependencias_ModelComponent } from "../../FrontModel/Cat_Dependencias.js";
import { Tbl_Case, Tbl_Case_ModelComponent, Tbl_VinculateCase } from '../../FrontModel/Tbl_CaseModule.js';
import { Tbl_Agenda_ModelComponent } from "../../FrontModel/Tbl_Agenda.js";
import { Tbl_Tareas_ModelComponent } from "../../FrontModel/Tbl_Tareas.js";
import { activityStyle } from '../../style.js';
import { caseGeneralData } from './CaseDetailComponent.js';
import { Tbl_Comments_ModelComponent } from '../../FrontModel/Tbl_Comments.js';
import { Tbl_Calendario_ModelComponent } from '../../FrontModel/Tbl_Calendario.js';
import { Tbl_Servicios, Tbl_Servicios_ModelComponent } from '../../FrontModel/Tbl_Servicios.js';

class CaseManagerComponent extends HTMLElement {
    /**
     * 
     * @param {Array<Tbl_Case_ModelComponent>} [Dataset]
     * @param {Array<Cat_Dependencias_ModelComponent>} Dependencias
     */
    constructor(Dependencias, Dataset) {
        super();
        this.Dataset = Dataset = [];
        this.Dependencias = Dependencias;

        this.append(this.WStyle, StylesControlsV2.cloneNode(true), StylesControlsV3.cloneNode(true));
        this.TabContainer = WRender.createElement({ type: 'div', props: { class: "content-container", id: "TabContainer" } });
        this.TabManager = new ComponentsManager({ MainContainer: this.TabContainer });
        this.OptionContainer = WRender.Create({ className: "OptionContainer" });
        this.DrawCaseManagerComponent();
    }
    connectedCallback() { }
    DrawCaseManagerComponent = async () => {
        //this.OptionContainer.append(WRender.Create({ tagName: 'input', type: 'button', className: 'Block-Basic', value: 'Estadística', onclick: this.dashBoardView }))
        this.OptionContainer.append(WRender.Create({ tagName: 'input', type: 'button', className: 'Block-Alert', value: 'Lista de Casos', onclick: this.actividadesManager }))
        //this.OptionContainer.append(WRender.Create({ tagName: 'input', type: 'button', className: 'Block-Secundary', value: 'Dependencias', onclick: this.dependenciasViewer }))
        if (this.Dependencias.length != 0 && WSecurity.HavePermission(Permissions.ADMINISTRAR_CASOS_DEPENDENCIA)) {
            this.OptionContainer.append(WRender.Create({
                tagName: 'input', type: 'button', className: 'Block-Success',
                value: 'Nuevo Proyecto', onclick: this.CaseForm
            }))
        }
        if (WSecurity.HavePermission(Permissions.ADMINISTRAR_CASOS_PROPIOS)) {
            this.OptionContainer.append(WRender.Create({
                tagName: 'input', type: 'button', className: 'Block-Success',
                value: 'Nuevo Proyecto Propio', onclick: () => {
                    this.TabManager.NavigateFunction("Tab-CaseFormBasicView", basicCaseForm(new Tbl_Case(), async (/**@type {Tbl_Case} */ entity) => {
                        const response = await entity.SaveOwCase();
                        this.append(ModalMessege("Caso guardado correctamente", undefined, true))
                    }))
                } 
            }))
        }
        this.append(this.OptionContainer, this.TabContainer);
        //this.dashBoardView();
        this.actividadesManager();
    }
    actividadesManager = async () => {
        // @ts-ignore
        this.Paginator = new WPaginatorViewer({ Dataset: [] });
        this.FilterOptions = new WFilterOptions({
            Dataset: [],
            UseEntityMethods: false,
            AutoFilter: true,
            Display: true,
            AutoSetDate: true,
            ModelObject: new Tbl_Case_ModelComponent(),
            FilterFunction: async (/** @type {Array | undefined} */ DFilt) => {
                /**@type {Array<Tbl_Case>} */
                // @ts-ignore
                const data = await new Tbl_Case_ModelComponent({ FilterData: DFilt }).GetOwCase();
                const datasetMap = data.map(actividad => {
                    // @ts-ignore
                    actividad.Dependencia = actividad.Cat_Dependencias?.Descripcion;
                    actividad.Progreso = actividad.Tbl_Tareas?.filter(tarea => tarea.Estado?.includes("Finalizado")).length;
                    return this.actividadElement(actividad);
                });
                this.Paginator?.Draw(datasetMap);
            }
        });

        this.TabManager.NavigateFunction("Tab-Actividades-Manager",
            WRender.Create({
                className: "actividadesView", children: [this.FilterOptions, this.Paginator]
            }));
    }

    actividadElement = (actividad) => {
        this.append(priorityStyles.cloneNode(true));
        return WRender.Create({
            className: "actividad", object: actividad, children: [
                {
                    tagName: 'h4', innerText: `#${actividad.Id_Case} - ${actividad.Titulo} (${actividad.Tbl_Servicios?.Descripcion_Servicio ?? ""})`, children: [
                        {
                            className: "options", children: [
                                { tagName: 'button', className: 'Btn-Mini', innerText: "Detalle", onclick: async () => await this.actividadDetail(actividad) },
                                WSecurity.HavePermission(Permissions.ADMINISTRAR_CASOS_DEPENDENCIA) ?
                                    { tagName: 'button', className: 'Btn-Mini', innerText: 'Vincular Caso', onclick: () => this.Vincular(actividad) } :
                                    ""
                            ]
                        }
                    ]
                },
                , caseGeneralData(actividad),
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
        // @ts-ignore
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
            WRender.Create({
                className: "CaseFormView", children: [CaseForm(undefined, this.Dependencias, () => {
                    console.log(false);
                    this.append(ModalMessege("Caso guardado correctamente", undefined, true))
                })]
            }));
    }
    Vincular = async (actividad) => {
        this.append(new WModalForm({
            title: "Vincular Casos",
            ObjectModal: CaseSearcherToVinculate(actividad, "Vincular", async (caso_vinculado, TableComponent, model) => {
                this.append(ModalVericateAction(async () => {
                    const response = await new Tbl_VinculateCase({
                        Casos_Vinculados: [actividad, caso_vinculado]
                    }).VincularCaso();
                    const updateData = await model.Get();
                    TableComponent.Dataset = updateData;
                    TableComponent.DrawTable();
                }, "Esta seguro de Vincular este caso"))
            })
        }));
    }



    WStyle = activityStyle.cloneNode(true)
}
customElements.define('w-case-manager', CaseManagerComponent);
export { CaseForm, CaseManagerComponent, simpleCaseForm };
/**
 * @param {Tbl_Case_ModelComponent} [entity]
 * @param {Array<Cat_Dependencias_ModelComponent>} [dependencias]
 * @param {Function} [action] 
 * @returns {WForm}
 */
const CaseForm = (entity, dependencias, action) => {
    const ModelCalendar = {
        type: 'CALENDAR',
        ModelObject: () => new Tbl_Calendario_ModelComponent(),
        require: false,
        CalendarFunction: async () => {
            return {
                Agenda: await new Tbl_Agenda_ModelComponent({ Id_Dependencia: form.FormObject.Cat_Dependencias?.Id_Dependencia }).Get(),
                Calendario: await new Tbl_Calendario_ModelComponent({ Id_Dependencia: form.FormObject.Cat_Dependencias?.Id_Dependencia }).Get()
            }
        }
    }

    const form = new WForm({
        EditObject: entity,
        SaveFunction: action,
        ImageUrlPath: "",
        AutoSave: true,
        ModelObject: new Tbl_Case_ModelComponent({
            // @ts-ignore
            Tbl_Tareas: {
                type: 'MasterDetail',
                ModelObject: () => new Tbl_Tareas_ModelComponent({ Tbl_Calendario: ModelCalendar })
            }, Cat_Dependencias: {
                type: "WSELECT", hiddenFilter: true, ModelObject: new Cat_Dependencias_ModelComponent(),
                Dataset: dependencias,
                action: (caso) => {
                    caso.Tbl_Tareas
                        .forEach(Tbl_Tarea => Tbl_Tarea.Tbl_Calendario = []);
                    form.DrawComponent();
                }
            }
        })
    })
    return form;
}

/**
 * @param {Tbl_Case_ModelComponent} [entity]
 * @param {Array<Cat_Dependencias_ModelComponent>} [dependencias]
 * @param {Array<Tbl_Servicios>} [servicios] 
 * @param {Function} [action] 
 * @returns {WForm}
 */
const simpleCaseForm = (entity, dependencias, servicios, action) => {
    servicios = servicios?.map(s => {
        // @ts-ignore
        s.Descripcion = s.Descripcion_Servicio;
        return s;
    })
    const form = new WForm({
        EditObject: entity,
        SaveFunction: action,
        ImageUrlPath: "",
        ModelObject: new Tbl_Case_ModelComponent({
            // @ts-ignore
            Tbl_Tareas: { type: "text", hidden: true },
            Id_Vinculate: { type: "text", hidden: true },
            Titulo: { type: "text", hidden: true },
            Tbl_Servicios: {
                type: "wselect", ModelObject: new Cat_Dependencias_ModelComponent(),
                Dataset: servicios, require: false
            },
            Fecha_Inicio: { type: "text", hidden: true },
            Estado: { type: "text", hidden: true },
            Fecha_Final: { type: "text", hidden: true },
            Descripcion: { type: "text", hidden: true },
            // @ts-ignore
            Tbl_Comments: { type: "MasterDetail", ModelObject: new Tbl_Comments_ModelComponent(), label: "Comentario", hidden: true },
            Cat_Dependencias: {
                type: "WSELECT", hiddenFilter: true, ModelObject: new Cat_Dependencias_ModelComponent(),
                Dataset: dependencias,
                action: async (caso) => {
                    const servicios = await new Tbl_Servicios_ModelComponent({ Id_Dependencia: caso.Cat_Dependencias?.Id_Dependencia }).Get();
                    form.ModelObject.Tbl_Servicios.Dataset = servicios;
                    form.DrawComponent();
                }
            },
        })
    })
    return form;
}
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

