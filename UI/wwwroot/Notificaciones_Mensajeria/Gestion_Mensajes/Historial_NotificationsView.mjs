//@ts-check


import { StylesControlsV2, StylesControlsV3, StyleScrolls } from "../../WDevCore/StyleModules/WStyleComponents.js";
import { ModalMessage } from "../../WDevCore/WComponents/ModalMessage.js";
import { WFilterOptions } from "../../WDevCore/WComponents/WFilterControls.js";
import { WPrintExportToolBar } from "../../WDevCore/WComponents/WPrintExportToolBar.mjs";
import { PageType } from "../../WDevCore/WComponents/WReportComponent.js";
import { DateTime } from "../../WDevCore/WModules/Types/DateTime.js";
import { WAjaxTools } from "../../WDevCore/WModules/WAjaxTools.js";
import { ComponentsManager, html, WRender } from "../../WDevCore/WModules/WComponentsTools.js";
import { css } from "../../WDevCore/WModules/WStyledRender.js";
import { Notificaciones_ModelComponent } from "../Model/ModelComponent/Notificacion_ModelComponent.js";
import { Notificaciones } from "../Model/Notificaciones.js";

const route = location.origin
const routeEstudiantes = location.origin + "/Media/Images/estudiantes/"

/**
 * @typedef {Object} Historial_NotificationsViewConfig
 * * @property {Object} [propierty]
 */
class Historial_NotificationsView extends HTMLElement {
	/**
	 * @param {Historial_NotificationsViewConfig} [props] 
	 */
	constructor(props) {
		super();
		this.OptionContainer = WRender.Create({ className: "OptionContainer" });
		this.TabContainer = WRender.Create({ className: "TabContainer", id: 'TabContainer' });
		this.Manager = new ComponentsManager({ MainContainer: this.TabContainer, SPAManage: false });
		this.append(this.CustomStyle);
		const container = WRender.Create({ className: "component" });

		// @ts-ignore
		const EntityModel = new Notificaciones({
			// @ts-ignore
			Get: async () => {
				return await EntityModel.GetData("ApiNotificacionesManage/GetNotificacionesEnviadas")
			}
		})
		this.Filter = new WFilterOptions({
			AutoSetDate: true,
			AutoFilter: true,
			EntityModel: EntityModel,
			// @ts-ignore
			ModelObject: new Notificaciones_ModelComponent(),
			UseEntityMethods: true,
			Display: true,
			Dataset: [],
			FilterFunction: async (filterData) => {
				container.innerHTML = "";
				this.Dataset = filterData;
				//console.log(this.Filter.ModelObject);
				/*const facturas = await new NotificationsRequest().Where(
					this.Filter.ModelObject.FilterData[0]
				)*/
				//this.DrawInformeNotifications(filterData);
				container.append(this.DrawInformeNotifications(filterData));
				this.Manager.NavigateFunction("informe", container)
			}
		});
		const notificationReportData = {
			FirstDate: null,
			LastDate: null,
			Mail: "",
			Notificaciones: []
		}
		this.OptionContainer.append(html`<div class="control-container">
			<input type="text" placeholder="escribe un correo" onchange="${(ev) => {
				notificationReportData.Mail = ev.target.value;
				console.log(notificationReportData);

			}}"/>
			<button class="BtnSuccess" onclick="${async () => {
				notificationReportData.Notificaciones = this.Dataset;
				notificationReportData.FirstDate = this.Filter.ModelObject.FilterData[0].Values[0];
				notificationReportData.LastDate = this.Filter.ModelObject.FilterData[0].Values[1];
				console.log(notificationReportData);
				const response = await WAjaxTools.PostRequest("/api/Report/SenReportNotifications", notificationReportData);
				if (response.status == 200) {
					document.body.append(ModalMessage(response.message))
				}
			}}">Enviar</button>
		</div>`);
		this.OptionContainer.append(new WPrintExportToolBar({
			ExportPdfAction: (/**@type {WPrintExportToolBar} */ tool) => {
				const body = container.cloneNode(true);
				body.appendChild(this.CustomStyle.cloneNode(true));
				tool.ExportPdf(body, PageType.OFICIO_HORIZONTAL)
			}
		}));
		this.append(
			StylesControlsV2.cloneNode(true),
			StyleScrolls.cloneNode(true),
			StylesControlsV3.cloneNode(true),
			this.OptionContainer,
			this.Filter,
			this.TabContainer
		);
		this.Informes = {};
		this.Draw();
	}
	Draw = async () => {
		this.MainComponent();
	}

	async SetOption() {

	}
	async MainComponent() {

		//return container;
	}

