//@ts-check
import { StylesControlsV2, StyleScrolls } from "../WDevCore/StyleModules/WStyleComponents.js";
import { WTableComponent } from "../WDevCore/WComponents/WTableComponent.js";
import { EntityClass } from "../WDevCore/WModules/EntityClass.js";
import { WRender } from "../WDevCore/WModules/WComponentsTools.js";
import { css } from "../WDevCore/WModules/WStyledRender.js";
import { Security_Users } from "../WDevCore/Security/SecurityModel.js";
// @ts-ignore
import { ModelProperty } from "../WDevCore/WModules/CommonModel.js";


/**
 * @typedef {Object} ComponentConfig
 * * @property {String} [Type]
 */
class LogView extends HTMLElement {
    /**
    * @param {ComponentConfig} [Config]
    */
    constructor(Config) {
        super();
        this.Config = Config
        this.Draw();
    }
    Draw = async () => {
        /**@type {Log} */
        this.ModelComponent = new Log();
        /**@type {Log} */
        this.EntityModel = new Log({ LogType: this.Config?.Type });
        if (LogType.ACTION == this.Config?.Type || LogType.INFO == this.Config?.Type) {
            this.ModelComponent.body.hidden = true;
        }
        if (LogType.ACTION == this.Config?.Type) {
            this.ModelComponent.Usuario.hidden = false;
            this.ModelComponent.Usuario.hiddenFilter = false;
        }
        //**@type {Array} */
        //this.Dataset = await this.EntityModel.Get();
        this.TabContainer = WRender.Create({ class: 'TabContainer' });
        this.MainComponent = new WTableComponent({
            ModelObject: this.ModelComponent,
            EntityModel: this.EntityModel,
            AutoSave: true,
            AddItemsFromApi: true,
            //Dataset: this.Dataset,
            Options: {
                Show: true,
                Filter: true,
                FilterDisplay: true
            }
        });
        this.TabContainer.append(this.MainComponent);
        this.append(
            StylesControlsV2.cloneNode(true),
            StyleScrolls.cloneNode(true),
            this.CustomStyle,
            this.TabContainer
        );
    };
    update = async () => {
        /**@type {Array|undefined} */
        const response = await this.EntityModel?.Get();
        this.MainComponent?.DrawTable(response);
    };
    CustomStyle = css`
         .component{
            display: block;
         }           
     `;
}
customElements.define('w-component-manager', LogView);
export { LogView };
class Log extends EntityClass {
    constructor(props) {
        super(props, 'Log');
        for (const prop in props) {
            this[prop] = props[prop];
        };
    }
    /**@type {ModelProperty} */
    Usuario = {
        type: 'WSelect', ModelObject: () => new Security_Users(),
        ForeignKeyColumn: "Id_User",
        hidden: true, hiddenFilter: true
    };
    Fecha = { type: 'Date' };
    message = { type: 'textarea' };
    body = { type: 'textarea', ModelObject: new ErrorEx() };
   

}
class ErrorEx {
    Source = { type: 'text' };
    StackTrace = { type: 'textarea' };
}

const LogType = {
    ERROR: "ERROR", INFO: "INFO", ACTION: "ACTION"
}
export { LogType };
