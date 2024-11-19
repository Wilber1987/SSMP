import { EntityClass } from "../../WDevCore/WModules/EntityClass.js";

class ViewParticipantesServicios extends EntityClass {
   constructor(props) {
       super(props, 'ViewDBO');
       for (const prop in props) {
           this[prop] = props[prop];
       }
   }
   Id_Perfil = { type: 'number' };
   Id_Servicio = { type: 'number' };
   Fecha_Ingreso = { type: 'date' };
   Estado_Participante = { type: 'text' };
   Descripcion_Servicio = { type: 'text' };
   Fecha_Inicio = { type: 'date' };
   Fecha_Finalizacion = { type: 'date' };
   Visibilidad = { type: 'text' };
   Cargo = { type: 'text' };
   Descripcion = { type: 'text' };
   Estado = { type: 'SELECT', Dataset: ["ACTIVO", "INACTIVO"]  };
   Nombre_Proyecto = { type: 'text' };
   Estado = { type: 'SELECT', Dataset: ["ACTIVO", "INACTIVO"]  };
}
export { ViewParticipantesServicios };

class ViewCalendarioByDependencia extends EntityClass {
   constructor(props) {
       super(props, 'ViewDBO');
       for (const prop in props) {
           this[prop] = props[prop];
       }
   }
   Id_Case = { type: 'number' };
   Id_TareaPadre = { type: 'number' };
   Fecha_Inicio = { type: 'date' };
   Fecha_Final = { type: 'date' };
   Estado = { type: 'SELECT', Dataset: ["ACTIVO", "INACTIVO"]  };
   IdCalendario = { type: 'number' };
   Id_Tarea = { type: 'number' };
   Id_Dependencia = { type: 'number' };
}
export { ViewCalendarioByDependencia };
class ViewActividadesParticipantes extends EntityClass {
   constructor(props) {
       super(props, 'ViewDBO');
       for (const prop in props) {
           this[prop] = props[prop];
       }
   }
   Id_Case = { type: 'number' };
   Titulo = { type: 'text' };
   Descripcion = { type: 'text' };
   Estado = { type: 'SELECT', Dataset: ["ACTIVO", "INACTIVO"]  };
   Id_Perfil = { type: 'number' };
}
export { ViewActividadesParticipantes };

