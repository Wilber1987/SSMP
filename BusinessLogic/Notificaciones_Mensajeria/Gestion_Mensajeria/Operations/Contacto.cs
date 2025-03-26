
using APPCORE;
using APPCORE.Security;
using DataBaseModel;

namespace CAPA_NEGOCIO.Gestion_Mensajeria
{
    public class Contacto
    {
        public string? Nombre_Completo { get; set; }
        public int? Id_User { get; set; }
        public string? Foto { get; set; }
        public List<FilterData>? filterData { get; set; }
        public List<OrdeData>? orderData { get; set; }
        public int Mensajes { get;  set; }
       
    }
}