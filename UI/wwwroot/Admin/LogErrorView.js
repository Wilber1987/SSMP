//@ts-check
import { StylesControlsV2, StyleScrolls } from "../WDevCore/StyleModules/WStyleComponents.js";
import { WTableComponent } from "../WDevCore/WComponents/WTableComponent.js";
import { WFilterOptions } from "../WDevCore/WComponents/WFilterControls.js";
import { css } from "../WDevCore/WModules/WStyledRender.js";
import { WRender } from "../WDevCore/WModules/WComponentsTools.js";
import { EntityClass } from "../WDevCore/WModules/EntityClass.js";


/**
 * @typedef {Object} ComponentConfig
 * * @property {Object} [propierty]
 */
class LogErrorView extends HTMLElement {
    /**
    * @param {ComponentConfig} [props]
    */
    constructor(props) {
        super();
        this.Draw();
    }
    Draw = async () => {
        /**@type {LogError} */
        this.ModelComponent = new LogError();
        /**@type {LogError} */
        this.EntityModel = new LogError();
        /**@type {Array} */
        this.Dataset = await this.EntityModel.Get();
        this.TabContainer = WRender.Create({ class: "content-container" });
        this.MainComponent = new WTableComponent({
            ModelObject: this.ModelComponent,
            EntityModel: this.EntityModel,
            AutoSave: true,
            Dataset: this.Dataset, Options: {
                Show: true
            }
        });
        this.FilterOptions = new WFilterOptions({
            Dataset: this.Dataset,
            AutoSetDate: true,
            ModelObject: this.ModelComponent,
            FilterFunction: (/** @type {Array | undefined} */ DFilt) => {
                this.MainComponent?.DrawTable(DFilt);
            }
        });
        this.TabContainer.append(this.MainComponent);
        this.append(
            StylesControlsV2.cloneNode(true),
            StyleScrolls.cloneNode(true),
            this.CustomStyle,
            this.FilterOptions,
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
customElements.define('w-component-manager', LogErrorView);
export { LogErrorView };
class LogError extends EntityClass {
    constructor(props) {
        super(props, 'Log');
        for (const prop in props) {
            this[prop] = props[prop];
        }
    }
    message = { type: 'textarea'};
    body = { type: 'textarea', ModelObject: new ErrorEx()};
    Fecha= { type: 'Date'};
}
class ErrorEx{
    Source= { type: 'text'};
    StackTrace=  { type: 'textarea'};
}