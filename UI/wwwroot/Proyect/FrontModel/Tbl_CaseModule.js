import { Tbl_Profile } from "../../WDevCore/Security/Tbl_Profile.js";
import { EntityClass } from "../../WDevCore/WModules/EntityClass.js";
import { Tbl_Servicios_ModelComponent } from "./Tbl_Servicios.js";
import { Tbl_Comments_ModelComponent } from "./Tbl_Comments.js";
import { Tbl_Tareas_ModelComponent } from "./Tbl_Tareas.js";
import { Cat_Cargos_Dependencias_ModelComponent } from "./Cat_Cargos_Dependencias.js";
import { Cat_Dependencias_ModelComponent } from "./Cat_Dependencias.js";
import { WAjaxTools } from "../../WDevCore/WModules/WAjaxTools.js";
import { ModelProperty } from "../../WDevCore/WModules/CommonModel.js";


class Tbl_Case_ModelComponent extends EntityClass {


    /**
    * @param {Partial<Tbl_Case_ModelComponent>} [props]
    */
    constructor(props) {
        super(props, 'EntityHelpdesk');
        for (const prop in props) {
            this[prop] = props[prop];
        };
        //this.Mail = undefined;
    }
    /**@type {ModelProperty} */
    Id_Case = { type: 'number', primary: true };
    /**@type {ModelProperty} */
    Id_Vinculate = { type: 'number', hidden: true };
    /**@type {ModelProperty} */
    //image = { type: 'img',   hidden: true};
    /**@type {ModelProperty} */
    firma = { type: 'draw', hidden: true };
    /**@type {ModelProperty} */
    Cat_Dependencias = {
        type: 'WSelect', hiddenFilter: true, ModelObject: () => new Cat_Dependencias_ModelComponent(),
        action: async (caso, form) => {
            const servicios = await new Tbl_Servicios_ModelComponent({ Id_Dependencia: caso.Cat_Dependencias?.Id_Dependencia }).Get();
            form.ModelObject.Tbl_Servicios.Dataset = servicios;
            form.DrawComponent();
        }
    };
    /**@type {ModelProperty} */
    Tbl_Servicios = { type: 'WSelect', hiddenFilter: true, ModelObject: () => new Tbl_Servicios_ModelComponent(), Dataset: [], hiddenInTable: true };
    /**@type {ModelProperty} */
    Titulo = { type: 'text' };
    /**@type {ModelProperty} */
    Fecha_Inicio = { type: 'date', hiddenInTable: true, };
    /**@type {ModelProperty} */
    Mail = { type: 'text', hidden: true };
    /**@type {ModelProperty} */
    Estado = { type: "Select", Dataset: ["Activo", "Espera", "Pendiente", "Finalizado"] };
    /**@type {ModelProperty} */
    Case_Priority = { type: "Select", Dataset: ["Alta", "Media", "Baja"], label: "Prioridad", hiddenInTable: true };
    /**@type {ModelProperty} */
    Fecha_Final = { type: 'date', hiddenFilter: true, hiddenInTable: true };
    /**@type {ModelProperty} */
    Descripcion = { type: 'richtext', hiddenFilter: true };
    /**@type {ModelProperty} */    Tbl_Tareas = undefined;
    /**
     * @returns {Array<Tbl_Case_ModelComponent>}
     */
    GetOwCase = async () => {
        return await this.GetData("Proyect/GetOwCase", true);
    }
    /**
     * @returns {Array<Tbl_Case_ModelComponent>}
     */
    GetOwCloseCase = async () => {
        return await this.GetData("Proyect/GetOwCloseCase");
    }
    /**
     * @returns {Array<Tbl_Case_ModelComponent>}
     */
    GetVinculateCase = async () => {
        return await this.GetData("Proyect/GetVinculateCase");
    }
    /**
     * @returns {Array<Tbl_Case_ModelComponent>}
     */
    GetOwSolicitudesPendientesAprobar = async () => {
        return await this.GetData("Proyect/GetOwSolicitudesPendientesAprobar");
    }
    /**
     * @returns {Array<Tbl_Case_ModelComponent>}
     */
    GetOwSolicitudesAprobadas = async () => {
        return await this.GetData("Proyect/GetOwSolicitudesAprobadas");
    }
    /**
     * @returns {Array<Tbl_Case_ModelComponent>}
     */
    GetOwSolicitudesRechazadas = async () => {
        return await this.GetData("Proyect/GetOwSolicitudesRechazadas");
    }
    /**
     * @returns {Array<Tbl_Case_ModelComponent>}
     */
    GetOwSolicitudesFinalizadas = async () => {
        return await this.GetData("Proyect/GetOwSolicitudesFinalizadas");
    }
    /**
    * @returns {Array<Tbl_Case_ModelComponent>}
    */
    GetOwSolicitudesVinculadas = async () => {
        return await this.GetData("Proyect/GetOwSolicitudesVinculadas");
    }
    /**
     * @returns {Array<Tbl_Case_ModelComponent>}
    */
    GetOwSolicitudesPendientes = async () => {
        return await this.GetData("Proyect/GetOwSolicitudesPendientes");
    }
    /**
     * @returns {Array<Tbl_Case_ModelComponent>}
     */
    GetOwSolicitudesProceso = async () => {
        return await this.GetData("Proyect/GetOwSolicitudesProceso");
    }
    /**
     * @returns {Array<Tbl_Case_ModelComponent>}
     */
    GetOwSolicitudesEspera = async () => {
        return await this.GetData("Proyect/GetOwSolicitudesEspera");
    }
    /**
     * @returns {Array<Tbl_Case_ModelComponent>}
     */
    GetSolicitudesPendientesAprobar = async () => {
        return await this.GetData("Proyect/GetSolicitudesPendientesAprobar");
    }
    /**
     * @returns {Array<Tbl_Case_ModelComponent>}
     */
    GetSolicitudesPendientesAprobarAdmin = async () => {
        return await this.GetData("Proyect/GetSolicitudesPendientesAprobarAdmin");
    }
    /**
     * @returns {Object}
     */
    RechazarSolicitud = async () => {
        return await this.GetData("Proyect/RechazarSolicitud");
    }
    /**
     * @returns {Object}
     */
    CerrarCaso = async () => {
        return await this.GetData("Proyect/CerrarCaso");
    }
    /**
    * @param {Array<Tbl_Case_ModelComponent>} element
    * @param {Tbl_Case_ModelComponent} table_case
    * @returns {Object}
    */
    AprobarCaseList = async (element, table_case) => {
        return await WAjaxTools.PostRequest("/api/Proyect/AprobarCaseList",
            { Tbl_Cases: element, servicio: table_case.Tbl_Servicios });
    }
    /**
       * @param {Array<Tbl_Case_ModelComponent>} element
       *  @param {Tbl_Comments_ModelComponent} comentario
       * @returns {Object}
       */
    RechazarCaseList = async (element, comentario) => {
        return await WAjaxTools.PostRequest("/api/Proyect/RechazarCaseList", {
            Tbl_Cases: element,
            comentarios: [comentario]
        });
    }
    /**
       * @param {Array<Tbl_Case_ModelComponent>} element
       * @param {Tbl_Comments_ModelComponent} dependencia
       * @param {Tbl_Case_ModelComponent} table_case
       * @param {Array<Tbl_Comments_ModelComponent>} comentarios
       * @returns {Object}
       */
    RemitirCasos = async (element, dependencia, comentarios, table_case) => {
        return await WAjaxTools.PostRequest("/api/Proyect/RemitirCasos", {
            Tbl_Cases: element,
            dependencia: dependencia,
            comentarios: comentarios,
            servicio: table_case.Tbl_Servicios
        });
    }
    /**
       * @param {Tbl_Case_ModelComponent} actividad
       * @returns {Object}
       */
    AddToBlackList = async (actividad) => {
        return await WAjaxTools.PostRequest("/api/ApiEntityADMINISTRATIVE_ACCESS/AddToBlackList", actividad);
    }
    /**
        * @param {Tbl_Case_ModelComponent} actividad
        * @returns {Object}
    */
    RemoveFromBlackList = async (actividad) => {
        return await WAjaxTools.PostRequest("/api/ApiEntityADMINISTRATIVE_ACCESS/RemoveFromBlackList", actividad);
    }
    /**
        * @param {Tbl_Case} actividad
        * @returns {Object}
        */
    IsInBlackList = async (actividad) => {
        return await WAjaxTools.PostRequest("/api/ApiEntityADMINISTRATIVE_ACCESS/IsInBlackList", actividad);
    }
}
export { Tbl_Case_ModelComponent }
class Tbl_Case extends EntityClass {

