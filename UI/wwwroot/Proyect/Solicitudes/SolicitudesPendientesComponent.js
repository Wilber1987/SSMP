//@ts-check
import { CaseSearcherToVinculate } from '../../AppComponents/CaseSearcherToVinculate.js';
import { priorityStyles } from '../../AppComponents/Styles.js';
import { Permissions, WSecurity } from '../../WDevCore/Security/WSecurity.js';
import { StylesControlsV2, StylesControlsV3 } from "../../WDevCore/StyleModules/WStyleComponents.js";
import { WCommentsComponent } from '../../WDevCore/WComponents/WCommentsComponent.js';
import { WFilterOptions } from "../../WDevCore/WComponents/WFilterControls.js";
import { ModalMessege, ModalVericateAction, WForm } from "../../WDevCore/WComponents/WForm.js";
import { WModalForm } from '../../WDevCore/WComponents/WModalForm.js';
import { WTableComponent } from '../../WDevCore/WComponents/WTableComponent.js';
import { ComponentsManager, WRender } from '../../WDevCore/WModules/WComponentsTools.js';
import { Cat_Dependencias_ModelComponent } from "../FrontModel/Cat_Dependencias.js";
import { Tbl_Case_ModelComponent, Tbl_VinculateCase } from '../FrontModel/Tbl_CaseModule.js';
import { Tbl_Comments_ModelComponent } from '../FrontModel/Tbl_Comments.js';
import { Tbl_Servicios_ModelComponent } from '../FrontModel/Tbl_Servicios.js';
import { CaseDetailComponent, caseGeneralData } from '../ProyectViews/Proyectos/CaseDetailComponent.js';
import { simpleCaseForm } from '../ProyectViews/Proyectos/CaseManagerComponent.js';
import { activityStyle } from '../style.js';
class SolicitudesPendientesComponent extends HTMLElement {
    /**
     * 
     * @param {Array<Tbl_Case_ModelComponent>} Dataset
     */
    constructor(Dataset) {
        super();
        this.Dataset = Dataset;
        //this.Dependencias = Dependencias;
        this.append(this.WStyle, StylesControlsV2.cloneNode(true), StylesControlsV3.cloneNode(true));
        this.TabContainer = WRender.createElement({ type: 'div', props: { class: "content-container", id: "TabContainer" } });
        this.TabManager = new ComponentsManager({ MainContainer: this.TabContainer });
        this.OptionContainer = WRender.Create({ className: "OptionContainer" });
        this.OptionContainer2 = WRender.Create({ className: "OptionContainer2" });
        this.ModelObject = new Tbl_Case_ModelComponent({
            Tbl_Tareas: {
                type: "text", hidden: true
            }, Estado: {
                type: "text", hidden: true
            }
        });
        this.filterD = [];
        this.nIntervId == null;
        this.DrawSolicitudesPendientesComponent();
    }
    connectedCallback() {
        if (this.nIntervId == null) {
            this.nIntervId = setInterval(this.update, 20000);
        }
    }
    disconnectedCallback() {
        if (this.nIntervId != null) {
            clearInterval(this.nIntervId);
            // liberar nuestro inervalId de la variable
            this.nIntervId = null;
        }
    }
    DrawSolicitudesPendientesComponent = async () => {
        this.OptionContainer.append(WRender.Create({ tagName: 'input', type: 'button', className: 'Block-Alert', value: 'Casos Pendientes de Aprobación', onclick: this.actividadesManager }))
        this.OptionContainer.append(WRender.Create({ tagName: 'input', type: 'button', className: 'Block-Success', value: 'Nuevo Proyecto', onclick: this.nuevoCaso }))
        this.UserActions.forEach(element => {
            this.OptionContainer2.append(WRender.Create({
                tagName: 'input', type: 'button', className: 'Btn',
                value: element.name, onclick: element.action
            }))

        });
        this.append(this.OptionContainer, this.TabContainer);
        this.actividadesManager();
    }

    actividadesManager = async () => {
        if (this.mainTable == undefined) {
            this.mainTable = new WTableComponent({
                Dataset: this.Dataset,
                AddItemsFromApi: false,
                ModelObject: this.ModelObject,
                Options: {
                    MultiSelect: true,
                    UserActions: [{
                        name: "ver detalles", action: async (element) => {
                            const find = await new Tbl_Case_ModelComponent({ Id_Case: element.Id_Case }).Get()
                            const CaseDetail = new CaseDetailComponent(find[0]);
                            this.TabManager.NavigateFunction("Detail" + element.Id_Case, CaseDetail)
                        }
                    }]
                }
            });
            this.FilterOptions = new WFilterOptions({
                Dataset: this.Dataset,
                ModelObject: this.ModelObject,
                UseEntityMethods: false,
                AutoSetDate: true,
                Display: true,
                FilterFunction: (DFilt) => {
                    this.filterD = DFilt;
                    this.update(this.filterD);
                }
            });
            this.mainTable.FilterOptions = this.FilterOptions;

            this.TabManager.NavigateFunction("Tab-Actividades-Manager",
                WRender.Create({
                    className: "actividadesView", children:
                        [this.FilterOptions, this.OptionContainer2, this.mainTable]
                }));
        } else {
            this.TabManager.NavigateFunction("Tab-Actividades-Manager");
        }

    }

    nuevoCaso = async () => {
        const form = new WForm({
            ModelObject: this.ModelObject
        })
        this.TabManager.NavigateFunction("Tab-nuevoCasoView",
            WRender.Create({ className: "nuevoCasoView", children: [form] }));
    }
    update = async (inst = this.filterD) => {
        const Solicitudes = await new Tbl_Case_ModelComponent({ FilterData: inst, OrderData: this.mainTable?.Sorts }).GetSolicitudesPendientesAprobar();
        this.mainTable?.DrawTable(Solicitudes);
    }

