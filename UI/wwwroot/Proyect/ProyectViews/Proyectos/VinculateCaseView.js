//@ts-check
import { StylesControlsV2, StylesControlsV3, StyleScrolls } from "../../../WDevCore/StyleModules/WStyleComponents.js";
import { WForm } from "../../../WDevCore/WComponents/WForm.js";
import { WModalForm } from "../../../WDevCore/WComponents/WModalForm.js";
import { WTableComponent } from "../../../WDevCore/WComponents/WTableComponent.js";
import { ComponentsManager, WRender } from "../../../WDevCore/WModules/WComponentsTools.js";
import { css } from "../../../WDevCore/WModules/WStyledRender.js";
import { Tbl_VinculateCase } from "../../FrontModel/Tbl_CaseModule.js";
/**
 * @typedef {Object} ComponentConfig
 * * @property {Object} [propierty]
 */
class ComponentView extends HTMLElement {
    /**
     * 
     * @param {ComponentConfig} [props] 
     */
    constructor(props) {
        super();
        
        this.OptionContainer = WRender.Create({ className: "OptionContainer" });        
        this.TabContainer = WRender.Create({ className: "TabContainer", id: "content-container" });
        this.Manager = new ComponentsManager({ MainContainer: this.TabContainer, SPAManage: false });        
        this.append(this.CustomStyle);        
        this.append(
            StylesControlsV2.cloneNode(true),
            StyleScrolls.cloneNode(true),
            StylesControlsV3.cloneNode(true),
            this.OptionContainer,
            this.TabContainer
        );
        this.Draw();
    }
    Draw = async () => {
        this.SetOption();
    }
  

    SetOption() {
        this.OptionContainer.append(WRender.Create({
            tagName: 'button', className: 'Block-Primary', innerText: 'Datos 1',
            onclick: async () => {
                this.Manager.NavigateFunction("table", new WTableComponent({ ModelObject: new Tbl_VinculateCase()}))
            } 
        }))  
        this.OptionContainer.append(WRender.Create({
            tagName: 'button', className: 'Block-Primary', innerText: 'Datos 2',
            onclick: async () => {
                this.Manager.NavigateFunction("form", new WForm({ ModelObject: new Tbl_VinculateCase()}))
            } 
        }))      
        this.OptionContainer.append(WRender.Create({
            tagName: 'button', className: 'Block-Primary', innerText: 'Datos 2',
            onclick: async () => {
                //this.Manager.NavigateFunction("formv", new ComponentView({}))
                this.append(new WModalForm({ObjectModal: new ComponentView()}))
            } 
        })) 
    }
   
    CustomStyle = css`
        .component{
           display: block;
        }           
    `
}
customElements.define('w-component', ComponentView);
export { ComponentView };

window.onload = ()=>{
    const t = document.createElement("w-component");
   // @ts-ignore
   Main.append(new ComponentView()); 
   // @ts-ignore
   Main.append(t);
}