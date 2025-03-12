//@ts-check
import { StylesControlsV2, StyleScrolls } from "../WDevCore/StyleModules/WStyleComponents.js";
import { WFilterOptions } from "../WDevCore/WComponents/WFilterControls.js";
import { WModalForm } from "../WDevCore/WComponents/WModalForm.js";
import { WTableComponent } from "../WDevCore/WComponents/WTableComponent.js";
import { EntityClass } from "../WDevCore/WModules/EntityClass.js";
import { WRender } from "../WDevCore/WModules/WComponentsTools.js";
class Transactional_ConfiguracionesView extends HTMLElement {
    
    constructor(props) {
        super();
        this.Draw();
    }
    Draw = async () => {
        const model = new Transactional_Configuraciones();
        const dataset = await model.Get();
        this.TabContainer = WRender.createElement({ type: 'div', props: { class: "content-container", id: "content-container" } })
        this.MainComponent = new WTableComponent({
            ModelObject: model, Dataset: dataset, Options: {
                UrlUpdate: "../api/ApiEntityADMINISTRATIVE_ACCESS/updateTransactional_Configuraciones",
                Search: true, UserActions: [
                    {
                        name: "Editar", action: (element) => {
                            this.append(new WModalForm({
                                AutoSave: true,
                                ModelObject: new Transactional_Configuraciones({
                                    Valor: { type: this.ConfigType(element), Dataset : this.GetDataset(element)}
                                }),
                                EditObject: element, ObjectOptions: {
                                    SaveFunction: () => {
                                        window.location.reload();
                                    }
                                }
                            }))
                        }
                    }
                ]

            }
        })
        this.TabContainer.append(this.MainComponent)
        this.FilterOptions = new WFilterOptions({
            Dataset: dataset,
            ModelObject: model,
            FilterFunction: (DFilt) => {
                this.MainComponent?.DrawTable(DFilt);
            }
        });
        this.append(
            StylesControlsV2.cloneNode(true),
            StyleScrolls.cloneNode(true),
            this.FilterOptions,
            this.TabContainer
        );
    }

    ConfigType(element) {
        if (this.IsImage(element)) {
            return "IMG";
        } else if (this.IsDrawImage(element)) {
            return "DRAW";
        } else if (this.IsNumber(element)) {
            return "NUMBER";
        } else if (this.IsSelectRadio(element)) {
            return "RADIO";
        }
        return "TEXTAREA"
    }
    IsSelectRadio(element) {
        return element.Tipo_Configuracion == "RADIO" || element.Tipo_Configuracion == "SELECT"
    }
    GetDataset(element) {
        if (this.IsSelectRadio(element)) {
            if (element.Nombre == "AUTOMATIC_SENDER_REPORT") {
                return ["AUTOMATICO", "MANUAL"];
            }
        }
        return undefined;
    }

    IsNumber(element) {
        return element.Tipo_Configuracion == "INTERESES" ||
            element.Tipo_Configuracion == "BENEFICIOS" ||
            element.Tipo_Configuracion == "NUMBER";
    }
    IsDrawImage(element) {
        return element.Nombre == "FIRMA_DIGITAL_APODERADO" ||
            element.Nombre == "FIRMA_DIGITAL_APODERADO_VICEPRESIDENTE"
    }
    IsImage(element) {
        return element.Nombre == "LOGO" || element.Tipo_Configuracion == "IMAGE";
    }
}
customElements.define('w-transactional_configuraciones', Transactional_ConfiguracionesView);
export { Transactional_ConfiguracionesView };

class Transactional_Configuraciones extends EntityClass {
    constructor(props) {
        super(props, 'EntityADMINISTRATIVE_ACCESS');
        for (const prop in props) {
            this[prop] = props[prop];
        }
    }
    Id_Configuracion = { type: 'number', primary: true };
    Nombre = { type: 'text', disabled: true };
    Descripcion = { type: 'text', disabled: true };
    Valor = { type: 'text' };
    Tipo_Configuracion = { type: 'text' , disabled: true };
}
export { Transactional_Configuraciones };

