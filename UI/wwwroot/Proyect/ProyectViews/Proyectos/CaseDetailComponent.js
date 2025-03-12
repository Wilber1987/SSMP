
//@ts-check
import { priorityStyles } from '../../../AppComponents/Styles.js';
import { Permissions, WSecurity } from '../../../WDevCore/Security/WSecurity.js';
import { StylesControlsV2, StylesControlsV3 } from "../../../WDevCore/StyleModules/WStyleComponents.js";
import { WAppNavigator } from '../../../WDevCore/WComponents/WAppNavigator.js';
import { GanttChart } from '../../../WDevCore/WComponents/WChartJSComponents.js';
import { WCommentsComponent } from '../../../WDevCore/WComponents/WCommentsComponent.js';
import { WDetailObject } from '../../../WDevCore/WComponents/WDetailObject.js';
import { WForm } from "../../../WDevCore/WComponents/WForm.js";
import { WModalForm } from '../../../WDevCore/WComponents/WModalForm.js';
import { WTableComponent } from "../../../WDevCore/WComponents/WTableComponent.js";
import { ComponentsManager, html, WRender } from '../../../WDevCore/WModules/WComponentsTools.js';
import { ControlBuilder } from '../../../WDevCore/WModules/WControlBuilder.js';
import { css } from '../../../WDevCore/WModules/WStyledRender.js';
import { CaseOwModel } from '../../FrontModel/CaseOwModel.js';
import { Cat_Dependencias_ModelComponent } from "../../FrontModel/Cat_Dependencias.js";
import { ViewCalendarioByDependencia } from '../../FrontModel/DBOViewModel.js';
import { Tbl_Case, Tbl_Case_ModelComponent, Tbl_Dependencias_Usuarios, Tbl_VinculateCase } from '../../FrontModel/Tbl_CaseModule.js';
import { Tbl_Agenda_ModelComponent } from "../../FrontModel/Tbl_Agenda.js";
import { Tbl_Comments_ModelComponent } from '../../FrontModel/Tbl_Comments.js';
import { Tbl_Profile_CasosAsignados, Tbl_Profile_CasosAsignados_ModelComponent } from '../../FrontModel/Tbl_Profile_CasosAsignados.js';
import { Tbl_Servicios_ModelComponent } from '../../FrontModel/Tbl_Servicios.js';
import { Tbl_Tareas, Tbl_Tareas_ModelComponent } from "../../FrontModel/Tbl_Tareas.js";
import { activityStyle } from '../../style.js';
import { simpleCaseForm } from './CaseManagerComponent.js';
import { TaskManagers } from './TaskManager.js';
import { Tbl_Participantes_ModelComponent } from '../../FrontModel/Tbl_Participantes.js';
import { Tbl_Profile } from '../../FrontModel/Tbl_Profile.js';
// @ts-ignore
import { FilterData } from '../../../WDevCore/WModules/CommonModel.js';
import { Tbl_Grupo } from '../../FrontModel/Tbl_Grupo_ModelComponent.js';
import { WAjaxTools } from '../../../WDevCore/WModules/WAjaxTools.js';
import { ModalVericateAction } from '../../../WDevCore/WComponents/ModalVericateAction.js';
import { ModalMessage } from '../../../WDevCore/WComponents/ModalMessage.js';
import { DateTime } from '../../../WDevCore/WModules/Types/DateTime.js';

class CaseDetailComponent extends HTMLElement {

    /**
     * 
     * @param {Tbl_Case} Actividad
     */
    constructor(Actividad) {
        super();
        this.Actividad = Actividad;
        this.attachShadow({ mode: 'open' });
        this.shadowRoot?.append(this.WStyle, this.CustomStyle, StylesControlsV2.cloneNode(true), StylesControlsV3.cloneNode(true));
        this.TabContainer = WRender.createElement({ type: 'div', props: { class: "content-container", id: "TabContainer" } });
        this.TabManager = new ComponentsManager({ MainContainer: this.TabContainer });
        this.OptionContainer = WRender.Create({ className: "OptionContainer" });

        // this.OptionContainer.append(WRender.Create({
        //     tagName: 'a', className: 'Block-Alert', innerText: 'Lista de Casos', href: "/ProyectViews/Proyectos"
        // }))

        this.shadowRoot?.append(this.OptionContainer, this.TabContainer);
        this.DrawCaseDetailComponent(this.Actividad);
    }
    connectedCallback() { }

