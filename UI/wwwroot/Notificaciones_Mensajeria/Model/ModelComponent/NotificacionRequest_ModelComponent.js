//@ts-check

//@ts-ignore
import { ModelProperty } from "../../../WDevCore/WModules/CommonModel.js";
import { EntityClass } from "../../../WDevCore/WModules/EntityClass.js";
class NotificationRequest_ModelComponent extends EntityClass {
    /** @param {Partial<NotificationRequest_ModelComponent>} [props] */
    constructor(props) {
        super(props, 'Notificaciones');
        for (const prop in props) {
            this[prop] = props[prop];
        }
    }
   /**@type {ModelProperty}*/ Id = { type: 'number', primary: true };
   /**@type {ModelProperty}*/ Titulo = { type: 'text' };
   /**@type {ModelProperty}*/ Mensaje = { type: 'richtext' };
   /**@type {ModelProperty}*/ Files = { type: 'file' , require: false};
   /**@type {ModelProperty}*/ NotificationType = { type: 'text', hidden: true };
   /**@type {ModelProperty}*/ NotificationsServicesEnum = { type: 'multiselect', hidden: true };

}
export { NotificationRequest_ModelComponent }

const NotificationsServicesEnum = {
    MAIL: "MAIL"
}
export { NotificationsServicesEnum }

const NotificationTypeEnum =
{
    DEPENDENCIA: "DEPENDENCIA", USUARIOS: "USUARIOS", LIBRE: "LIBRE"
}
export { NotificationTypeEnum }