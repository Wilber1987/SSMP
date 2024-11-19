//@ts-check
import { StyleScrolls, StylesControlsV2, StylesControlsV3 } from "../../../WDevCore/StyleModules/WStyleComponents.js";
import { WModalForm } from '../../../WDevCore/WComponents/WModalForm.js';
import { WToolTip } from '../../../WDevCore/WComponents/WMultiSelect.js';
import { ComponentsManager, WRender } from '../../../WDevCore/WModules/WComponentsTools.js';
import { css } from '../../../WDevCore/WModules/WStyledRender.js';
import { TareaDetailView } from './TareaDetailView.js';
import { Tbl_Tareas_ModelComponent } from "../../FrontModel/Tbl_Tareas.js";
import { Tbl_Agenda_ModelComponent } from "../../FrontModel/Tbl_Agenda.js";
import { Tbl_Calendario_ModelComponent } from "../../FrontModel/Tbl_Calendario.js";
import { WSecurity, Permissions } from "../../../WDevCore/Security/WSecurity.js";

class TaskManagers extends HTMLElement {
    /**
     * @param {Array<Tbl_Tareas_ModelComponent>} Dataset
     * @param {Tbl_Tareas_ModelComponent} Model
     * @param {Object} [Config] 
     */
    constructor(Dataset, Model, Config) {
        super();
        this.Dataset = Dataset;
        this.Config = Config;
        this.TaskModel = Model ?? new Tbl_Tareas_ModelComponent();

        this.append(this.WStyle,
            StyleScrolls.cloneNode(true),
            StylesControlsV2.cloneNode(true),
            StylesControlsV3.cloneNode(true));
        this.TaskContainer = WRender.Create({ class: 'TaskContainer', id: "TaskContainer" });
        this.TabManager = new ComponentsManager({ MainContainer: this.TaskContainer });
        this.OptionContainer = WRender.Create({ className: "OptionContainer" });
        this.append(this.TaskContainer);
        this.StatePanelContainer = WRender.Create({
            className: "panelContainer",
            style: " grid-template-columns: repeat(" + this.TaskModel.Estado.Dataset.length + ", auto);"
        })
        this.DrawTaskManagers();
    }
    connectedCallback() { }
    DrawTaskManagers = async () => {
        this.StatePanelContainer.innerHTML = "";
        this.TaskManager(this.Dataset);
    }
    /**
     * 
     * @param {Array<Object>} DatasetData 
     */
    TaskManager = (DatasetData = this.Dataset) => {
        this.TaskModel.Estado.Dataset.forEach(state => {
            const Dataset = DatasetData.filter(t => t.Estado == state);
            const Panel = WRender.Create({
                className: Dataset.length > 0 ? "panel" : "panel-inact", id: "Panel-" + state,
                ondrop: (ev) => {
                    var data = ev.dataTransfer.getData("text");
                    const taskCard = this.querySelector("#" + data);
                    if (ev.target.className.includes("panel")) {
                        ev.target.appendChild(taskCard);
                        // @ts-ignore
                        taskCard.task.Estado = state;
                        // @ts-ignore
                        taskCard.querySelector(".tag").style.backgroundColor = GetTaskColor(taskCard.task);
                        // @ts-ignore
                        this.cardDrop(taskCard?.task, state);
                    }
                }, ondragover: (ev) => {
                    ev.preventDefault();
                }, children: [
                    { tagName: "label", class: "title-panel", innerText: state }
                ]
            });
            Dataset.forEach(task => {
                Panel.append(this.taskCard(task));
            });
            const panelOptions = WRender.Create({
                className: "panel-options", children: [
                    {//display
                        tagName: 'input', style: 'transform: rotate(90deg)', type: "button", class: 'BtnDinamictT', value: '>', onclick: async (ev) => {
                            if (Panel.className == "panel") {
                                ev.target.style["transform"] = "inherit";
                                Panel.className = "panel-inact";
                            } else {
                                ev.target.style["transform"] = "rotate(90deg)";
                                Panel.className = "panel";
                            }
                        }

                    }]
            });
            this.StatePanelContainer.appendChild(WRender.Create({
                className: "panel-container",
                children: [panelOptions, Panel]
            }));
        });
        this.TabManager.NavigateFunction("main-task", this.StatePanelContainer);
    }
    taskCard = (task) => {
        const card = TaskCard(task, this.TabManager);
        card.draggable = true;
        card.id = "task" + task.Id_Tarea;
        card.className += " task-card"
        // @ts-ignore
        card.task = task;
        // @ts-ignore
        card.querySelector(".tag").style.backgroundColor = GetTaskColor(task);
        card.ondragstart = (ev) => {
            // @ts-ignore
            ev.dataTransfer?.setData("text", ev.target?.id);

        };

        return card
    }

