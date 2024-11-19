//@ts-check
import { WSecurity, Permissions } from "../../../WDevCore/Security/WSecurity.js";
import { StylesControlsV2, StylesControlsV3, StyleScrolls } from "../../../WDevCore/StyleModules/WStyleComponents.js";
import { WCommentsComponent } from "../../../WDevCore/WComponents/WCommentsComponent.js";
import { WModalForm } from "../../../WDevCore/WComponents/WModalForm.js";
import { DateTime } from "../../../WDevCore/WModules/Types/DateTime.js";
import { ComponentsManager, html, WRender } from "../../../WDevCore/WModules/WComponentsTools.js";
import { css } from "../../../WDevCore/WModules/WStyledRender.js";
import { Tbl_Agenda_ModelComponent } from "../../FrontModel/Tbl_Agenda.js";
import { Tbl_Calendario_ModelComponent } from "../../FrontModel/Tbl_Calendario.js";
import { Tbl_Comments_Tasks_ModelComponent } from "../../FrontModel/Tbl_Comments_Tasks.js";
import { Tbl_Tareas, Tbl_Tareas_ModelComponent } from "../../FrontModel/Tbl_Tareas.js";

/**
 * @typedef {Object} ComponentConfig
 * * @property {Tbl_Tareas} Task
 * * @property {Function} [action]
 * * @property {Function} [BackAction]
 */
class TareaDetailView extends HTMLElement {
    /**
     * @param {ComponentConfig} Config 
     */
    constructor(Config) {
        super();
        this.OptionContainer = WRender.Create({ className: "OptionContainer" });
        this.TabContainer = WRender.Create({ className: "TabContainer", id: 'TabContainer' });
        this.Manager = new ComponentsManager({ MainContainer: this.TabContainer, SPAManage: false });
        this.Task = Config.Task;
        this.Config = Config;
        this.append(
            StylesControlsV2.cloneNode(true),
            StyleScrolls.cloneNode(true),
            StylesControlsV3.cloneNode(true),
            this.CustomStyle,
            //this.OptionContainer,
            this.TabContainer
        );
        this.Draw();
    }
    Draw = async () => {
        this.SetOption();
    }

    async SetOption() {
        this.OptionContainer.append(WRender.Create({
            tagName: 'button', className: 'Block-Primary', innerText: 'Home',
            onclick: async () => this.Manager.NavigateFunction("id", await this.MainComponent())
        }))
        this.Manager.NavigateFunction("id", await this.MainComponent());
    }
    async MainComponent() {
        const commentsActividad = await new Tbl_Comments_Tasks_ModelComponent({ Id_Tarea: this.Task?.Id_Tarea }).Get();
        const commentsContainer = new WCommentsComponent({
            Dataset: commentsActividad,
            ModelObject: new Tbl_Comments_Tasks_ModelComponent(),
            User: WSecurity.UserData,
            UserIdProp: "Id_User",
            CommentsIdentify: this.Task?.Id_Tarea,
            CommentsIdentifyName: "Id_Tarea",
            UseDestinatarios: false,
            UrlSearch: "../api/ApiEntityHelpdesk/getTbl_Comments_Tasks",
            UrlAdd: "../api/ApiEntityHelpdesk/saveTbl_Comments_Tasks"
        });
        const column = html`<div class="column"></div>`;

        const node = html`<div class="task-contaier">
            <h2 class="title">${this.Task?.Titulo} (${this.Task.Estado})</h2>
            <div class="right"> 
                <p>${this.Task?.Descripcion}</p>                
                <div class="authores">
                    ${this.Task?.Tbl_Participantes?.map(I => `<div class="author"><img src="${I.Tbl_Profile?.Foto}" title="${I.Tbl_Profile?.Nombres} ${I.Tbl_Profile?.Apellidos}"/></div>`)?.join("") ?? "Sin Participantes"}
                </div>
                <div class="horarios">
                    <h5>Horario</h5>
                    ${this.BuildDatesDetail()}
                </div>
                <hr />
                ${commentsContainer}
            </div>               
            ${column}
        </div>`
        if (this.Config.BackAction) {
            column.append(WRender.Create({
                tagName: 'input', type: 'button', className: 'BtnSuccess BtnReturn btn-return', value: '<', onclick: () => {
                    // @ts-ignore
                    this.Config.BackAction();
                }
            }))
        }
        //"Activo", "Proceso", "Finalizado", "Espera", "Inactivo"
        column.append(WRender.Create({
            className: "options", children: [
                WSecurity.HavePermission(Permissions.GESTOR_TAREAS) ? {
                    tagName: 'button', className: 'btn-secondary', innerText: 'Editar', onclick: async () => this.taskEdit(this.Task)
                } : "", {
                    tagName: 'button', className: ' btn-success', innerText: 'Activar', onclick: async () => this.changeState(this.Task, "Activo")
                }, {
                    tagName: 'button', className: 'Btn-Mini btn-info', innerText: 'En proceso', onclick: async () => this.changeState(this.Task, "Proceso")
                }, {
                    tagName: 'button', className: 'Btn-Mini btn-danger', innerText: 'Finalizar', onclick: async () => this.changeState(this.Task, "Finalizado")
                }, {
                    tagName: 'button', className: 'Btn', innerText: 'En espera', onclick: async () => this.changeState(this.Task, "Espera")
                }, {
                    tagName: 'button', className: 'btn-danger2', innerText: 'Inactivar', onclick: async () => this.changeState(this.Task, "Inactivo")
                }
            ]
        }))
        
        return WRender.Create({ className: "container", children: [node] });
    }

