
import { priorityStyles } from '../../AppComponents/Styles.js';
import { WSecurity } from '../../WDevCore/Security/WSecurity.js';
import { StylesControlsV2, StylesControlsV3 } from "../../WDevCore/StyleModules/WStyleComponents.js";
import { WCommentsComponent } from '../../WDevCore/WComponents/WCommentsComponent.js';
import { WDetailObject } from '../../WDevCore/WComponents/WDetailObject.js';
import { WFilterOptions } from "../../WDevCore/WComponents/WFilterControls.js";
import { ModalMessege, WForm } from "../../WDevCore/WComponents/WForm.js";
import { WPaginatorViewer } from '../../WDevCore/WComponents/WPaginatorViewer.js';
import { ComponentsManager, html, WRender } from '../../WDevCore/WModules/WComponentsTools.js';
import { Cat_Dependencias_ModelComponent } from "../FrontModel/Cat_Dependencias.js";
import { Tbl_Case_ModelComponent } from '../FrontModel/Tbl_CaseModule.js';
import { Tbl_Comments_ModelComponent } from '../FrontModel/Tbl_Comments.js';
import { Tbl_Profile_CasosAsignados, Tbl_Profile_CasosAsignados_ModelComponent } from '../FrontModel/Tbl_Profile_CasosAsignados.js';
import { Tbl_Servicios } from '../FrontModel/Tbl_Servicios.js';
import { caseGeneralData } from '../ProyectViews/Proyectos/CaseDetailComponent.js';
import { activityStyle } from '../style.js';

const OnLoad = async () => {
    const Solicitudes = await new Tbl_Case_ModelComponent().GetOwSolicitudesPendientesAprobar();
    const AdminPerfil = new MainSolicitudesView(Solicitudes);
    Main.appendChild(AdminPerfil);
}
window.onload = OnLoad;
class MainSolicitudesView extends HTMLElement {
    /**
     * 
     * @param {Array<Tbl_Case_ModelComponent>} Dataset
     * @param {Array<Cat_Dependencias_ModelComponent>} Dependencias
     */
    constructor(Dataset) {
        super();
        this.Dataset = Dataset;
        //this.Dependencias = Dependencias;
        this.append(this.WStyle, StylesControlsV2.cloneNode(true), StylesControlsV3.cloneNode(true));
        this.TabContainer = WRender.createElement({ type: 'div', props: { class: "content-container", id: "TabContainer" } });
        this.TabManager = new ComponentsManager({ MainContainer: this.TabContainer });
        this.OptionContainer = WRender.Create({ className: "OptionContainer" });
        this.ModelObject = new Tbl_Case_ModelComponent({
            Tbl_Tareas: {
                type: "text", hidden: true
            }, Estado: {
                type: "text", hidden: true
            }
        });

        this.DrawMainSolicitudesView();
    }
    connectedCallback() { }
    DrawMainSolicitudesView = async () => {
        //this.OptionContainer.append(WRender.Create({ tagName: 'input', type: 'button', className: 'Block-Basic', value: 'Estadística', onclick: this.dashBoardView }))
        this.OptionContainer.append(WRender.Create({
            tagName: 'input', type: 'button', className: 'Block-Primary', value: 'Pendientes',
            onclick: async () =>
                this.actividadesManager(this.Dataset, "Pendientes")
        }))
        this.OptionContainer.append(WRender.Create({
            tagName: 'input', type: 'button', className: 'Block-Secundary', value: 'Aprobadas', onclick: async () =>
                this.actividadesManager(await new Tbl_Case_ModelComponent().GetOwSolicitudesAprobadas(), "Aprobadas")
        }))
        this.OptionContainer.append(WRender.Create({
            tagName: 'input', type: 'button', className: 'Block-Tertiary', value: 'Rechazadas', onclick: async () =>
                this.actividadesManager(await new Tbl_Case_ModelComponent().GetOwSolicitudesRechazadas(), "Rechazadas")
        }))
        this.OptionContainer.append(WRender.Create({
            tagName: 'input', type: 'button', className: 'Block-Fourth', value: 'Vinculadas', onclick: async () =>
                this.actividadesManager(await new Tbl_Case_ModelComponent().GetOwSolicitudesVinculadas(), "Vinculadas")
        }))
        this.OptionContainer.append(WRender.Create({
            tagName: 'input', type: 'button', className: 'Block-Alert', value: 'Finalizadas', onclick: async () =>
                this.actividadesManager(await new Tbl_Case_ModelComponent().GetOwSolicitudesFinalizadas(), "Finalizadas")
        }))
        this.OptionContainer.append(WRender.Create({ tagName: 'input', type: 'button', className: 'Block-Success', value: 'Nuevo Proyecto', onclick: this.nuevoCaso }))

        this.append(this.OptionContainer, this.TabContainer);
        //this.dashBoardView();
        this.actividadesManager();
    }
    actividadesManager = async (Dataset = this.Dataset, View = "Pendientes") => {
        const paginator = new WPaginatorViewer({
            Dataset: this.mapCaseToPaginatorElement(Dataset), 
            userStyles: [StylesControlsV2], 
            Options: { Search: false }
        });
        this.FilterOptions = new WFilterOptions({
            Dataset: Dataset,
            AutoSetDate: true,
            ModelObject: this.ModelObject,
            Display: true,
            FilterFunction: (DFilt) => {
                this.paginator?.Draw(DFilt);
            }
        });
        this.TabManager.NavigateFunction("Tab-Actividades-Manager-" + View,
            WRender.Create({
                className: "actividadesView",
                children: [
                    html`<h1>${View}</h1>`,
                    this.FilterOptions,
                    paginator
                ]
            }));
    }

