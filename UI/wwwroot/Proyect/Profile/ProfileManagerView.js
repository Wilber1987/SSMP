//@ts-check
import { StylesControlsV2, StylesControlsV3 } from '../../WDevCore/StyleModules/WStyleComponents.js';
import { WFilterOptions } from '../../WDevCore/WComponents/WFilterControls.js';
import { ModalMessege, ModalVericateAction } from '../../WDevCore/WComponents/WForm.js';
import { WModalForm } from '../../WDevCore/WComponents/WModalForm.js';
import { WTableComponent } from '../../WDevCore/WComponents/WTableComponent.js';
import { ComponentsManager, WRender } from '../../WDevCore/WModules/WComponentsTools.js';
import { Cat_Dependencias_ModelComponent } from "../FrontModel/Cat_Dependencias.js";
import { Tbl_Profile } from '../FrontModel/Tbl_Profile.js';
import { Tbl_Servicios_ModelComponent } from '../FrontModel/Tbl_Servicios.js';
import { activityStyle } from '../style.js';

const OnLoad = async () => {
    const Dataset = await new Tbl_Profile().Get();
    const ProfilesComp = new PerfilManagerComponent(Dataset);
    // @ts-ignore
    Main.appendChild(ProfilesComp);
}
window.onload = OnLoad;
class PerfilManagerComponent extends HTMLElement {
    /**
    * @param {Array<Tbl_Profile>} [Dataset] 
    */
    constructor(Dataset) {
        super();
        this.updateDataset(Dataset);
        this.Dataset = Dataset;
     
        this.append(this.WStyle, StylesControlsV2.cloneNode(true), StylesControlsV3.cloneNode(true));
        this.TabContainer = WRender.createElement({ type: 'div', props: { class: "content-container", id: "TabContainer" } });
        this.TabManager = new ComponentsManager({ MainContainer: this.TabContainer });
        this.OptionContainer = WRender.Create({ className: "OptionContainer" });
        this.OptionContainer2 = WRender.Create({ className: "OptionContainer" });
        this.ModelObject = new Tbl_Profile({});
        this.Draw();
    }
    updateDataset(Dataset) {
        Dataset?.forEach(d => {
            d.Tbl_Dependencias_Usuarios = d.Tbl_Dependencias_Usuarios?.map(dp => ({
                Id_Dependencia: dp.Cat_Dependencias.Id_Dependencia,
                Descripcion: dp.Cat_Dependencias.Descripcion
            }));
            d.Tbl_Servicios_Profile = d.Tbl_Servicios_Profile?.map(dp => dp.Tbl_Servicios);
        });
    }
    connectedCallback() { }
    Draw = async () => {
        this.OptionContainer.append(WRender.Create({
            tagName: 'input', type:
                'button', className: 'Block-Alert', value: 'Gestión de perfiles', onclick: this.perfilManagerComponent
        }))
        this.UserActions.forEach(element => {
            this.OptionContainer2.append(WRender.Create({
                tagName: 'input', type: 'button', className: 'Btn',
                value: element.name, onclick: element.action
            }))

        });
        this.append(this.OptionContainer, this.TabContainer);
        this.perfilManagerComponent();
    }

    perfilManagerComponent = async () => {
        this.mainTable = new WTableComponent({
            Dataset: this.Dataset,
            AddItemsFromApi: false,
            AutoSave: true,
            ModelObject: this.ModelObject, CustomStyle: StylesControlsV2,
            Options: {
                MultiSelect: true,
                Show: true,
                Edit: true
            }
        });
        this.FilterOptions = new WFilterOptions({
            // @ts-ignore
            Dataset: this.Dataset,
            AutoSetDate: false,
            ModelObject: this.ModelObject,
            UseEntityMethods: true,
            FilterFunction: (DFilt) => {
                this.mainTable?.DrawTable(DFilt);
            }
        });
        this.TabManager.NavigateFunction("Tab-perfil-Manager",
            WRender.Create({
                className: "perfilView", children:
                    [this.FilterOptions, this.OptionContainer2, this.mainTable]
            }));
    }
    update = async () => {
        const Dataset = await new Tbl_Profile().Get();
        this.updateDataset(Dataset);
        this.Dataset = Dataset;
        // @ts-ignore
        this.mainTable.selectedItems = [];
        this.mainTable?.DrawTable(this.Dataset);
    }
    UserActions = [{
        name: "Asignar a dependencia", action: async () => {
            // @ts-ignore
            if (this.mainTable.selectedItems.length <= 0) {
                this.append(ModalMessege("Seleccione perfiles"));
                return;
            }
            //const dependencias = await new Cat_Dependencias().Get();
            const modal = new WModalForm({
                ModelObject: new Tbl_Profile({
                    Nombres: { type: "text", hidden: true },
                    Apellidos: { type: "text", hidden: true },
                    FechaNac: { type: "text", hidden: true },
                    Sexo: { type: "text", hidden: true },
                    Foto: { type: "text", hidden: true },
                    DNI: { type: "text", hidden: true },
                    Correo_institucional: { type: "text", hidden: true },
                    Estado: { type: "text", hidden: true },
                    Tbl_Dependencias_Usuarios: { type: 'Wselect', ModelObject: () => new Cat_Dependencias_ModelComponent() },
                    Tbl_Servicios: { type: 'Wselect', ModelObject: () => new Tbl_Servicios_ModelComponent(), hidden: true }
                }), ObjectOptions: {
                    SaveFunction: async (profile) => {
                        this.append(ModalVericateAction(async () => {
                            const response =
                                // @ts-ignore
                                await new Tbl_Profile().AsignarDependencias(this.mainTable?.selectedItems,
                                    profile.Tbl_Dependencias_Usuarios);
                            if (response.status == 200) {
                                this.append(ModalMessege("Asignación Correcta"));
                                this.update();
                            } else {
                                this.append(ModalMessege("Error"));
                            }
                            modal.close();
                        }, "Esta seguro que desea asignar a esta dependencia"))
                    }
                }
            });
            this.append(modal);
        }
    }
    ]


    WStyle = activityStyle.cloneNode(true)

    mapCaseToPaginatorElement(Dataset) {
        return Dataset.map(actividad => {
            actividad.Dependencia = actividad.Cat_Dependencias.Descripcion;
            //actividad.Progreso = actividad.Tbl_Tareas?.filter(tarea => tarea.Estado?.includes("Finalizado")).length;
            //return this.perfilElement(actividad);
        });
    }
}

customElements.define('w-perfil-manager-component', PerfilManagerComponent);
export { PerfilManagerComponent };


