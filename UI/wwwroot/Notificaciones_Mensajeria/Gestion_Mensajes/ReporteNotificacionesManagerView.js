//@ts-check

import { NotificationRequest_ModelComponent, NotificationTypeEnum } from "../Model/ModelComponent/NotificacionRequest_ModelComponent.js";

import { NotificationRequest } from "../Model/NotificationRequest.js";
import { Cat_Dependencias, Cat_Dependencias_ModelComponent } from "../../Proyect/FrontModel/Cat_Dependencias.js";
import { Tbl_Profile } from "../../Proyect/FrontModel/Tbl_Profile.js";
import { StylesControlsV2, StylesControlsV3, StyleScrolls } from "../../WDevCore/StyleModules/WStyleComponents.js";
import { WAppNavigator } from "../../WDevCore/WComponents/WAppNavigator.js";

import { WModalForm } from "../../WDevCore/WComponents/WModalForm.js";
import { WTableComponent } from "../../WDevCore/WComponents/WTableComponent.js";
import { html, WRender } from "../../WDevCore/WModules/WComponentsTools.js";
import { css } from "../../WDevCore/WModules/WStyledRender.js";
// @ts-ignore
import { ModelProperty } from "../../WDevCore/WModules/CommonModel.js";
import "../../WDevCore/libs/html2pdf.js"
import "../../WDevCore/libs/xlsx.full.min.js"
import { Notificaciones_ModelComponent } from "../Model/ModelComponent/Notificacion_ModelComponent.js";
import { EntityClass } from "../../WDevCore/WModules/EntityClass.js";
import { Historial_NotificationsView } from "./Historial_NotificationsView.mjs";
import { ModalMessage } from "../../WDevCore/WComponents/ModalMessage.js";
import { ModalVericateAction } from "../../WDevCore/WComponents/ModalVericateAction.js";
import { BuildReportMessage } from "./ReporteMessageBuilder.js";


/**
 * @typedef {Object} ReporteNotificacionesManagerViewConfig
 * * @property {Object} [propierty]
 */
class ReporteNotificacionesManagerView extends HTMLElement {
	/**
	 * @param {ReporteNotificacionesManagerViewConfig} props 
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
			html`<h1>REPORTE DE NOTIFICACIONES</h1>`,
			//this.OptionContainer
		);
		this.Draw();
		this.NotificationType = NotificationTypeEnum.CLASE
	}
	Draw = async () => {
		this.SetOption();
	}


	async SetOption() {		
		/*this.OptionContainer.append()*/
		this.Navigator = new WAppNavigator({
			DarkMode: false,
			//Direction: "row",
			NavStyle: "tab",
			Inicialize: true,
			Elements: [
				{
					name: "Notificaciones enviadas", action: () => {
						return WRender.Create({ children: [this.SendsNotificationsComponent()] });
					}
				}, {
					name: "Informe", action: () => {
						return new Historial_NotificationsView();
					}
				}
			]
		});
		this.Navigator.className = "TabContainer"
		this.append(this.Navigator);
	}
	
	SendsNotificationsComponent() {
		this.NotificationType = NotificationTypeEnum.LIBRE;
		if (!this.SendsFreeNotificationComponent) {
			const model = new Notificaciones_ModelComponent({
				// @ts-ignore
				Get: async () => {
					return await model.GetData("ApiNotificacionesManage/GetNotificaciones")
				}
			})
			const NotificationTable = new WTableComponent({
				// @ts-ignore
				ModelObject: model,
				Options: {
					MultiSelect: true,					
					Filter: true,
					AutoSetDate: true,
					FilterDisplay: true,
					UserActions: [{
						name: "Reporte" ,action: (notificacion)=> {
							this.append(BuildReportMessage(notificacion))
						}
					}]
				}
			});
			this.SendsNotificationTable = NotificationTable;
			
			this.SendsFreeNotificationComponent = html`<div>
				<div class="OptionsContainer">
					<style> 
						.OptionsContainer{
							display: flex;
							justify-content: justify;
							gap: 20px;
							margin-bottom: 20px;
							& * {
								color: #fff;
							}
						}
					</style>             
				</div>
				${NotificationTable}
			</div>`
		}
		return this.SendsFreeNotificationComponent;
	}	
	CustomStyle = css`
		.TabContainer{
		   margin-top: 20px;
		}           
	`
	
}

class NotificactionDestinatarios_ModelComponent {
	/**@type {ModelProperty} */ Correo = { type: "text" };
	/**@type {ModelProperty} */ Telefono = { type: "text" };
	/**@type {ModelProperty} */ NotificationData = {
		type: "MODEL",
		label: "Datos del destinatario", ModelObject: () => new NotificationData_ModelComponent()
	};
}
class NotificationData_ModelComponent {
	Departamento = { type: "text" };
	NumeroPaquete = { type: "text" };
	Direccion = { type: "text" };
	Destinatario = { type: "text" };
	Identificacion = { type: "text", hidden: true };
	Correlativo = { type: "text" };
	Fecha = { type: "text" };
	Fecha_del_envio_de_notificacion = { type: "text" };
	Municipio = { type: "text" };
	Agencia = { type: "text" };
	NumeroAduana = { type: "text" };
	Correo = { type: "text", hidden: true };
	Telefono = { type: "text", hidden: true };
	Dpi = { type: "text" };
	Nit = { type: "text" };	
}
customElements.define('w-notificaciones-manager-view', ReporteNotificacionesManagerView);
export { ReporteNotificacionesManagerView }