    DrawCaseDetailComponent = async (actividad = (new Tbl_Case())) => {
        actividad.MimeMessageCaseData.NewMessage = false;
        this.IsInBlackList = (await new Tbl_Case_ModelComponent().IsInBlackList(actividad)).body;

        new Tbl_Case({ Id_Case: actividad.Id_Case, MimeMessageCaseData: actividad.MimeMessageCaseData }).Update();
        


        const tareasActividad = await new Tbl_Tareas_ModelComponent({ Id_Case: actividad.Id_Case }).Get();
        actividad.Tbl_Tareas = tareasActividad;
        this.ganttChart = new GanttChart({ Dataset: tareasActividad ?? [], EvalValue: "date" });
        this.actividadDetailView = WRender.Create({ className: "actividadDetailView" });
        /**@type {Array<Tbl_Dependencias_Usuarios>} */
        const perfiles = await GetOwProfilesDependeciasGroup(actividad);

        const taskModel = this.BuildTaskModel(actividad, tareasActividad, perfiles);
        const taskContainer = WRender.Create({ className: "" });
        const tabManager = new ComponentsManager({ MainContainer: taskContainer });
        //const commentsActividad = await new Tbl_Comments_ModelComponent({ Id_Case: actividad.Id_Case }).Get();
        const commentsContainer = new WCommentsComponent({
            Dataset: [],
            ModelObject: new Tbl_Comments_ModelComponent(),
            User: WSecurity.UserData,
            UserIdProp: "Id_User",
            CommentsIdentify: actividad.Id_Case,
            CommentsIdentifyName: "Id_Case",
            UrlSearch: "../api/ApiEntityHelpdesk/getTbl_Comments",
            UrlAdd: "../api/ApiEntityHelpdesk/saveTbl_Comments",
            AddObject: actividad?.MimeMessageCaseData?.PlatformType != "WHATSAPP" && actividad?.MimeMessageCaseData?.PlatformType != "WEBAPI",
            UseDestinatarios: actividad?.MimeMessageCaseData?.PlatformType != "WHATSAPP" && actividad?.MimeMessageCaseData?.PlatformType != "WEBAPI",
            isRichTextActive: actividad?.MimeMessageCaseData?.PlatformType != "WHATSAPP"
        });
        this.taskManager = new TaskManagers(tareasActividad,
            taskModel, {
            ImageUrlPath: "", action: async (task) => {
                this.update();
            }
        })

        const taskNav = this.BuildNavigator(taskModel, actividad);
        const DetailContainer = html`<div class="case-admin-panel">
            ${this.actividadElementDetail(actividad)}
            ${taskNav}
        </div>`
        this.actividadDetailView.append(DetailContainer, commentsContainer)
        this.TabManager.NavigateFunction("Tab-Actividades-Viewer" + actividad.Id_Case, this.actividadDetailView);
    }

    actividadElement = (actividad) => {
        actividad.Progreso = actividad.Tbl_Tareas?.filter(tarea => tarea.Estado?.includes("Finalizado")).length;
        this.shadowRoot?.append(priorityStyles.cloneNode(true));
        return WRender.Create({
            className: "actividad", object: actividad, children: [
                {
                    tagName: 'h4', innerText: `#${actividad.Id_Case} - ${actividad.Titulo} (${actividad.Tbl_Servicios?.Descripcion_Servicio ?? ""})`, children: [
                        {
                            className: "options", children: [
                                WSecurity.HavePermission(Permissions.ADMINISTRAR_CASOS_DEPENDENCIA) ?
                                    { tagName: 'button', className: 'Btn-Mini', innerText: "Editar", onclick: async () => this.editCase() } : "",

                                actividad.Estado == "Finalizado" ?
                                    { tagName: 'button', className: 'Btn-Mini', innerText: 'Reabrir Caso', onclick: () => this.Reabrir(actividad) } :
                                    (
                                        actividad.Estado == "Solicitado" ?
                                            { tagName: 'button', className: 'Btn-Mini', innerText: 'Aprobar Caso', onclick: () => this.AprobarCaso(actividad) } :
                                            { tagName: 'button', className: 'Btn-Mini', innerText: 'Cerrar Caso', onclick: () => this.CerrarCaso(actividad) }

                                    ),
                                this.IsInBlackList != true ? { tagName: 'button', className: 'Btn-Mini', innerText: 'Agregar a lista negra', onclick: () => this.AddToBlackList(actividad) } : "",
                                this.IsInBlackList == true ? { tagName: 'button', className: 'Btn-Mini', innerText: 'Remover de lista negra', onclick: () => this.RemoveFromBlackList(actividad) } : ""

                            ]
                        }
                    ]
                },
                , caseGeneralData(actividad),
                //{ tagName: 'h4', innerText: "Progreso" },
                ControlBuilder.BuildProgressBar(actividad.Progreso,
                    actividad.Tbl_Tareas?.filter(tarea => !tarea.Estado?.includes("Inactivo"))?.length)
            ]
        })
    }

