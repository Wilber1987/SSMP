//@ts-check
import { WSecurity } from "../../WDevCore/Security/WSecurity.js";
import { StylesControlsV2, StylesControlsV3, StyleScrolls } from "../../WDevCore/StyleModules/WStyleComponents.js";
import { WCommentsComponent } from "../../WDevCore/WComponents/WCommentsComponent.js";
import { ComponentsManager, html, WRender } from "../../WDevCore/WModules/WComponentsTools.js";
import { ControlBuilder } from "../../WDevCore/WModules/WControlBuilder.js";
import { css } from "../../WDevCore/WModules/WStyledRender.js";
import { Conversacion } from "../Model/Conversacion.js";
import { Mensajes_ModelComponent } from "../Model/ModelComponent/Mensajes_ModelComponent.js";
const route = location.origin
const routeContacto = location.origin + "/Media/Images/profile/"


/**
 * @typedef {Object} MensajesConfig
 * * @property {Object} [propierty]
 */
class MensajesView extends HTMLElement {
    /**
     * 
     * @param {MensajesConfig} Config 
     */
    constructor(Config) {
        super();
        this.Config = Config
        this.OptionContainer = WRender.Create({ className: "OptionContainer" });
        this.TabContainer = WRender.Create({ className: "TabContainer", id: "content-container" });
        this.Manager = new ComponentsManager({ MainContainer: this.TabContainer, SPAManage: false });
        this.append(
            StylesControlsV2.cloneNode(true),
            StyleScrolls.cloneNode(true),
            StylesControlsV3.cloneNode(true),
            //this.OptionContainer,
            this.CustomStyle
        );
        this.SearchInput = ControlBuilder.BuildSearchInput(this.SearchContacto);
        this.ConversacionesContact = html`<section class="contacts-container aside-container">
        </section>`;
        this.Draw();
    }

    async Draw() {
        const contentContact = html`<section class="contacts-container aside-container"> 
             <div class="contact-container">
                 <h4 class="text-uppercase">Mensajes</h4>
             </div>
         </section>`;
        this.ConversacionesContact?.append(
            html`<div class="options-container">  
                <h4 class="text-uppercase">Conversaciones</h4>    
                ${this.SearchInput}      
            </div>`)
        await this.SearchContacto();
        this.append(html`<div class="main-container">${[contentContact, this.ConversacionesContact, this.TabContainer]}</div>`);
    }
    /**  */
    SearchContacto = async (searcName = "") => {
        /**@type {Array<Conversacion>} */
        const conversaciones = await new Conversacion({ Nombre_Completo: searcName }).Get();

        this.ConversacionesContact?.querySelector(".contact-container")?.remove();
        this.ConversacionesContact?.append(
            html`<div class="contact-container">${conversaciones.map(conversacion => this.BuildConversacion(conversacion))}</div>`
        )
        //this.VerConversacion(conversaciones[0]);
    }
    /**
     * @param {Conversacion} conversacion
     */
    BuildConversacion(conversacion) {
        const conversacionUsuarios = conversacion.Conversacion_usuarios.filter(c => c.Id_usuario != WSecurity.UserData.UserId);
        const conversacionCont = html`<div class="conversacion-card-container" onclick="${(/** @type {any} */ ev) => this.VerConversacion(conversacion, conversacionCont)}">            
            ${conversacionUsuarios.map(c => html`<img class="avatar" src="${route + c.Avatar}"/>`)}                                              
            <div class="data-conversacion-container">                    
                <div class="data-name-container">${conversacionUsuarios.map(c => html`<label>${c.Name}</label>`)}</div>
                ${conversacion.MensajesPendientes > 0 ?
                html`<span class="message-pendientes">${conversacion.MensajesPendientes}</span>`
                : ""
            }
            </div>
        </div>`;
        return conversacionCont
    }

    /**
    * @param {Conversacion} conversacion
    *  @param {HTMLElement} element
    */
    async VerConversacion(conversacion, element) {
        element.querySelector(".message-pendientes")?.remove();

        this.ConversacionSeleccionado = conversacion;
        if (this.ConversacionSeleccionado.Id_conversacion == null) {
            /**@type {Conversacion} */
            const conversacionF = await conversacion.Find();
            this.ConversacionSeleccionado = conversacionF;
        }

        const commentsContainer = new WCommentsComponent({
            Dataset: [],
            ModelObject: new Mensajes_ModelComponent(),
            User: WSecurity.UserData,
            UserIdProp: "Id_User",
            CommentsIdentify: this.ConversacionSeleccionado.Id_conversacion,
            CommentsIdentifyName: "Id_conversacion",
            UrlSearch: route + "/api/ApiMessageManager/getMensajes",
            UrlAdd: route + "/api/ApiMessageManager/saveMensajes",
            AddObject: true,
            UseDestinatarios: false,
            UseAttach: false
        });
        this.Manager.NavigateFunction("EstDetail_" +  this.ConversacionSeleccionado.Id_conversacion, commentsContainer);
    }
    connectedCallback() {
        this.Interval = setInterval(async () => {
            //this.SearchContacto(this.SearchInput.querySelector("input")?.value);
        }, 10000)
    }
    disconnectedCallback() {
        this.Interval = null;
    }

    CustomStyle = css`
        .main-container {
            display: grid;
            grid-template-columns: 400px calc(100% - 430px);
            grid-template-rows: 120px 600px;
            gap: 20px;
        }
        .TabContainer {
            grid-row: span 2;
            grid-column: span 2;
            grid-column-start: 2;
            grid-row-start: 1;
            min-height: 700px;
            max-height: 700px;
        }
        .options-container {
            display: flex;
            gap: 10px;
            flex-direction: column;
            margin-bottom: 20px;
        }
        .contact-container {
            display: flex;
            flex-direction: column;
            max-height: 440px;
            overflow-y: auto;
            gap: 10px;
        }
        .conversacion-card-container {
            padding: 10px;
            border-radius: 15px;
            border: solid 1px var(--fifty-color);
            cursor: pointer;
            display: flex;
            gap: 10px;
            align-items: center;
        }
        .data-conversacion-container {
            display: flex;
            flex: 1;
        }
        .data-name-container {
            display: flex;
            flex-wrap: wrap;
            font-size: 12px;
            flex: 1;
        }
        .message-pendientes {
            color: #fff;
            padding: 10px;
            background-color: #039a08;
            border-radius: 50%;
            height: 30px;
            width: 30px;
            display: flex;
            align-items: center;
            justify-content: center;
        }
        
        @media (max-width: 600px) {
            .main-container {
                display: flex;
                flex-direction: column;
              
            }
        }
    `

}
customElements.define('w-historial', MensajesView);
export { MensajesView };