    /**
     * @param {Tbl_Tareas_ModelComponent} task
     * @param {String} state 
     */
    cardDrop = async (task, state) => {
        const response = await task.Update();
        if (response.status == 200 && this.Config?.action != undefined) {
            this.Config.action(task);
        }
    }
    WStyle = css`
        w-main-task {
            display: flex !important;
            height: 100% !important;
            width: 100% !important;
        }
        .task-container {
            height: -webkit-fill-available;
            box-sizing: border-box;
            height: calc(100vh - 100px);
            display: grid;
            grid-template-rows: auto calc(100% );
        }

        .TaskContainer {
            height: -webkit-fill-available;
            width: 100%;
        }

        w-main-task {
            display: block;
            height: -webkit-fill-available;
        }

        .dashBoardView {
            display: grid;
            grid-template-columns: auto auto;
            grid-gap: 20px
        }

        .OptionContainer {
            margin: 0 0 20px 0;
        }

        .panelContainer {
            display: flex;
            overflow-x: auto;
            overflow-y: hidden;
            padding: 10px;
            gap: 10px;
            width: fit-content;
            height: 100%;
            max-width: calc(100% - 50px);
        }


        .panel-container {
            padding: 0px;
            border-radius: 0px 10px 10px 0px;
            border-left: 1px solid var(--fifty-color);
            display: grid;
            grid-template-columns: 30px fit-content(360px);
            width: fit-content;
            min-width: fit-content;
        }

        .BtnDinamictT {
            font-weight: bold;
            border: none;
            padding: 0px;
            margin: 5px;
            outline: none;
            text-align: center;
            display: inline-block;
            font-size: 10px;
            cursor: pointer;
            background-color: #4894aa;
            color: #fff;
            border-radius: 0.2cm;
            width: 25px;
            height: 25px;
            background-color: #4894aa;
            font-family: monospace;
        }

        .panel {
            padding: 5px;
            transition: all 0.4s;
            overflow-y: auto;
            padding-bottom: 20px;
        }

        .panel-inact {
            padding: 5px;
            overflow: hidden;
            width: calc(80px);
            font-size: 12px;
            transition: all 0.4s;
        }

        .task-card {
            background-color: var(--secundary-color);
            height: 130px;
            border-radius: 10px;
            padding-bottom: 10px;
            display: flex;
            flex-direction: column;
            overflow: hidden;
            margin-bottom: 15px;
            /* container-type: inline-size; */
        }

        .task-title {
            padding: 10px 10px;
            font-size: 12px;
            font-weight: bold;
            background-color: var(--secundary-color);
            cursor: pointer;
        }

        .card-options {
            padding: 0px;
            width: calc(100% - 20px);
            height: 25px;
            justify-content: flex-end;
            display: flex;
        }

        .title-panel {
            font-size: 12px;
            text-transform: uppercase;
            font-weight: bold;
            margin-bottom: 10px;
            display: block;
            color: var(--font-secundary-color);
        }

        .p-title {
            height: 100%;
            padding: 5px 10px;
            margin: 0px;
        }

        .task-detail {
            padding: 5px 10px;
            font-size: 11px
        }

        .p-participantes {
            display: flex;
            padding: 5px 10px;
        }

        .img-participantes {
            padding: 0;
            height: 25px;
            width: 25px;
            border-radius: 50%;
            margin-right: 5px;
            overflow: hidden;
        }`
}
customElements.define('w-main-task', TaskManagers);
export { TaskManagers };


