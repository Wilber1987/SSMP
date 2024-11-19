import { EntityClass } from "../../WDevCore/WModules/EntityClass.js";
import { Tbl_Profile } from "./Tbl_Profile.js";

class Tbl_Grupo extends EntityClass {
    constructor(props) {
        super(props, 'EntityHelpdesk');
        for (const prop in props) {
            this[prop] = props[prop];
        };
       
        
    }
    /**@type {Number}*/ Id_Grupo;
    /**@type {Number}*/ Id_Perfil_Crea;    
    /**@type {String}*/ Nombre;
    /**@type {String}*/ Descripcion;
    /**@type {String}*/ color;
    /**@type {Array<Tbl_Grupos_Profile>}*/ Tbl_Grupos_Profiles;
    /**@type {String}*/ Color;
}
export { Tbl_Grupo }

class Tbl_Grupo_ModelComponent extends EntityClass {
    constructor(props) {
        super(props, 'EntityHelpdesk');
        for (const prop in props) {
            this[prop] = props[prop];
        }
    }
    /**@type {ModelProperty}*/ Id_Grupo = { type: 'number', primary: true };
    /**@type {ModelProperty}*/ Nombre = { type: 'text' };
    /**@type {ModelProperty}*/ color = { type: 'color' };
    /**@type {ModelProperty}*/ Descripcion = { type: 'richtext' };
    //**@type {ModelProperty}*/ Tbl_Grupos_Profiles = { type: "masterdetail", ModelObject: () => new Tbl_Grupos_Profiles_ModelComponent() };
}

export { Tbl_Grupo_ModelComponent }


class Tbl_Grupos_Profile_ModelComponent extends EntityClass {
    constructor(props) {
        super(props, 'EntityHelpdesk');
        for (const prop in props) {
            this[prop] = props[prop];
        }
    }
    /**@type {ModelProperty}*/ Id_Grupo = { type: 'number', primary: true };
    /**@type {ModelProperty}*/ Id_Perfil = { type: 'number', primary: true };
    //**@type {ModelProperty}*/ Tbl_Grupos = { type: 'WSelect', hiddenFilter: true, ModelObject: () => new Tbl_Grupo_ModelComponent() };
    /**@type {ModelProperty}*/ Tbl_Profile = { type: 'WSelect', hiddenFilter: true, ModelObject: () => new Tbl_Profile() };
}
export { Tbl_Grupos_Profile_ModelComponent }

class Tbl_Grupos_Profile extends EntityClass {
    constructor(props) {
        super(props, 'EntityHelpdesk');
        for (const prop in props) {
            this[prop] = props[prop];
        };
       
    }
    /**@type {String}*/  Estado;
    /**@type {Number}*/ Id_Grupo;
    /**@type {Number}*/ Id_Perfil;
    /**@type {Tbl_Profile}*/ Tbl_Profile;
    /**@type {Tbl_Grupo}*/ Tbl_Grupo;

}
export { Tbl_Grupos_Profile }

const GroupState =
{
    ACTIVO: "ACTIVO", INACTIVO: "INACTIVO", RECHAZADO: "RECHAZADO", INVITADO: "INVITADO", SOLICITANTE: "SOLICITANTE"
}
export { GroupState }