	/**
	 * @param {Array<Notificaciones>} notificaciones 
	 */
	DrawInformeNotifications(notificaciones) {
		console.log(notificaciones);


		// @ts-ignore                    
		const NotificationsNotificacion = Object.groupBy(notificaciones, p => p.Year);
		const containerInforme = this.ViewNotificacionInforme(NotificationsNotificacion);
		//this.Manager.NavigateFunction("informe",);
		return containerInforme;
	}
	ViewNotificacionInforme(notificaciones) {
		// @ts-ignore

		//console.log(facturasNotificacion);
		const div = html`<div class="Notification-container">
		<style>
			.Notification-container {
				display: flex;
				flex-direction: column;
				gap: 10px;
				padding: 10px;
			}
			</style>            
		</div>`;

		for (const NotificacionId in notificaciones) {
			/**@type {Notificaciones} */
			const notificacion = notificaciones[NotificacionId][0];
			const NotificacionContainer = html`<div class="">
				<h1 style="color: #09559c">${localStorage.getItem("TITULO")}</h1>
				<h2 style="color: #0c3964; border-bottom: solid 1px #cccccc; ">Informe de notificaciones enviadas ${notificacion.Year}</h2>
				<!-- <div class="Notificacion">
					<div class="data-container">
						<label class="Notificacion-prop">AÑO:</label>
						<label>{notificacion.Year} - </label>
					</div>
				</div>				 -->
			 </div>`;

			//NotificacionContainer.append(html`<h3>Cargo</h3>`);
			//NotificationsNotificacion[NotificacionId].forEach((/** @type {Tbl_Notification} */ Notification) => {
			const notificacionesMes = this.BuildNotificacionesXMes(notificaciones[NotificacionId]);
			NotificacionContainer.append(notificacionesMes);
			//});
			div.append(NotificacionContainer);
		}
		return div;
	}

	/**
	 * Construye los Notifications del mes y los agrega al contenedor
	 * @param {Notificaciones[]} notificaciones - Lista de Notifications
	 */
	BuildNotificacionesXMes(notificaciones) {
		const div = html`<div class="Notification-mes-container"></div>`;
		// @ts-ignore
		const notificacionesGroup = Object.groupBy(notificaciones, p => p.Month);

		console.log(notificacionesGroup);

		for (const NotificationsMes in notificacionesGroup) {
			div.append(html`<h3>${NotificationsMes.toUpperCase()}</h3>`)
			const mesContainer = html`<table class="mes-container">				
				<tr class="Notification-details-container">
					<td class="Notification-title">No. de teléfono</td>
					<td class="Notification-title">Destinatario</td>
					<td class="Notification-title">No. Paquete</td>
					<td class="Notification-title">No. Aduana</td>
					<td class="Notification-title">Cita</td>
					<td class="Notification-title">Dirección del destinatario</td>
					<td class="Notification-title">Fecha de ingreso</td>
					<td class="Notification-title">Departamento</td>
					<td class="Notification-title">Municipio</td>
					<td class="Notification-title">Agencia</td>
					<!-- <td class="Notification-title" width="300px">Mensaje</td> -->
					<td class="Notification-title">Correlativo</td>
					<td class="Notification-title">Fecha de ingreso de notificación</td>
					<td class="Notification-title">DPI</td>
					<td class="Notification-title">NIT</td>
					<td class="Notification-title">E-mail</td>
				</tr>
			</table>`;
			//-------------------->
			//mesContainer.append(html`<h3>Notificaciones</h3>`);
			notificacionesGroup[NotificationsMes].forEach((/** @type {Notificaciones} */ Notification) => {
				const card = this.NotificationsCard(new Notificaciones(Notification));
				mesContainer.append(card);
			});
			div.append(mesContainer);
			div.append(html`<div class="Notification-details-container total-container">
				<div class="Notification-title" style="grid-column: span 12">Sub-Total</div>
				<div class="Notification-title value">${notificacionesGroup[NotificationsMes].length}</div>			
			</div>`)
			//-------------------->
			//mesContainer.append(html`<h3>Resumen</h3>`);

		}
		div.append(html`<div class="mes-container total-container">
			<div class="Notification-title" style="grid-column: span 12">Total</div>
			<div class="Notification-title total-cargos">${notificaciones.length}</div>
		</div>`);
		return div;
	}
	/**
	 * @param {Notificaciones} Notification
	 */
	NotificationsCard(Notification) {
		return WRender.Create({
			tagName: "tr", className: "Notification-details-container",
			children: [
				{ tagName: "td", class: "Notification-value", innerText: Notification.GetParam("N.º de teléfono") },
				{ tagName: "td", class: "Notification-value", innerText: Notification.GetParam("Destinatario") },
				{ tagName: "td", class: "Notification-value", innerText: Notification.GetParam("No. de paquete") },
				{ tagName: "td", class: "Notification-value", innerText: Notification.NotificationData.NumeroAduana ?? "Desconocido" },
				{ tagName: "td", class: "Notification-value", innerText: Notification.GetParam("Cita") },
				{ tagName: "td", class: "Notification-value", innerText: Notification.GetParam("Dirección del destinatario") },
				//{ tagName: "td", class: "Notification-value", innerText: new DateTime(Notification.GetParam("Fecha de ingreso")).toDDMMYYYY() },
				{ tagName: "td", class: "Notification-value", innerText: Notification.GetParam("Fecha de ingreso") },
				{ tagName: "td", class: "Notification-value", innerText: Notification.GetParam("Departamento") },
				{ tagName: "td", class: "Notification-value", innerText: Notification.GetParam("Municipio") },
				{ tagName: "td", class: "Notification-value", innerText: Notification.GetParam("Agencia") },
				//{ tagName: "td", class: "Notification-value", innerText: Notification.Mensaje ?? "-" },
				{ tagName: "td", class: "Notification-value", innerText: Notification.GetParam("Correlativo") },
				//{ tagName: "td", class: "Notification-value", innerText: new DateTime(Notification.GetParam("fecha del envió de notificación")).toDDMMYYYY() },
				{ tagName: "td", class: "Notification-value", innerText: Notification.GetParam("fecha del envió de notificación") },
				{ tagName: "td", class: "Notification-value", innerText: Notification.GetParam("Dpi") },
				{ tagName: "td", class: "Notification-value", innerText: Notification.NotificationData.Nit ?? "Desconocido" },
				{ tagName: "td", class: "Notification-value", innerText: Notification.GetParam("E-mail") },
			]
		});
	}