    actividadElement = (actividad) => {
        this.append(priorityStyles.cloneNode(true));
        return WRender.Create({
            className: "actividad", object: actividad, children: [
                { tagName: 'h4', innerText: `#${actividad.Id_Case} - ${actividad.Titulo} (${actividad.Tbl_Servicios?.Descripcion_Servicio ?? ""})` },
                caseGeneralData(actividad), {
                    className: "options", children: [
                        {
                            tagName: 'button', className: 'Btn-Mini', innerText: "Detalle",
                            onclick: async () => await this.actividadDetail(actividad)
                        },
                    ]
                },
            ]
        })
    }
    actividadDetail = async (actividad) => {
        actividad.Tbl_Profile_CasosAsignados = await new Tbl_Profile_CasosAsignados({ Id_Case: actividad.Id_Case }).Get();
        const actividadDetailView = WRender.Create({
            className: "actividadDetailView",
            style: {
                display: "grid",
                gap: "20px",
                gridTemplateColumns: "calc(100% - 620px) 600px"
            },
            children: [
                new WDetailObject({
                    ObjectDetail: actividad, ModelObject: new Tbl_Case_ModelComponent({
                        // @ts-ignore
                        Tbl_Profile_CasosAsignados: {
                            type: "MASTERDETAIL",
                            label: "Participantes",
                            ModelObject: new Tbl_Profile_CasosAsignados_ModelComponent()
                        }
                    })
                })]
        });
        const commentsDataset = await new Tbl_Comments_ModelComponent({ Id_Case: actividad.Id_Case }).Get();
        const commentsContainer = new WCommentsComponent({
            Dataset: commentsDataset,
            ModelObject: new Tbl_Comments_ModelComponent(),
            User: WSecurity.UserData,
            UserIdProp: "Id_User",
            CommentsIdentify: actividad.Id_Case,
            CommentsIdentifyName: "Id_Case",
            UrlSearch: "../api/ApiEntityHelpdesk/getTbl_Comments",
            UrlAdd: "../api/ApiEntityHelpdesk/saveTbl_Comments"
        });
        actividadDetailView.append(commentsContainer)
        this.TabManager.NavigateFunction("Tab-Actividades-Viewer" + actividad.Id_Case, actividadDetailView);
    }

    actividadElementDetail = (actividad) => {
        return WRender.Create({
            className: "actividadDetail", object: actividad, children: [
                this.actividadElement(actividad)
            ]
        })
    }

    nuevoCaso = async () => {
        const form = new WForm({
            ModelObject: this.ModelObject,
            AutoSave: true,
            SaveFunction: () => {
                this.append(ModalMessege("Aviso", "Caso guardado correctamente", true))
            }
        });
        //TODO REVISAR COMO HAYA UNA CARGA REAL DE FORMA SINCRONA
        setTimeout(async () => {
            const servicios = await new Tbl_Servicios({ Id_Dependencia: form.ModelObject.Cat_Dependencias.Dataset[0].Id_Dependencia }).Get();
            form.ModelObject.Tbl_Servicios.Dataset = servicios;
            form.DrawComponent();
            await this.TabManager.NavigateFunction("Tab-nuevoCasoView",
                WRender.Create({ className: "nuevoCasoView", children: [form] }));
        }, 100);
    }


    WStyle = activityStyle.cloneNode(true);

    mapCaseToPaginatorElement(Dataset) {
        return Dataset.map(actividad => {
            actividad.Dependencia = actividad.Cat_Dependencias.Descripcion;
            //actividad.Progreso = actividad.Tbl_Tareas?.filter(tarea => tarea.Estado?.includes("Finalizado")).length;
            return this.actividadElement(actividad);
        });
    }
}
customElements.define('w-main-solicitudes', MainSolicitudesView);
export { MainSolicitudesView };