    editCase = async () => {
        this.shadowRoot?.append(new WModalForm({
            title: "Editar Caso",
            AutoSave: true,
            EditObject: this.Actividad,
            ModelObject: await CaseOwModel(this.Actividad.Id_Dependencia),
            ObjectOptions: {
                SaveFunction: () => {
                    this.update();
                }
            }
        }))
    }

    actividadElementDetail = (actividad) => {
        return WRender.Create({
            className: "actividadDetail", object: actividad, children: [
                this.actividadElement(actividad)
            ]
        })
    }

    Desvincular = async (actividad, table, modelVinculate) => {
        this.shadowRoot?.append(ModalVericateAction(async () => {
            const response = await new Tbl_VinculateCase({
                Casos_Vinculados: [actividad]
            }).DesvincularCaso();
            this.shadowRoot?.append(ModalMessage("Caso desvinculado exitosamente"));
            table.Dataset = await modelVinculate.GetOwCase();
            table.DrawTable();
        }, "¿Está seguro que desea desvincular este caso?"))

    }
    CerrarCaso = async (actividad) => {
        this.shadowRoot?.append(ModalVericateAction(async () => {
            const response = await new Tbl_Case_ModelComponent(actividad).CerrarCaso();
            if (response.status == 200) {
                this.shadowRoot?.append(ModalMessage("Caso cerrado exitosamente"));
                location.reload();
            } else {
                this.shadowRoot?.append(ModalMessage(response.message))
            }

        }, "¿Está seguro que desea cerrar este caso?"))

    }
    AprobarCaso = async (actividad) => {
        const dependencias = await new Cat_Dependencias_ModelComponent().Get();
        const servicios = await new Tbl_Servicios_ModelComponent({ Id_Dependencia: actividad?.Cat_Dependencias?.Id_Dependencia }).Get();
        console.log(dependencias.filter(d => d.Id_Dependencia == actividad?.Cat_Dependencias?.Id_Dependencia));
        console.log(actividad);
        const modal = new WModalForm({
            ObjectModal: simpleCaseForm(actividad,
                dependencias.filter(d => d.Id_Dependencia == actividad?.Cat_Dependencias?.Id_Dependencia),
                servicios,
                async (table_case) => {
                    const response = await new Tbl_Case_ModelComponent()
                        .AprobarCaseList([actividad], table_case);
                    if (response.status == 200) {
                        this.shadowRoot?.append(ModalMessage("Solicitud aprobada"));
                        actividad.Estado = "Activa";
                        this.update();
                    } else {
                        this.shadowRoot?.append(ModalMessage("Error"));
                    }
                    modal.close();
                })
        });
        this.shadowRoot?.append(modal);
    }
    Reabrir = async (actividad) => {
        this.shadowRoot?.append(ModalVericateAction(async () => {
            actividad.Estado = "Activo"
            const response = await new Tbl_Case_ModelComponent(actividad).Update();
            if (response.status == 200) {
                this.shadowRoot?.append(ModalMessage("Caso reabierto exitosamente"));
                location.reload();
            } else {
                this.append(ModalMessage(response.message))
            }
        }, "¿Está seguro que desea reabrir este caso?"))

    }
    AddToBlackList(actividad) {
        this.shadowRoot?.append(ModalVericateAction(async () => {
            actividad.Estado = "Activo"
            const response = await new Tbl_Case_ModelComponent().AddToBlackList(actividad);
            if (response.status == 200) {
                this.shadowRoot?.append(ModalMessage("Remitente enviado a lista negra"));
                location.reload();
            } else {
                this.append(ModalMessage(response.message))
            }
        }, "Este remitente será enviado a lista negra ¿Está seguro que desea ejecutar esta acción?"))
    }
    RemoveFromBlackList(actividad) {
        this.shadowRoot?.append(ModalVericateAction(async () => {
            actividad.Estado = "Activo"
            const response = await new Tbl_Case_ModelComponent().RemoveFromBlackList(actividad);
            if (response.status == 200) {
                this.shadowRoot?.append(ModalMessage("Remitente enviado a lista negra"));
                location.reload();
            } else {
                this.append(ModalMessage(response.message))
            }
        }, "Este remitente será enviado a lista negra ¿Está seguro que desea ejecutar esta acción?"))
    }
    update = async () => {
        const dataTask = await new Tbl_Tareas_ModelComponent({ Id_Case: this.Actividad.Id_Case }).Get();
        //console.log(dataTask);
        // @ts-ignore
        this.ganttChart.Dataset = dataTask;
        this.Actividad.Tbl_Tareas = dataTask;
        //this.tasktable.Dataset = dataTask;
        // @ts-ignore
        this.taskManager.Dataset = dataTask;
        this.Actividad.Progreso = this.Actividad.Tbl_Tareas?.filter(tarea => tarea.Estado?.includes("Finalizado")).length;
        // @ts-ignore
        this.actividadDetailView.querySelector(".actividadDetail").innerHTML = "";
        // @ts-ignore
        this.actividadDetailView.querySelector(".actividadDetail").append(this.actividadElementDetail(this.Actividad));
        //this.tasktable.DrawTable();
        // @ts-ignore
        this.taskManager.DrawTaskManagers();
        // @ts-ignore
        this.ganttChart.DrawComponent();
        // @ts-ignore
        this.ganttChart.Animate();
    }