/**
* @param {Tbl_Tareas_ModelComponent} element
* @param {ComponentsManager} Manager
*/
const TaskCard = (element, Manager) => {
    const toolTip = new WToolTip(WRender.Create({
        className: 'option-btn-container',
        children: WSecurity.HavePermission(Permissions.GESTOR_TAREAS) ? [
            {
                tagName: 'input', type: 'button', className: 'option-btn', value: 'editar', onclick: async () => {
                    taskEdit(element)
                }
            }
        ] : []
    }));
    const card = WRender.Create({
        className: "task", children: [
            {
                className: "tags", children: [
                    { tagName: "span", className: "tag", innerHTML: "Tarea de: " + element.Tbl_Case.Titulo },
                    {
                        tagName: 'button', className: 'options', children: [WRender.CreateStringNode(`<div><svg xml:space="preserve" viewBox="0 0 41.915 41.916" xmlns:xlink="http://www.w3.org/1999/xlink" xmlns="http://www.w3.org/2000/svg" id="Capa_1" version="1.1" fill="#000000"><g stroke-width="0" id="SVGRepo_bgCarrier"></g><g stroke-linejoin="round" stroke-linecap="round" id="SVGRepo_tracerCarrier"></g><g id="SVGRepo_iconCarrier"><g><g><path d="M11.214,20.956c0,3.091-2.509,5.589-5.607,5.589C2.51,26.544,0,24.046,0,20.956c0-3.082,2.511-5.585,5.607-5.585 C8.705,15.371,11.214,17.874,11.214,20.956z"></path> <path d="M26.564,20.956c0,3.091-2.509,5.589-5.606,5.589c-3.097,0-5.607-2.498-5.607-5.589c0-3.082,2.511-5.585,5.607-5.585 C24.056,15.371,26.564,17.874,26.564,20.956z"></path> <path d="M41.915,20.956c0,3.091-2.509,5.589-5.607,5.589c-3.097,0-5.606-2.498-5.606-5.589c0-3.082,2.511-5.585,5.606-5.585 C39.406,15.371,41.915,17.874,41.915,20.956z"></path></g></g></g></svg></div>`)],
                        onclick: async (ev) => {
                            console.log(true);
                            //code.....
                            //if (!card.querySelector("w-tooltip")) {
                            //    card.append(toolTip)
                            // }
                            //  else {
                            toolTip.DisplayOptions(card);
                            //  }                            
                        }
                    }
                ]
            }, {
                tagName: "label", className: "labelheader", innerHTML: element.Titulo
            }, {
                tagName: "p", className: "", innerHTML: element.Descripcion
            }, {
                className: "stats", children: [
                    [
                        WRender.CreateStringNode(`<div><svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24"><g stroke-width="0" id="SVGRepo_bgCarrier"></g><g stroke-linejoin="round" stroke-linecap="round" id="SVGRepo_tracerCarrier"></g><g id="SVGRepo_iconCarrier"> <path stroke-linecap="round" stroke-width="2" d="M12 8V12L15 15"></path> <circle stroke-width="2" r="9" cy="12" cx="12"></circle> </g></svg>
                            ${// @ts-ignore
                            new Date(element.Fecha_Inicio).toLocaleDateString()}</div>`),
                        //WRender.CreateStringNode(`<div><svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24"><g stroke-width="0" id="SVGRepo_bgCarrier"></g><g stroke-linejoin="round" stroke-linecap="round" id="SVGRepo_tracerCarrier"></g><g id="SVGRepo_iconCarrier"> <path stroke-linejoin="round" stroke-linecap="round" stroke-width="1.5" d="M16 10H16.01M12 10H12.01M8 10H8.01M3 10C3 4.64706 5.11765 3 12 3C18.8824 3 21 4.64706 21 10C21 15.3529 18.8824 17 12 17C11.6592 17 11.3301 16.996 11.0124 16.9876L7 21V16.4939C4.0328 15.6692 3 13.7383 3 10Z"></path> </g></svg>18</div>`),
                        //WRender.CreateStringNode(`<div><svg fill="#000000" xmlns:xlink="http://www.w3.org/1999/xlink" xmlns="http://www.w3.org/2000/svg" version="1.1" viewBox="-2.5 0 32 32"><g stroke-width="0" id="SVGRepo_bgCarrier"></g><g stroke-linejoin="round" stroke-linecap="round" id="SVGRepo_tracerCarrier"></g><g id="SVGRepo_iconCarrier"> <g id="icomoon-ignore"> </g> <path fill="#000000" d="M0 10.284l0.505 0.36c0.089 0.064 0.92 0.621 2.604 0.621 0.27 0 0.55-0.015 0.836-0.044 3.752 4.346 6.411 7.472 7.060 8.299-1.227 2.735-1.42 5.808-0.537 8.686l0.256 0.834 7.63-7.631 8.309 8.309 0.742-0.742-8.309-8.309 7.631-7.631-0.834-0.255c-2.829-0.868-5.986-0.672-8.686 0.537-0.825-0.648-3.942-3.3-8.28-7.044 0.11-0.669 0.23-2.183-0.575-3.441l-0.352-0.549-8.001 8.001zM1.729 10.039l6.032-6.033c0.385 1.122 0.090 2.319 0.086 2.334l-0.080 0.314 0.245 0.214c7.409 6.398 8.631 7.39 8.992 7.546l-0.002 0.006 0.195 0.058 0.185-0.087c2.257-1.079 4.903-1.378 7.343-0.836l-13.482 13.481c-0.55-2.47-0.262-5.045 0.837-7.342l0.104-0.218-0.098-0.221-0.031 0.013c-0.322-0.632-1.831-2.38-7.498-8.944l-0.185-0.215-0.282 0.038c-0.338 0.045-0.668 0.069-0.981 0.069-0.595 0-1.053-0.083-1.38-0.176z"> </path> </g></svg>7</div>`),
                        {
                            className: "viewer", children: element?.Tbl_Participantes?.map(I =>
                            ({
                                tagName: 'img', className: "img-participantes",
                                title: I.Tbl_Profile?.Nombres + " " + I.Tbl_Profile?.Apellidos,
                                src: "" + I.Tbl_Profile?.Foto
                            })) ?? []// [
                            // WRender.CreateStringNode(`<span><svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24"><g stroke-width="0" id="SVGRepo_bgCarrier"></g><g stroke-linejoin="round" stroke-linecap="round" id="SVGRepo_tracerCarrier"></g><g id="SVGRepo_iconCarrier"> <path stroke-width="2" stroke="#ffffff" d="M17 8C17 10.7614 14.7614 13 12 13C9.23858 13 7 10.7614 7 8C7 5.23858 9.23858 3 12 3C14.7614 3 17 5.23858 17 8Z"></path> <path stroke-linecap="round" stroke-width="2" stroke="#ffffff" d="M3 21C3.95728 17.9237 6.41998 17 12 17C17.58 17 20.0427 17.9237 21 21"></path> </g></svg>`),
                            // WRender.CreateStringNode(`<span><svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24"><g stroke-width="0" id="SVGRepo_bgCarrier"></g><g stroke-linejoin="round" stroke-linecap="round" id="SVGRepo_tracerCarrier"></g><g id="SVGRepo_iconCarrier"> <path stroke-width="2" stroke="#ffffff" d="M17 8C17 10.7614 14.7614 13 12 13C9.23858 13 7 10.7614 7 8C7 5.23858 9.23858 3 12 3C14.7614 3 17 5.23858 17 8Z"></path> <path stroke-linecap="round" stroke-width="2" stroke="#ffffff" d="M3 21C3.95728 17.9237 6.41998 17 12 17C17.58 17 20.0427 17.9237 21 21"></path> </g></svg>`),
                            // WRender.CreateStringNode(`<span><svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24"><g stroke-width="0" id="SVGRepo_bgCarrier"></g><g stroke-linejoin="round" stroke-linecap="round" id="SVGRepo_tracerCarrier"></g><g id="SVGRepo_iconCarrier"> <path stroke-width="2" stroke="#ffffff" d="M17 8C17 10.7614 14.7614 13 12 13C9.23858 13 7 10.7614 7 8C7 5.23858 9.23858 3 12 3C14.7614 3 17 5.23858 17 8Z"></path> <path stroke-linecap="round" stroke-width="2" stroke="#ffffff" d="M3 21C3.95728 17.9237 6.41998 17 12 17C17.58 17 20.0427 17.9237 21 21"></path> </g></svg>`)
                            //]
                        }, {
                            tagName: "a", innerHTML: "Ver detalles", onclick: async () => {
                                const find = await new Tbl_Tareas_ModelComponent({ Id_Tarea: element.Id_Tarea }).Get()
                                const CaseDetail = new TareaDetailView({
                                    Task: find[0],
                                    // BackAction: () => {  Manager.NavigateFunction("main-task")}
                                });
                                document.body.appendChild(new WModalForm({ ObjectModal: CaseDetail }));
                                //Manager.NavigateFunction("Detail" + element.Id_Tarea, CaseDetail)
                            }
                        }
                    ]
                ]
            }, css`
            .task {
                position: relative;
                color: var(--font-primary-color);
                cursor: move;
                background-color: var(--secundary-color);
                padding: 1rem;
                border-radius: 8px;
                margin-bottom: 1rem;
                border: 3px dashed transparent;
                min-width: 290px;
                border: solid var(--fourth-color) 1px;
                min-height: 150px;
                height: 150px;
                display: grid;
                grid-template-rows: 30px 20px 60px 40px;
              }
              .labelheader {
                margin: 10px 0px;
                display: block;
                font-size: 12px;
                font-weight: 600;
                text-transform: capitalize;
                color: var(--font-secundary-color);
              }
              
              .task:hover {
                border-color: var(--fifty-color) !important;
              }
              
              .task p {
                font-size: 13px;
                margin: 10px 0;
                text-overflow: ellipsis;
                white-space: nowrap;
                overflow: hidden;
              }
              
              .tag {
                border-radius: 100px;
                padding: 4px 13px;
                font-size: 12px;
                color: var(--font-primary-color);
                background-color: ${GetTaskColor(element)};
                display: -webkit-box;
                -webkit-line-clamp: 2; /* Limitar a 2 líneas */
                -webkit-box-orient: vertical;
                overflow: hidden;
                text-overflow: ellipsis;
                max-width: 300px;
              }
              
              .tags {
                width: 100%;
                display: flex;
                align-items: center;
                justify-content: space-between;
              }
              
              .options {
                background: transparent;
                border: 0;
                color: var(--font-primary-color);
                font-size: 17px;
                cursor:pointer;
              }
              w-tooltip {
                left: auto !important;
                right: 10px !important;
                top: 35px !important;
                width: 100px !important;
              }
              
              .options svg {
                fill: var(--font-secundary-color);
                width: 20px;
              }
              .option-btn-container{ 
                border-radius: 10px;
                display:flex;
                width: 100px;
                flex-direction: column;
                gap: 5px;
              }
              .option-btn {
                background: transparent;
                border: 0;
                color: var(--font-primary-color);
                font-size: 17px;
                cursor:pointer;
                transition: all 0.5s;
              }
              .option-btn:hover {
                color: var(--font-secundary-color);
              }
              
              .stats {
                position: relative;
                width: 100%;
                color: var(--font-primary-color);
                font-size: 12px;
                display: flex;
                align-items: center;
                justify-content: space-between;                   
              }
              .viewer {
                justify-content: flex-end;
              }
              .stats div {
                margin-right: 1rem;
                height: 20px;
                display: flex;
                align-items: center;
                cursor: pointer;
                width: 100%;
              }
              
              .stats svg {
                margin-right: 5px;
                height: 100%;
                stroke: #9fa4aa;
              }
              
              .viewer .img-participantes {
                height: 30px;
                width: 30px;
                background-color: rgb(28, 117, 219);
                margin-right: -10px;
                border-radius: 50%;
                border: 1px solid var(--fifty-color);
                display: grid;
                align-items: center;
                text-align: center;
                font-weight: bold;
                color: var(--font-primary-color);
                padding: 2px;
              }
              
              .viewer span svg {
                stroke: var(--font-primary-color);;
              } `
        ]
    });
    /**
    * @param {Tbl_Tareas_ModelComponent} task
    */
    const taskEdit = async (task) => {
        const CalendarModel = {
            type: 'CALENDAR',
            ModelObject: () => new Tbl_Calendario_ModelComponent(),
            require: false,
            CalendarFunction: async () => {
                return {
                    Agenda: await new Tbl_Agenda_ModelComponent({
                        // @ts-ignore
                        Id_Dependencia: task?.Tbl_Case?.Id_Dependencia
                    }).Get(),
                    Calendario: await new Tbl_Calendario_ModelComponent({
                        // @ts-ignore
                        Id_Dependencia: task?.Tbl_Case?.Id_Dependencia
                    }).Get()
                }
            }
        }
        card.append(new WModalForm({
            title: "Editar tarea",
            EditObject: task,
            //ImageUrlPath: this.Config?.ImageUrlPath,
            AutoSave: true,
            ModelObject: new Tbl_Tareas_ModelComponent({ Tbl_Calendario: CalendarModel })
        }))
    }
    return card;
}

export { TaskCard }

/**
 * @param {Tbl_Tareas_ModelComponent} element
 */
function GetTaskColor(element) {
    switch (element.Estado.toString().toUpperCase()) {
        case "ACTIVO":
            return "#1389eb"
        case "PROCESO":
            return "#48af05"
        case "FINALIZADO":
            return "#fd7326"
        case "ESPERA":
            return "#c13109"
        case "INACTIVO":
            return "#999999"
        default:
            return "#737373"
    }
}
