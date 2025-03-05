//@ts-check

import { WDocumentViewer } from "../../WDevCore/WComponents/WDocumentViewer.js";
import { WForm } from "../../WDevCore/WComponents/WForm.js";
import { WModalForm } from "../../WDevCore/WComponents/WModalForm.js";
import { html } from "../../WDevCore/WModules/WComponentsTools.js";
import { Notificaciones } from "../Model/Notificaciones.js";

/**
* @param {Notificaciones} notificacion 
* @returns {HTMLElement}
*/
export const BuildReportMessage = (notificacion) => {
    return new WModalForm({
        ObjectModal: new WDocumentViewer({
            exportPdf: true,
            Dataset: [
                html`<div>
                    <div style='text-align:center;'><img src='data:image/png;base64,${localStorage.getItem("MEMBRETE_HEADER")}' style='width:100%; display: block; margin: auto; height: auto;'></div>
                    <p style='text-align:left; font-size: 24px; padding: 60px 40px;'>${notificacion.Mensaje}</p>     
                    <div style='text-align:center;'><img src='data:image/png;base64,${localStorage.getItem("MEMBRETE_FOOTHER")}' style='width:90%; display: block; margin: auto;height: auto; position: absolute; bottom: 0;'></div>               
                </div>`
            ]
        })
    });
}