    WStyle = activityStyle.cloneNode(true)
    CustomStyle = css`
        w-coment-component {
            max-height: 700px;
        }
       
       .actividadDetailView {
            display: grid;
            grid-template-columns: calc(100% - 820px) 800px;
            gap: 20px;
        }
        .case-admin-panel {
            overflow-y: auto;
            overflow-x: hidden;
            max-height: 740px;
        }
        w-app-navigator{
            min-height: 480px;
        }
        w-app-navigator, w-coment-component  {
            border: 1px solid var(--fifty-color);
            padding: 20px;
            border-radius: 15px;           
            background-color: var(--secundary-color);
        }

        @media(max-width: 1400px){
            .actividadDetailView {
                display: grid;
                grid-template-columns: calc(100% - 20px);
                grid-template-rows: auto  auto;
                gap: 0px 20px;
            }
            w-coment-component {
                grid-row: span 3;
                position: fixed;
                z-index: 1000;
                width: 700px;
                background-color: var(--secundary-color);
                bottom: 0;
                right: 10px;
                display: block;
                padding: 10px;
                border: solid 1px  var(--fifty-color);
                height: 80vh;
                box-shadow: 0 0 5px 0  var(--fifty-color);
                max-height:20px;
                transition: all 0.5s;
            }
            w-coment-component::before{
                content: "comentarios";
                height: 30px;
                display: block;                
                cursor: pointer;
            } 
            w-coment-component:hover {
                max-height: 100vh;
            }
        }      
    `

    BuildNavigator(taskModel, actividad) {
        return new WAppNavigator({
            NavStyle: "tab",
            Inicialize: true,
            Elements: [{
                name: "Vista de panel", action: async (ev) => {
                    //tabManager.NavigateFunction("taskManager", this.taskManager)
                    return this.taskManager;
                }
            }, {
                name: "Vista de progreso", action: async (ev) => {
                    //tabManager.NavigateFunction("ganttChart", this.ganttChart)
                    return this.ganttChart;
                }
            },
            {
                name: "Detalles del caso", action: async (ev) => {
                    this.Actividad.Tbl_Profile_CasosAsignados = await new Tbl_Profile_CasosAsignados({ Id_Case: this.Actividad.Id_Case }).Get();
                    //tabManager.NavigateFunction("detalles", )
                    return new WDetailObject({
                        ModelObject: new Tbl_Case_ModelComponent({
                            // @ts-ignore
                            Tbl_Profile_CasosAsignados: {
                                type: "MASTERDETAIL",
                                label: "Participantes",
                                ModelObject: new Tbl_Profile_CasosAsignados_ModelComponent()
                            }
                        }),
                        ObjectDetail: this.Actividad
                    });
                }
            }, {
                name: "Nueva Tarea", Disabled: WSecurity.HavePermission(Permissions.GESTOR_TAREAS), action: async (ev) => {
                    //this.shadowRoot?.append()
                    return new WForm({
                        ModelObject: taskModel,
                        AutoSave: true,
                        //title: "Nueva Tarea",
                        SaveFunction: (task, response, /**@type {WForm} */ form) => {
                            form.FormObject = new Tbl_Tareas();
                            form.DrawComponent();
                            this.update();
                        }
                    });
                }
            }, {
                name: "Vinculaciones", action: async (ev) => {
                    // @ts-ignore
                    const modelVinculate = new Tbl_Case_ModelComponent({ Id_Vinculate: actividad.Id_Vinculate });
                    const vinculateTable = new WTableComponent({
                        Dataset: await modelVinculate.GetVinculateCase(),
                        ModelObject: new Tbl_Case_ModelComponent(),
                        AddItemsFromApi: false,
                        Options: {
                            UserActions: WSecurity.HavePermission(Permissions.ADMINISTRAR_CASOS_DEPENDENCIA) ? [{
                                name: "Desvincular caso", action: (caso) => {
                                    this.Desvincular(caso, vinculateTable, modelVinculate);
                                }
                            }] : undefined
                        }
                    });
                    //tabManager.NavigateFunction("vinculaciones", vinculateTable)
                    return vinculateTable;
                }
            }
            ]
        });
    }

