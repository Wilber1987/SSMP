//@ts-check
//import { Cat_Dependencias, Tbl_Servicios } from "../../ModelProyect/Tbl_CaseModule.js";
import { WForm } from "../../WDevCore/WComponents/WForm.js";
// @ts-ignore
import { ModelProperty } from "../../WDevCore/WModules/CommonModel.js";
import { EntityClass } from "../../WDevCore/WModules/EntityClass.js";
import { WAjaxTools } from "../../WDevCore/WModules/WAjaxTools.js";
import { Cat_Dependencias, Cat_Dependencias_ModelComponent } from "./Cat_Dependencias.js";
import { Tbl_Servicios_ModelComponent } from "./Tbl_Servicios.js";


//@ts-check
class Tbl_Profile extends EntityClass {
    constructor(props) {
        super(props, 'EntityHelpdesk');
        for (const prop in props) {
            this[prop] = props[prop];
        }
    }
    /**@type {ModelProperty}*/ Id_Perfil = { type: 'number', primary: true };
    /**@type {ModelProperty}*/ Nombres = { type: 'text' };
    /**@type {ModelProperty}*/ Apellidos = { type: 'text' };
    /**@type {ModelProperty}*/ FechaNac = { type: 'date', label: "fecha de nacimiento" };
    /**@type {ModelProperty}*/ Sexo = { type: "Select", Dataset: ["Masculino", "Femenino"] };
    /**@type {ModelProperty}*/ Foto = { type: 'img', require: false };
    /**@type {ModelProperty}*/ DNI = { type: 'text' };

    /**@type {ModelProperty}*/ Correo_institucional = { type: 'text', label: "correo", disabled: true, hidden: true };
    /**@type {ModelProperty}*/ Estado = { type: "Select", Dataset: ["ACTIVO", "INACTIVO"] };

    /** campos de investigaciones */
    //**@type {ModelProperty}*/ Tbl_Grupos_Profiles = { type: 'masterdetail', require: false , ModelObject: ()=> new Tbl_Grupos_Profiles_ModelComponent() };
    /**@type {ModelProperty}*/ ORCID = { type: 'text', require: false };
    //PROPIEDADES DE HELPDESK
    /**@type {ModelProperty}*/ Cat_Dependencias = {
        type: 'Multiselect', hiddenFilter: true,
        ModelObject: () => new Cat_Dependencias_ModelComponent(), require: false,
        action: async (Profile, /** @type {WForm} */ Form) => {
            if (Profile.Cat_Dependencias.length > 0) {
                const servicios = await new Tbl_Servicios_ModelComponent({
                    FilterData: [{
                        PropName: "Id_Dependencia", FilterType: "in", Values:
                            Profile.Cat_Dependencias.map(d => d.Id_Dependencia.toString())

                    }]
                }).Get();
                this.Tbl_Servicios.Dataset = servicios;
                this.Tbl_Servicios.disabled = false;
                Profile.Tbl_Servicios.forEach(servicio => {
                    if (!Profile.Cat_Dependencias.map(d => d.Id_Dependencia).includes(servicio.Id_Dependencia)) {
                        let filtObject = Profile.Tbl_Servicios.indexOf(servicio);
                        Profile.Tbl_Servicios.splice(filtObject, 1);
                    }
                });
                Form.DrawComponent();
            } else {
                this.Tbl_Servicios.disabled = true;
                this.Tbl_Servicios.Dataset = [];
                Profile.Tbl_Servicios = [];
                Form.DrawComponent();
            }
        }
    }
    /**@type {ModelProperty}*/ Tbl_Servicios = {
        type: 'Multiselect', hiddenFilter: true, ModelObject: () => new Tbl_Servicios_ModelComponent(),
        require: false, disabled: async (Profile, /** @type {WForm} */ Form) => {
            return Profile.Tbl_Servicios?.length == 0
        }
    }
    /**
      * @param {Array<Tbl_Profile>} perfiles
      * @param {Cat_Dependencias} dependencia
      * @returns {Promise<Object>}
      */
    AsignarDependencias = async (perfiles, dependencia) => {
        return await WAjaxTools.PostRequest("/api/Proyect/AsignarDependencias", {
            perfiles: perfiles,
            dependencia: dependencia
        });
    }
}

export { Tbl_Profile };
