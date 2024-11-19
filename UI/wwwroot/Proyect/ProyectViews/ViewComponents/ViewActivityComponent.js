import { StylesControlsV2, StyleScrolls } from "../../../WDevCore/StyleModules/WStyleComponents.JS";
import { WModalForm } from '../../../WDevCore/WComponents/WModalForm.js';
import { WRender } from "../../../WDevCore/WModules/WComponentsTools.js";
import { WCssClass, WStyledRender } from '../../../WDevCore/WModules/WStyledRender.js';
import { WAjaxTools } from "../../../WDevCore/WModules/WAjaxTools.js";
class ViewActivityComponent extends HTMLElement {
    constructor(Activity) {
        super();
        this.Activity = Activity;
        this.Container = WRender.Create({ className: "Container" });
        this.Container.append(WRender.Create({
            className: "DivOptions",
            children: [{
                tagName: 'button', className: 'Btn', innerText: 'Nueva Tarea',
                onclick: async () => {
                    await this.NewTarea();
                }
            }]
        }))
        this.TareasContainer = WRender.Create({ className: "TareasContainer" });
        this.append(WRender.createElement(StyleScrolls));
        this.append(WRender.createElement(StylesControlsV2));
        this.append(this.Style, this.Container, this.TareasContainer);
        this.Draw();
    }
    async NewTarea() {
        const modal = new WModalForm({
            ObjectModal: ReservaComp,
            title: "Nueva Tarea",
            StyleForm: "FullScreen"
        });
        this.append(modal);
    }
    connectedCallback() { }
    Draw = async () => {
        this.Dataset = await WAjaxTools.PostRequest("../../api/Calendar/TakeActividad", this.Activity);
        this.TareasContainer.innerHTML = "";
        this.Dataset.Tareas.forEach(tarea => {
            this.CreateTareaContainer(tarea);
        });
    }
    Style = new WStyledRender({
        ClassList: [
            new WCssClass(`.TareasContainer`, {
                display: "flex",
                "flex-direction": "column"
            }), new WCssClass(`.DivTarea`, {
                display: "grid",
                "grid-template-columns": "35% 15% 20% 20%",
                padding: "5px 15px",
                margin: 5,
                "grid-gap": 10,
                "box-shadow": "0 0 3px #888",
                "align-items": "center"
            }), new WCssClass(`.Activa, .Pendiente`, {
                "justify-content": "space-between",
                display: "flex"
            }), new WCssClass(`.Activa::after`, {
                content: "''",
                "border-radius": "50%",
                height: 20,
                width: 20,
                "background-color": "#1ba31f",
                display: "block"
            }), new WCssClass(`.Pendiente::after`, {
                content: "''",
                "border-radius": "50%",
                height: 20,
                width: 20,
                "background-color": "#d3210f",
                display: "block"
            }),
        ]
    });
    CreateTareaContainer(tarea) {
        this.TareasContainer.append(WRender.Create({
            className: "DivTarea",
            children: [
                tarea.Titulo,
                WRender.Create({ tagName: "label", innerText: tarea.Estado, className: tarea.Estado }),
                {
                    tagName: 'button', className: 'Btn-Mini', innerText: 'Agregar Evidencias',
                    onclick: async () => {
                        const modal = new WModalForm({
                            title: "Agregar Evidencias",
                            StyleForm: "columnX1",
                            ModelObject: {
                                Id_Tarea_hidden: tarea.Id_Tarea,
                                Evidencias: { type: "IMAGES" },
                            }, SaveFunction: async (Object) => {
                                const DataPost = {
                                    Id_Tarea: Object.OdTarea,
                                    Evidencias: Object.Evidencias.map(x => {
                                        if (typeof x === "string") {
                                            return { Id_Tarea: Object.Id_Tarea, Data: x };
                                        }
                                        return x;
                                    })
                                };
                                const response = await WAjaxTools.PostRequest(
                                    "./api/Calendar/SaveEvidencias", DataPost
                                );
                                modal.close();
                            }
                        });
                        this.append(modal);
                    }
                }, {
                    tagName: 'button', className: 'Btn-Mini', innerText: 'Marcar Finalizado',
                    onclick: async () => {
                    }
                },
            ]
        }));
    }
}
customElements.define('w-component', ViewActivityComponent);
export { ViewActivityComponent };
