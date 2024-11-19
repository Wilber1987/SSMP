class TareaModel {
    constructor(Config){
        this.Id_TareaPadre = Config.Tareas.map(x => {
            return { id: x.Id_Tarea, desc: x.Titulo };
        });
        this.Participantes.Dataset = Config.Participantes.map(x => {
            x.id_ = x.IdUsuario;
            x.IdUsuario = x.IdUsuario;
            x.Descripcion = `${x.Nombres} ${x.Apellidos} - ${x.Mail}`;
            return x;
        })
    }
    Titulo = "";
    Descripcion = "";
    Estado = ["Activa", "Finalizado"];
    Id_TareaPadre = [];
    Participantes = {
        type: "MULTISELECT", Dataset: []
    };
    Evidencias = { type: "IMAGES" };
}
export { TareaModel }