
//@ts-check
import { StylesControlsV2, StyleScrolls } from "../../../WDevCore/StyleModules/WStyleComponents.js";
import { WTableComponent } from "../../../WDevCore/WComponents/WTableComponent.js";
import { WRender } from "../../../WDevCore/WModules/WComponentsTools.js";
import { css } from "../../../WDevCore/WModules/WStyledRender.js";
import { Tests_ModelComponent } from "../../FrontModel/ModelComponent/Tests_ModelComponent.js";
import { Tests } from "../../FrontModel/Tests.js";

/**
 * @typedef {Object} ComponentConfig
 * * @property {Object} [propierty]
 */
class TestsViewManager extends HTMLElement {
    /**
    * @param {ComponentConfig} props
    */
    constructor(props) {
        super();
        this.Draw();
    }
    Draw = async () => {
        /**@type {Tests_ModelComponent} */
        this.ModelComponent = new Tests_ModelComponent();
        /**@type {Tests} */
        this.EntityModel = new Tests();
        /**@type {Array} */
        this.Dataset = await this.EntityModel.Get();
        this.TabContainer = WRender.Create({ class: "content-container" });
        this.MainComponent = new WTableComponent({
            ModelObject: this.ModelComponent,
            EntityModel: this.EntityModel,
            AutoSave: true,
            Dataset: this.Dataset, Options: {
                Add: true,
                Edit: true,
                Filter: true,
                //UserActions: [{ name: "action", action: (entity) => {/*action*/ }}]
            }
        });
        // this.FilterOptions = new WFilterOptions({
        //     Dataset: this.Dataset,
        //     AutoSetDate: true,
        //     ModelObject: this.ModelComponent,
        //     FilterFunction: (/** @type {Array | undefined} */ DFilt) => {
        //         this.MainComponent?.DrawTable(DFilt);
        //     }
        // });
        this.TabContainer.append(this.MainComponent);
        this.append(
            StylesControlsV2.cloneNode(true),
            StyleScrolls.cloneNode(true),
            this.CustomStyle,
            //this.FilterOptions,
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
customElements.define('w-component-tests-manager', TestsViewManager);
export { TestsViewManager };