    BuildTaskModel(actividad, tareasActividad, perfiles) {
        return new Tbl_Tareas_ModelComponent({
            Id_Case: { type: 'number', hidden: true, value: actividad.Id_Case },
            Tbl_Tarea: {
                type: 'WSelect', label: "Tarea principal", require: false, hiddenInTable: true,
                Dataset: tareasActividad, ModelObject: () => new Tbl_Tareas_ModelComponent()
            },
            Tbl_Calendario: {
                type: 'CALENDAR', require: false, CalendarFunction: async () => {
                    return {
                        Agenda: await new Tbl_Agenda_ModelComponent({ Id_Dependencia: actividad.Cat_Dependencias.Id_Dependencia }).Get(),
                        Calendario: await new ViewCalendarioByDependencia({ Id_Dependencia: actividad.Cat_Dependencias.Id_Dependencia }).Get()
                    };
                }
            },
            Tbl_Participantes: {
                type: 'MasterDetail',
                require: false,
                ModelObject: () => new Tbl_Participantes_ModelComponent({
                    Tbl_Profile: {
                        type: 'WSelect', hiddenFilter: true, ModelObject: () => new Tbl_Profile(), Dataset: perfiles
                    }
                })
            }
        });
    }
}
customElements.define('w-case-detail', CaseDetailComponent);
export { CaseDetailComponent };
const caseGeneralData = (actividad) => {
    return {
        className: "propiedades", children: [
            { tagName: 'label', innerText: "Dependencia: " + (actividad.Cat_Dependencias?.Descripcion ?? "-") },
            { tagName: 'label', innerText: "Estado: " + actividad.Estado },
            {
                tagName: 'label', className: "prioridad_" + (actividad.Case_Priority != null ? actividad.Case_Priority : undefined),
                innerText: "Prioridad: " + (actividad.Case_Priority != null ? actividad.Case_Priority ?? "indefinida" : "indefinida")
            },
            {
                tagName: 'label', innerText: "Fecha inicio: " + (
                    new DateTime(actividad.Fecha_Inicio).toDDMMYYYY() ?? "")
            },
            {
                tagName: 'label', className: "platform_" + actividad?.MimeMessageCaseData?.PlatformType
            }, {
                tagName: 'label', className: "newActivity_" + actividad?.MimeMessageCaseData?.NewMessage?.toString()
            },
            //{ tagName: 'label', innerText: "Fecha de finalización: " + (actividad.Fecha_Final?.toString().toDateFormatEs() ?? "") },
        ]
    };
}

export { caseGeneralData };
/**
 * @param {Tbl_Case} actividad
 */
async function GetOwProfilesDependeciasGroup(actividad) {
    const perfiles_dependencias = await new Tbl_Dependencias_Usuarios({
        filterData: [
            new FilterData({ PropName: "Id_Dependencia", Values: [actividad.Id_Dependencia?.toString()], FilterType: "=" })
        ]
    }).Get();

    const response = await WAjaxTools.PostRequest("../../api/Profile/TakeProfile");
    let misGrupos = [];
    if (response.Tbl_Grupos_Profiles?.length > 0) {
        misGrupos = await new Tbl_Grupo({
            FilterData: [new FilterData({
                PropName: "Id_Grupo",
                Values: response.Tbl_Grupos_Profiles?.map(p => p.Id_Grupo.toString()),
                FilterType: "in",
            })]
        }).Get();
    }
    const perfiles = [];
    perfiles_dependencias.forEach(p => {
        perfiles.push(p.Tbl_Profile);
    });

    misGrupos.flatMap(p => p.Tbl_Grupos_Profiles).forEach(p => {

        if (perfiles.find(pp => pp.Id_Perfil == p.Id_Perfil) != null) {
            return;
        }
        perfiles.push(p.Tbl_Profile);
    });
    return perfiles;
}