    /** @param {Partial<Tbl_Case>} [props] */
    constructor(props) {
        super(props, 'EntityHelpdesk');
        for (const prop in props) {
            this[prop] = props[prop];
        };
        this.Progreso = undefined;
    }

    /**@type {Number}*/ Id_Case;
    /**@type {Number}*/ Id_Vinculate;
    /**@type {Number}*/ Id_Dependencia;
    /**@type {String}*/ Titulo;
    /**@type {String}*/ Descripcion;
    /**@type {String}*/ Estado;
    /**@type {Date}*/ Fecha_Inicio;
    /**@type {Date}*/ Fecha_Final;
    /**@type {String}*/ Mail;
    /**@type {String}*/ Case_Priority;
    /**@type {Object}*/ MimeMessageCaseData;
    /**@type {Tbl_Profile} ManyToOne*/ Tbl_Profile;
    /**@type {Cat_Dependencias} ManyToOne*/ Cat_Dependencias;
    /**@type {Tbl_Servicios} ManyToOne*/ Tbl_Servicios;
    /**@type {Tbl_VinculateCase} ManyToOne*/ Tbl_VinculateCase;
    /**@type {Array<Tbl_Comments>} OneToMany*/ Tbl_Comments;
    /**@type {Array<Tbl_Mails>} OneToMany*/ Tbl_Mails;
    /**@type {Array<Tbl_Profile_CasosAsignados>} OneToMany*/ Tbl_Profile_CasosAsignados;
    /**@type {Array<Tbl_Tareas>} OneToMany*/ Tbl_Tareas;
    async SaveOwCase() {
        return await this.SaveData("Proyect/SaveOwCase", this);
    }
}
export { Tbl_Case }
class Tbl_Dependencias_Usuarios extends EntityClass {
    constructor(props) {
        super(props, 'EntityHelpdesk');
        for (const prop in props) {
            this[prop] = props[prop];
        }
    }
    Tbl_Profile = { type: 'WSelect', hiddenFilter: true, ModelObject: () => new Tbl_Profile() }
    Cat_Cargos_Dependencias = { type: 'WSelect', hiddenFilter: true, ModelObject: () => new Cat_Cargos_Dependencias_ModelComponent() }
}
export { Tbl_Dependencias_Usuarios }

class Tbl_VinculateCase extends EntityClass {
    constructor(props) {
        super(props, 'EntityHelpdesk');
        for (const prop in props) {
            this[prop] = props[prop];
        }
    }
    Id_Vinculate = { type: 'number', primary: true };
    Descripcion = { type: 'text' };
    //Fecha = { type: 'dated' };


    Casos_Vinculados = {
        type: 'MasterDetail', ModelObject: () => new Tbl_Case_ModelComponent(),
        require: false
    };

    VincularCaso = async () => {
        return await this.GetData("Proyect/VincularCaso");
    }
    DesvincularCaso = async () => {
        return await this.GetData("Proyect/DesvincularCaso");
    }
}
export { Tbl_VinculateCase }