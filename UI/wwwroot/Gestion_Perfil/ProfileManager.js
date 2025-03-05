//@ts-check

import { StylesControlsV2, StylesControlsV3, StyleScrolls } from "../WDevCore/StyleModules/WStyleComponents.js";
import { ModalMessage } from "../WDevCore/WComponents/ModalMessage.js";
import { ModalVericateAction } from "../WDevCore/WComponents/ModalVericateAction.js";

import { WModalForm } from "../WDevCore/WComponents/WModalForm.js";
import { WTableComponent } from "../WDevCore/WComponents/WTableComponent.js";
import { ComponentsManager, WRender } from "../WDevCore/WModules/WComponentsTools.js";
import { css } from "../WDevCore/WModules/WStyledRender.js";
import { ProfileRequest_ModelComponent, ProfileRequest, ProfileRequestsStatus } from "./ProfilesRequest.js";


/**
 * @typedef {Object} ProfileManagerConfig
 * * @property {Object} [propierty]
 */
class ProfileManager extends HTMLElement {
    /**
     * 
     * @param {ProfileManagerConfig} props 
     */
    constructor(props) {
        super();
        this.attachShadow({ mode: 'open' });
        this.OptionContainer = WRender.Create({ className: "OptionContainer" });
        this.TabContainer = WRender.Create({ className: "TabContainer", id: "content-container" });
        this.Manager = new ComponentsManager({ MainContainer: this.TabContainer, SPAManage: false });
        this.shadowRoot?.append(this.CustomStyle);
        this.shadowRoot?.append(
            StylesControlsV2.cloneNode(true),
            StyleScrolls.cloneNode(true),
            StylesControlsV3.cloneNode(true),
            this.OptionContainer,
            this.TabContainer
        );
        this.Draw();
    }
    Draw = async () => {
        this.SetOption();
    }

    async SetOption() {
        this.OptionContainer.append(WRender.Create({
            tagName: 'button', className: 'Block-Primary', innerText: 'Solicitudes de perfil',
            onclick: async () => this.Manager.NavigateFunction("id", await this.MainComponent())
        }))
        this.Manager.NavigateFunction("id", await this.MainComponent());
    }
    async MainComponent() {
        return new WTableComponent({
            ModelObject: new ProfileRequest_ModelComponent(),
            EntityModel: new ProfileRequest(),
            AutoSave: true,
            Options: {
                Search: true,
                FilterDisplay: true,
                AutoSetDate: false,
                UserActions: [
                    {
                        name: "Aprobar", action: async (/** @type {ProfileRequest} */ ProfileRequest) => {
                            this.ProcessRequest(ProfileRequest, ProfileRequestsStatus.APROBADO);

                        }
                    },
                    {
                        name: "Rechazar", action: async (/** @type {ProfileRequest} */ ProfileRequest) => {
                            this.ProcessRequest(ProfileRequest, ProfileRequestsStatus.RECHAZADO);
                        }
                    }
                ]
            }
        })
    }

    CustomStyle = css`
        .component{
           display: block;
        }           
    `

    /**
     * @param {ProfileRequest} ProfileRequest
     * @param {string} ESTADO
     */
    ProcessRequest(ProfileRequest, ESTADO) {


        const modal = new WModalForm({
            ModelObject: {
                observacion: { type: "TEXTAREA" }
            }, title: "Solicitud de perfil",
            ObjectOptions: {
                SaveFunction: async (entity) => {
                    document.body.appendChild(ModalVericateAction(async () => {
                        ProfileRequest.Estado = ESTADO;
                        ProfileRequest.Observacion = entity.observacion;
                        const response = await ProfileRequest.Update();
                        document.body.appendChild(ModalMessage(response.message, undefined, true));
                        modal.close();
                    }, `¿Desea cambiar el estado de la solicitud a "${ESTADO.toLowerCase()}"?`));
                }
            }
        });
        document.body.append(modal);
    }
}
customElements.define('w-profile-manager-view', ProfileManager);
export { ProfileManager }