    actividadDetail = async (actividad) => {
        const actividadDetailView = WRender.Create({ className: "actividadDetailView", children: [this.actividadElementDetail(actividad)] });
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
    actividadElement = (actividad) => {
        this.append(priorityStyles.cloneNode(true));
        return WRender.Create({
            className: "actividad", object: actividad, children: [
                {
                    tagName: 'h4', innerText: `#${actividad.Id_Case} - ${actividad.Titulo} (${actividad.Tbl_Servicios?.Descripcion_Servicio ?? ""})`, children: [
                        {
                            className: "options", children: [
                                { tagName: 'button', className: 'Btn-Mini', innerText: "Detalle", onclick: async () => await this.actividadDetail(actividad) },
                                WSecurity.HavePermission(Permissions.ADMINISTRAR_CASOS_DEPENDENCIA) ? { tagName: 'button', className: 'Btn-Mini', innerText: 'Vincular Caso', onclick: () => this.Vincular(actividad) } : ""
                            ]
                        }
                    ]
                }, caseGeneralData(actividad)
            ]
        })
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
    UserActions = [
        {
            name: "Aprobar", action: async (/**@type {Tbl_Case_ModelComponent}*/element) => {
                // @ts-ignore
                if (this.mainTable.selectedItems.length <= 0) {
                    this.append(ModalMessege("Seleccione solicitudes"));
                    return;
                }
                const dependencias = await new Cat_Dependencias_ModelComponent().Get();
                // @ts-ignore
                const servicios = await new Tbl_Servicios_ModelComponent({ Id_Dependencia: this.mainTable.selectedItems[0]?.Cat_Dependencias?.Id_Dependencia }).Get();
                const modal = new WModalForm({
                    ObjectModal: simpleCaseForm(element,
                        // @ts-ignore
                        dependencias.filter(d => d.Id_Dependencia == this.mainTable.selectedItems[0]?.Cat_Dependencias?.Id_Dependencia),
                        servicios,
                        async (table_case) => {
                            const response = await new Tbl_Case_ModelComponent({})
                                .AprobarCaseList(this.mainTable?.selectedItems ?? [], table_case);
                            if (response.status == 200) {
                                this.append(ModalMessege("Solicitudes aprobadas"));
                                this.update();
                            } else {
                                this.append(ModalMessege("Error"));
                            }
                            modal.close();
                        })
                });
                this.append(modal);
            }
        }, {
            name: "Rechazar", action: async (/**@type {Tbl_Case_ModelComponent}*/element) => {
                // @ts-ignore
                if (this.mainTable.selectedItems.length <= 0) {
                    this.append(ModalMessege("Seleccione solicitudes"));
                    return;
                }
                this.append(new WModalForm({
                    title: "Escriba la razón por la cual se están rechazando estas solicitudes",
                    EditObject: {
                        Id_Case: element.Id_Case,
                    },
                    ModelObject: new Tbl_Comments_ModelComponent(),
                    ObjectOptions: {
                        SaveFunction: async (comentario) => {
                            this.append(ModalVericateAction(async () => {
                                const response = await new Tbl_Case_ModelComponent()
                                    .RechazarCaseList(this.mainTable?.selectedItems ?? [], comentario);
                                if (response.status == 200) {
                                    this.append(ModalMessege("Solicitudes rechazadas"));
                                    this.update();
                                } else {
                                    this.append(ModalMessege("Error"));
                                }
                                //modal.close();
                            }, "Esta seguro que desea rechazar estas solicitudes"));
                        }
                    }
                }))

            }
        }, {
            name: "Remitir a otra dependencia", action: async (/**@type {Tbl_Case_ModelComponent}*/element) => {
                // @ts-ignore
                if (this.mainTable.selectedItems.length <= 0) {
                    this.append(ModalMessege("Seleccione solicitudes"));
                    return;
                }
                const dependencias = await new Cat_Dependencias_ModelComponent().Get();
                // @ts-ignore
                const filterDepend = dependencias.filter(d => d.Id_Dependencia != this.mainTable.selectedItems[0]?.Cat_Dependencias?.Id_Dependencia)
                const servicios = await new Tbl_Servicios_ModelComponent({ Id_Dependencia: filterDepend[0]?.Id_Dependencia }).Get();
                const modal = new WModalForm({
                    ObjectModal: simpleCaseForm(element,
                        filterDepend,
                        servicios,
                        async (table_case) => {
                            this.append(ModalVericateAction(async () => {
                                const response =
                                    // @ts-ignore
                                    await new Tbl_Case_ModelComponent().RemitirCasos(this.mainTable.selectedItems,
                                        table_case.Cat_Dependencias, table_case.Tbl_Comments, table_case);
                                if (response.status == 200) {
                                    this.append(ModalMessege("Solicitud remitida"));
                                    this.update();
                                } else {
                                    this.append(ModalMessege("Error"));
                                }
                                modal.close();
                            }, "Esta seguro que desea remitir esta solicitud"))
                        })
                });
                this.append(modal);
            }
        }
    ]


    WStyle = activityStyle.cloneNode(true)

    mapCaseToPaginatorElement(Dataset) {
        return Dataset.map(actividad => {
            actividad.Dependencia = actividad.Cat_Dependencias.Descripcion;
            //actividad.Progreso = actividad.Tbl_Tareas?.filter(tarea => tarea.Estado?.includes("Finalizado")).length;
            return this.actividadElement(actividad);
        });
    }
}
customElements.define('w-main-solicitudes-component', SolicitudesPendientesComponent);
export { SolicitudesPendientesComponent };

