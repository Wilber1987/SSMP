//@ts-check
import { EntityClass } from "../../../WDevCore/WModules/EntityClass.js";
// @ts-ignore
import { ModelProperty } from "../../../WDevCore/WModules/CommonModel.js";

class Tbl_Pronts_ModelComponent extends EntityClass {
    /** @param {Partial<Tbl_Pronts_ModelComponent>} [props] */
    constructor(props) {
        super(props, 'ArticleManager');
        for (const prop in props) {
            this[prop] = props[prop];
        }
    }
    /**@type {ModelProperty}*/ Id_Pront = { type: 'number', primary: true };
    /**@type {ModelProperty}*/ Pront_Line = { type: 'richtext' };
    /**@type {ModelProperty}*/ Type = {
        type: 'select', Dataset: ["SYSTEMPRONT",
            "RASTREO_Y_SEGUIMIENTOS",
            "INFORMACION_ENTREGAS_SEGUIMIENTOS",
            "INFORMACION_SOBRE_DOCUMENTOS",
            "QUEJAS_POR_IMPORTES",
            "QUEJAS_POR_ESTAFA",
            "QUEJAS_GENERALES",
            "QUEJAS_POR_RETRASOS",
            "CONSULTA_DE_HORARIOS",
            "CONSULTA_DE_CONTACTO",
            "EVENTOS",
            "SOLICITUD_DE_ASISTENCIA",
            "ASISTENCIA_GENERAL",
            "SERVICES_PRONT_VALIDATOR_CONTEXT"]
    };
}

export { Tbl_Pronts_ModelComponent };
