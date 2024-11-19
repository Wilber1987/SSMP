
//@ts-check
import { WSecurity } from "../WDevCore/Security/WSecurity.js";
import { WModalForm } from "../WDevCore/WComponents/WModalForm.js";
import { ProfileRequest, ProfileRequest_ModelComponent } from "./ProfilesRequest.js";


window.onload = () => {
    // @ts-ignore
    editBtn.onclick = () => {
        document.body.appendChild(new WModalForm({
            ModelObject: new ProfileRequest_ModelComponent({
                Nombre: undefined, 
                Telefono_Anterior: undefined,
                Celular_Anterior: undefined, 
                Correo_Anterior: undefined, 
                Observacion: undefined,
                Estado: undefined
            }),
            AutoSave: true,
            EditObject: new ProfileRequest({
                Correo: WSecurity.UserData.Correo,
                Telefono: WSecurity.UserData.Telefono,
                Celular: WSecurity.UserData.Celular,
                Direccion: WSecurity.UserData.Direccion,
                Foto: WSecurity.UserData.Foto
            }), ObjectOptions  : {
                SaveFunction: async () => {
                    location.reload();                    
                }
            }
        }));
    }
}