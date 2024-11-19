import "../../WDevCore/WComponents/WTableComponent.js";
import { ComponentsManager, WRender } from '../../WDevCore/WModules/WComponentsTools.js';
import { WCssClass } from '../../WDevCore/WModules/WStyledRender.js';
import { ProyectDetailViewer } from './ProyectDetailViewer.js';
import { WAjaxTools } from "../../WDevCore/WModules/WAjaxTools.js";

const DOMManager = new ComponentsManager();
class ViewProyects extends HTMLElement {
    constructor(TipoProyecto) {
        super();
        this.TipoProyecto = TipoProyecto;
        this.ViewProyectsContainer = WRender.createElement({ type: 'div', props: { class: 'ViewProyectsContainer' } });
        //this.ViewProyectsContainer.append(Card);
        this.TabContainer = WRender.createElement({ type: 'div', props: { id: "content-container", class: "content-container" } });
        this.TipoProyecto.forEach(element => {
            this.ComponentTab.Elements.push({
                name: element.Descripcion, url: "#", icon: element.icon,
                action: async (ev) => {
                    const response = await WAjaxTools.PostRequest("../../api/Proyect/TakeProyects", {
                        Id_Tipo_Servicio: element.Id_Tipo_Servicio
                    });
                    DOMManager.NavigateFunction(
                        "Tab-" + element.Descripcion,
                        new ViewProyectsTab(element, response), "TabContainer"
                    );
                }
            });
        });
        this.append(
            WRender.CreateStringNode("<div><h3 class='HeaderDis'>Proyectos Nacionales</h3><hr></div>"),
            WRender.createElement(this.styleComponent),
            this.ComponentTab,
            this.TabContainer
        );
    }
    connectedCallback() {
        if (this.ViewProyectsContainer.innerHTML != "") {
            return;
        }
        this.DrawComponent();
    }
    DrawComponent = async () => {
        console.log("connected");
    }
    ComponentTab = WRender.createElement({
        type: "w-app-navigator",
        props: {
            NavStyle: "tab",
            id: "GuidesNav",
            title: "Menu",
            Inicialize: true,
            Elements: []
        }
    });

    styleComponent = {
        type: 'w-style', props: {
            ClassList: [
                new WCssClass(`w-view-proyect`, {
                    display: 'block',
                    "background-color": "#fff",
                    padding: 20,
                    "border-radius": "0.3cm",
                    "box-shadow": "0 2px 5px 2px var(--fifty-color)"
                }), new WCssClass(`.ViewProyectsContainer`, {
                    display: 'grid',
                    "grid-template-columns": "250px auto",
                    "border-bottom": "solid 2px #bbbec1",
                    "padding-bottom": 10,
                    "margin-bottom": 20,
                }), new WCssClass(`.DataContainer`, {
                    display: 'flex',
                    "flex-direction": "column",
                    padding: "20px 0px"
                }), new WCssClass(` h3`, {
                    margin: "5px 10px",
                    color: "#09315f"
                }), new WCssClass(`.DataContainer label`, {
                    margin: "5px 10px",
                }), new WCssClass(`.MapIframe`, {
                    width: '100%',
                    height: 400,
                    border: "none",
                    "border-bottom": "solid 6px rgb(12 109 148)"
                }),
            ], MediaQuery: [{
                condicion: "(max-width: 800px)",
                ClassList: [
                    new WCssClass(`.ViewProyectsContainer`, {
                        display: 'grid',
                        "grid-template-columns": "100%"
                    })
                ]
            }]
        }
    };
}
class ViewProyectsTab extends HTMLElement {
    constructor(TipeProyect = {}, Dataset = []) {
        super();
        this.className = "Tab";
        this.append(WRender.createElement(this.style));
        this.append(WRender.Create({
            tagName: 'w-articles',
            //ArticleHeader : ["nombre_Proyecto"],
            ArticleBody: ["Nombre_Proyecto", "Descripcion_Servicio"],
            Dataset: Dataset,
            Options: {
                Search: true,
                ApiUrlSearch: "api/Investigaciones/TakeInvestigaciones",
                UserActions: [{
                    name: "Detalles", action: async (Article) => {
                        this.LoadProyect(Article.Id_Servicio);
                    }
                }]
            }
        }))
    }
    connectedCallback() { this.DraViewProyectsTab(); }
    DraViewProyectsTab = async () => { }
    LoadProyect = async (Id_Servicio) => {
        const response = await WAjaxTools.PostRequest("../../api/Proyect/TakeProyect",
            { Id_Servicio: Id_Servicio }
        );
        const ProyectMap = WRender.Create({});
        const BodyComponents = new ProyectDetailViewer(response, DOMManager);
        //this.appendChild(WRender.createElement(ModalComp(BodyComponents, ProyectMap)));
        DOMManager.NavigateFunction("proyecto" + Id_Servicio, BodyComponents)
    }
    Style = {
        type: "w-style",
        props: {
            ClassList: [
                new WCssClass(".Tab", {
                    padding: 10,
                    display: "grid",
                    "grid-template-columns": "29% 69%",
                    "grid-gap": 10
                })
            ], MediaQuery: [{
                condicion: '(max-width: 1200px)',
                ClassList: [
                    new WCssClass(".GuidesContainer", {
                        "grid-template-columns": "50% 50%"
                    }),
                ]
            }]
        }
    };
}
customElements.define('w-tab-proy', ViewProyectsTab);
export { ViewProyectsTab };
export { ViewProyects };
customElements.define('w-view-proyect', ViewProyects);