	CustomStyle = css`
		.component{
		   display: block;
		}    
		.CANCELADO {
			color: green;
		}  
		.PENDIENTE {
			color: red
		}  
		.control-container {
			display: flex;
			width: 400px;
			height: 45px;
		}
		.component {
			background-color: #FFF;
			color: #000;	
			border-radius: 10px;
		}
		.OptionContainer {
			display: flex;
			justify-content: flex-end;
			align-items: center;
		}
		.mes-container {
			/* display: grid;
			grid-template-columns: repeat(14, 1fr); */
			gap: 5px;
			& h3 {
				grid-column: span 14;
				font-size: 1em;
				border-bottom: 1px solid #919191;
			}
			
		}   
		table.mes-container {
			gap: 5px;
			border-collapse: collapse;
			width: 100%;
		}
		.Notification-details-container {
			/* display: contents; */
			grid-column: span 14;
		}
		.total-container {
			display: flex;
			justify-content: space-between;
			background-color: #f1f1f1;
			font-weight: bold;
		}
		.Notification-title {
			font-size: 0.8em;
			padding: 8px;
			font-weight: bold;
			background-color: #f1f1f1;
		}
		.Notification-value {
			font-size: 0.8em;
			padding: 8px;
			vertical-align: top;
		}
		.total-container {			
			font-size: 1em;
		} 
		.value, .total-cargos {
			text-align: right;
		}  
		.Historial{
			display: flex;
			flex-direction: column;            
			gap: 20px;
		}   
		.Historial .options-container {
			display: flex;
			align-items: center;
			justify-content: space-between;
			grid-column: span 2;
		}
		.Notificacion-card-container {
			display: flex;
			border: 1px solid #d6d6d6;;
			border-radius: 10px;
			cursor: pointer;
			padding: 10px;
			max-width: 400px; 
		}
		.Notificacion-card {
			display: flex;         
			gap: 10px;
			min-width: 400px;
			align-items: center;
		}
		.alumnos-container {
			display: flex;
			flex-direction: row;
			flex-wrap: wrap;
			gap: 10px;
		}
		.TabContainer {
			min-height: 400px;
		}
		.avatar-est{
			height: 100px;
			width: 80px;
			min-width: 80px;
			border-radius: 10px !important;
			object-fit: cover;
		}
		
		.aside-container {            
			padding: 0;
			border-radius: 0;
			box-shadow: unset
		}
		.Notificacion-container {
			display: flex;
			flex-direction: column;
			gap: 0px;
			padding: 10px;
			& .Notificacion {
				margin-bottom: 20px;
			}
			& .data-container {
				display: flex;
				justify-content: flex-start;
				/* border-bottom: 1px solid #d6d6d6; */
				& .Notificacion-prop {
					background-color: #f1f1f1;
					width: 100px;
				}
				& label {
					padding: 10px;
					margin-bottom: 0;
				}
			}
		}
		
		@media (max-width: 768px) {
			.Historial{               
				grid-template-columns: 100%;
			} 
			.Historial .options-container {
				grid-column: span 1;
			}
			.TabContainer {
				border-left: unset;
				padding-left: unset;                
			}
		}
	`
}
customElements.define('w-component', Historial_NotificationsView);
export { Historial_NotificationsView };