    /**
    * 
    * @param {Tbl_Tareas} task
    */
    taskEdit = async (task) => {
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
        this.TaskModel = new Tbl_Tareas_ModelComponent();
        // @ts-ignore
        this.TaskModel.Tbl_Calendario = CalendarModel;
        this.append(new WModalForm({
            EditObject: task,
            AutoSave: true,
            title: "Editar",
            ModelObject: this.TaskModel
        }))
    }
    /**
     * 
     * @param {Tbl_Tareas} task
     * @param {String} state 
     */
    changeState = async (task, state) => {
        // @ts-ignore
        task.Estado = state;
        const response = await task.Update();
    }


    CustomStyle = css`
        @import url('https://maxcdn.bootstrapcdn.com/font-awesome/4.5.0/css/font-awesome.min.css');
            .component{
            display: block;
            }
            w-coment-component {
                display: flex;
                flex-direction: column-reverse
            }  
            .btn-return {
                position: absolute;
                top: -25px;
                left: 5px;
                width: 50px;
                padding: 5px;
            }          
            /*Just the background stuff*/
            .container {
                display: grid;
                grid-template-columns: 100%;
                column-gap: 20px;
            }    
            /*My hum... body.. yeah..*/           

            /* The card */
            .task-contaier {
                min-height: 500px;
                width: 100%;
                border-radius: 10px;
                grid-template-columns: calc(100% - 220px) 200px;
                column-gap: 20px;
                display: grid;

            }
            .task-contaier h3, .task-contaier h2, .task-contaier h4 {
                color: #0462bb;
                margin: 5px 0px;
            }
            .task-contaier .title {
                grid-column: span 2;
                border-bottom: 1px solid #ccc;
                padding-bottom: 10px;
                margin-bottom: 10px; 
            }
            /* Image on the left side */
            .thumbnail {
                float: left;
                position: relative;
                left: 30px;
                top: -10px;
                height: 100px;
                width: 180px;
                -webkit-box-shadow: 10px 10px 60px 0px rgba(0, 0, 0, 0.75);
                -moz-box-shadow: 10px 10px 60px 0px rgba(0, 0, 0, 0.75);
                box-shadow: 10px 10px 60px 0px rgba(0, 0, 0, 0.75);
                overflow: hidden;
            }

                /*object-fit: cover;*/
                /*object-position: center;*/
            img.left {
                position: absolute;
                left: 50%;
                top: 50%;
                height: auto;
                width: 100%;
                -webkit-transform: translate(-50%, -50%);
                -ms-transform: translate(-50%, -50%);
                transform: translate(-50%, -50%);
            }

            /* Right side of the card */
            .right {
                margin-left: 20px;
                margin-right: 20px;
            }

            .authores{
                display: flex;
                gap: 10px;
                align-items: center;
                width: 100%;
                flex-wrap: wrap;
            }

            .author {
                display: flex;
                align-items: center;
                gap: 5px;
                background-color: #9ECAFF;
                height: 50px;
                border-radius: 20px;
                font-size: 12px;
            }

            .author > img {
                float: left;
                height: 50px;
                width: 50px;
                border-radius: 50%;
            }

            p {
                text-align: justify;
                padding-top: 10px;
                font-size: 0.95rem;
                line-height: 150%;
                color: #4B4B4B;
            }

            .horarios{
                display: flex;
                gap: 5px;
                align-items: flex-start;
                width: 100%;
                flex-wrap: wrap;
                flex-direction: column;
            }

            .horario {
                padding: 0;
                margin: 0;
                text-align: right;
                font-size: 12px;
            }
            h5 {
               margin:0px;
               padding:0px;
               font-size: 16px;
               color: #C3C3C3;
            }           
            /* Floating action button */
            .options {
                display: flex;
                background-color: var(--secundary-color);
                padding: 10px;       
                text-align: center;    
                flex-wrap: wrap;
                gap: 5px;
                justify-content: flex-end;
                & button {
                    font-size: 12px;
                    margin: 0;
                    padding: 8px;
                    width: 100%;
                }
            }                  
    `

    BuildDatesDetail() {
        return this.Task?.Tbl_Calendario?.map(H => `<div class="horario"> Del 
            ${new DateTime(H.Fecha_Inicio)?.toDateFormatEs()}   hasta el 
            ${new DateTime(H.Fecha_Final)?.toDateFormatEs()}
        </div>`)?.join("") ?? "-";
    }
}
customElements.define('w-tarea-detail-component', TareaDetailView);
export { TareaDetailView };

