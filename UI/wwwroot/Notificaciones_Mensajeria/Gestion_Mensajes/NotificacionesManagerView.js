//@ts-check

import { NotificationRequest_ModelComponent, NotificationTypeEnum } from "../Model/ModelComponent/NotificacionRequest_ModelComponent.js";

import { NotificationRequest } from "../Model/NotificationRequest.js";
import { Cat_Dependencias, Cat_Dependencias_ModelComponent } from "../../Proyect/FrontModel/Cat_Dependencias.js";
import { Tbl_Profile } from "../../Proyect/FrontModel/Tbl_Profile.js";
import { StylesControlsV2, StylesControlsV3, StyleScrolls } from "../../WDevCore/StyleModules/WStyleComponents.js";
import { WAppNavigator } from "../../WDevCore/WComponents/WAppNavigator.js";
import { ModalMessege, ModalVericateAction } from "../../WDevCore/WComponents/WForm.js";
import { WModalForm } from "../../WDevCore/WComponents/WModalForm.js";
import { WTableComponent } from "../../WDevCore/WComponents/WTableComponent.js";
import {  WRender } from "../../WDevCore/WModules/WComponentsTools.js";
import { css } from "../../WDevCore/WModules/WStyledRender.js";


/**
 * @typedef {Object} NotificacionesManagerViewConfig
 * * @property {Object} [propierty]
 */
class NotificacionesManagerView extends HTMLElement {
    /**
     * @param {NotificacionesManagerViewConfig} props 
     */
    constructor(props) {
        super();       
        this.OptionContainer = WRender.Create({ className: "OptionContainer" });
        //this.Manager = new ComponentsManager({ MainContainer: this.TabContainer, SPAManage: false });
        this.append(this.CustomStyle);
        this.append(
            StylesControlsV2.cloneNode(true),
            StyleScrolls.cloneNode(true),
            StylesControlsV3.cloneNode(true),
            this.OptionContainer
        );
        this.Draw();
        this.NotificationType = NotificationTypeEnum.CLASE
    }
    Draw = async () => {
        this.SetOption();
    }


    async SetOption() {
        this.OptionContainer.append(WRender.Create({
            tagName: 'button', className: 'Block-Primary', innerText: 'NUEVA NOTIFICACIÓN',
            onclick: async () => this.ProcessRequest()
        }))
        this.Navigator = new WAppNavigator({
            DarkMode: false,
            //Direction: "row",
            NavStyle: "tab",
            Inicialize: true,
            Elements: [
                {
                    name: "Dependencia", action: () => {
                        return this.DependenciasComponent();
                    }
                }, {
                    name: "Usuarios", action: () => {
                        return this.UsuariosComponent();
                    }
                }
            ]
        });
        this.Navigator.className = "TabContainer"
        this.append(this.Navigator);
    }
    DependenciasComponent() {
        this.NotificationType = NotificationTypeEnum.DEPENDENCIA;
        if (!this.DependenciaComponent) {
            this.DependenciaComponent = new WTableComponent({
                ModelObject: new Cat_Dependencias_ModelComponent(),
                EntityModel: new Cat_Dependencias(),
                AutoSave: true,
                Options: {
                    Filter: true,
                    FilterDisplay: true,
                    AutoSetDate: false,
                    MultiSelect: true
                }
            })
        }
        return this.DependenciaComponent;
    }
    UsuariosComponent() {
        this.NotificationType = NotificationTypeEnum.SECCION;
        if (!this.UsuarioComponent) {
            this.UsuarioComponent = new WTableComponent({
                ModelObject: new Tbl_Profile(),
                AutoSave: true,
                Options: {
                    Filter: true,
                    FilterDisplay: true,
                    AutoSetDate: false,
                    MultiSelect: true
                }
            })
        }
        return this.UsuarioComponent;
    }

    CustomStyle = css`
        .TabContainer{
           margin-top: 20px;
        }           
    `
    ProcessRequest = () => {
        const modal = new WModalForm({
            ModelObject: new NotificationRequest_ModelComponent(), 
            title: "NUEVA NOTIFICACIÓN",
            StyleForm: "columnX1",
            ObjectOptions: {
                SaveFunction: async (/**@type {NotificationRequest} */ entity) => {
                    let mensaje = ""
                    console.log(this.NotificationType);
                    
                    if (this.NotificationType == NotificationTypeEnum.USUARIOS) {
                        entity.NotificationType = NotificationTypeEnum.USUARIOS;
                        // @ts-ignore
                        entity.Usuarios = this.UsuarioComponent?.selectedItems.map(s => s.User_id );
                        mensaje =  entity.Usuarios.length == 0 ? "a todas los Usuarios" : "a los Usuarios selecionadas";


                    } else if  (this.NotificationType == NotificationTypeEnum.DEPENDENCIA) {
                        entity.NotificationType = NotificationTypeEnum.DEPENDENCIA;
                        // @ts-ignore
                        entity.Dependencias = this.DependenciaComponent?.selectedItems.map(s => s.Id_Dependencia );
                        mensaje =  entity.Dependencias.length == 0 ? "a todas las dependencias" : "a las clases selecionadas";

                    } 
                    document.body.appendChild(ModalVericateAction(async () => {
                        const response = await new NotificationRequest(entity).Save();
                        document.body.appendChild(ModalMessege(response.message, undefined, true));
                        modal.close();
                    }, `¿Desea enviar la notificación ${mensaje}?`));
                }
            }
        });
        document.body.append(modal);
    }
}
customElements.define('w-notificaciones-manager-view', NotificacionesManagerView);
export { NotificacionesManagerView }