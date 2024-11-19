using CAPA_DATOS;
namespace DataBaseModel {

   public class ViewParticipantesServicios : EntityClass {
       public int? Id_Perfil { get; set; }
       public int? Id_Servicio { get; set; }
       public DateTime? Fecha_Ingreso { get; set; }
       public string? Estado_Participante { get; set; }
       public string? Descripcion_Servicio { get; set; }
       public DateTime? Fecha_Inicio { get; set; }
       public DateTime? Fecha_Finalizacion { get; set; }
       public string? Visibilidad { get; set; }
       public string? Cargo { get; set; }
       public string? Descripcion { get; set; }
       public string? Estado { get; set; }
       public string? Nombre_Proyecto { get; set; }
   }
   
   public class ViewCalendarioByDependencia : EntityClass {
       public int? Id_Case { get; set; }
       public int? Id_TareaPadre { get; set; }
       public DateTime? Fecha_Inicio { get; set; }
       public DateTime? Fecha_Final { get; set; }
       public string? Estado { get; set; }
       public int? IdCalendario { get; set; }
       public int? Id_Tarea { get; set; }
       public int? Id_Dependencia { get; set; }
   }
   public class ViewActividadesParticipantes : EntityClass {
       public int? Id_Case { get; set; }
       public string? Titulo { get; set; }
       public string? Descripcion { get; set; }
       public string? Estado { get; set; }
       public int? Id_Perfil { get; set; }
   }
}
