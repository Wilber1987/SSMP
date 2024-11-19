//@ts-check
import { StylesControlsV2, StylesControlsV3, StyleScrolls } from "../../../WDevCore/StyleModules/WStyleComponents.js";
import { ComponentsManager, WRender } from "../../../WDevCore/WModules/WComponentsTools.js";
import { css } from "../../../WDevCore/WModules/WStyledRender.js";
import { Tests_ModelComponent } from "../../FrontModel/ModelComponent/Tests_ModelComponent.js";
import { Tests } from "../../FrontModel/Tests.js";
// @ts-ignore
import { ModalMessege } from "../../../WDevCore/WComponents/WForm.js";
import { WModalForm } from "../../../WDevCore/WComponents/WModalForm.js";
// @ts-ignore
import { ModelProperty } from "../../../WDevCore/WModules/CommonModel.js";
import { WArrayF } from "../../../WDevCore/WModules/WArrayF.js";
import { Cat_Valor_Preguntas } from "../../FrontModel/Cat_Valor_Preguntas.js";
import { Pregunta_Tests } from "../../FrontModel/Pregunta_Tests.js";
import { Resultados_Pregunta_Tests } from "../../FrontModel/Resultados_Pregunta_Tests.js";
import { Resultados_Tests } from "../../FrontModel/Resultados_Tests.js";
/**
 * @typedef {Object} ComponentConfig
 * * @property {Object} [propierty]
 */
class ResolveTestViewComponent extends HTMLElement {
    /**
     * 
     * @param {ComponentConfig} props 
     */
    constructor(props) {
        super();
        
        this.OptionContainer = WRender.Create({ className: "OptionContainer" });
        this.TabContainer = WRender.Create({ className: "content-container", id: "content-container" });
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
        /**@type {Tests_ModelComponent} */
        this.ModelComponent = new Tests_ModelComponent();
        /**@type {Tests} */
        this.EntityModel = new Tests();
        /**@type {Array<Tests>} */
        this.Dataset = await this.EntityModel.Get();

        /**@type {Array} */
        this.DatasetCategories = WArrayF.GroupBy(this.Dataset.map(d => d.Cat_Categorias_Test), "Descripcion");
        this.DatasetCategories.forEach(test => {
            const testCategories = this.Dataset?.filter(test =>
                test.Cat_Categorias_Test.Descripcion == test.Cat_Categorias_Test.Descripcion
            );
            testCategories?.forEach(test => {

                this.TabContainer.append(this.TestCard(test));
            });
        });
    }


    /**
     * @param {Tests} test
     */
    BuildTest(test) {
        const model = {};
        test.Pregunta_Tests.forEach(p => {
            let type = "WRADIO";
            /**@type {ModelProperty} */
            const modelPropierty = {
                type: type,
                Dataset: p.Cat_Tipo_Preguntas.Cat_Valor_Preguntas,
                label: p.Descripcion_pregunta
            };
            model[p.Id_pregunta_test] = modelPropierty;
        });
        this.append(new WModalForm({
            ModelObject: model, ObjectOptions: {
                SaveFunction: async (/**@type {Object} */ editinObject) => {
                    /**@type {Array<Resultados_Pregunta_Tests>} */
                    await this.SaveTest(editinObject, test);
                }
            }
        }));
    }

    /**
     * @param {{ [x: string]: Cat_Valor_Preguntas; }} editinObject
     * @param {Tests} test
     */
    async SaveTest(editinObject, test) {
        const resultados = [];

        for (const prop in editinObject) {

            /**@type {Cat_Valor_Preguntas} */
            const pregunta = editinObject[prop];
            console.log(pregunta, prop);
            console.log(test);

            resultados.push(new Resultados_Pregunta_Tests({
                Id_Perfil: 1,
                Cat_Valor_Preguntas: pregunta,
                Pregunta_Tests: test.Pregunta_Tests?.find(p => p.Id_pregunta_test.toString() == prop),
                Fecha: new Date(),
                Resultado: pregunta.Valor,
                Respuesta: "",
                Estado: "ACTIVO"
            }));
        }

        /**@type {Array<Pregunta_Tests>} */
        const secciones = WArrayF.GroupBy(resultados.map(r => r.Pregunta_Tests), "Seccion");
        const Test = new Tests({ Resultados_Tests: [] });

        for (const pregntaTest of secciones) {
            const resultadosSeccion = resultados.filter(r => r.Pregunta_Tests.Seccion == pregntaTest.Seccion);
            const resultado = new Resultados_Tests({
                Id_Perfil: 1,
                Tests: test,
                Fecha: new Date(),
                Valor: WArrayF.SumValAtt(resultadosSeccion, "Resultado").toString(),
                Seccion: pregntaTest.Seccion,
                Resultados_Pregunta_Tests: resultadosSeccion,
                Tipo: pregntaTest.Seccion != null ? "SUB_ESCALA" : "ESCALA",
                Categoria_valor: "Por definir" //TODO DEFINIR EN BACKEND
            });
            Test.Resultados_Tests.push(resultado);
        }
        const response = await Test.SaveResultado();
        this.append(ModalMessege(response.message));
    }

    SetOption() {
        this.OptionContainer.append(WRender.Create({
            tagName: 'button', className: 'Block-Primary', innerText: 'Datos contrato',
            onclick: async () => this.Manager.NavigateFunction("id", undefined ?? WRender.Create({ className: "component" }))
        }))
    }

    CustomStyle = css`
        .component{
           display: block;
        }           
    `
    TestCard(/**@type { Tests } */ test) {
        return WRender.Create({
            className: "task", children: [
                {
                    className: "tags", children: [{
                        tagName: 'img', className: "img-cover",
                        src: "" + test.Image
                    }, {
                        className: "viewer", children: [
                            { tagName: "span", className: "tag", innerHTML: test.Titulo },
                            {
                                tagName: 'button', className: 'options', children:
                                    [WRender.CreateStringNode(`<div><svg xml:space="preserve" viewBox="0 0 41.915 41.916" xmlns:xlink="http://www.w3.org/1999/xlink" xmlns="http://www.w3.org/2000/svg" id="Capa_1" version="1.1" fill="#000000"><g stroke-width="0" id="SVGRepo_bgCarrier"></g><g stroke-linejoin="round" stroke-linecap="round" id="SVGRepo_tracerCarrier"></g><g id="SVGRepo_iconCarrier"><g><g><path d="M11.214,20.956c0,3.091-2.509,5.589-5.607,5.589C2.51,26.544,0,24.046,0,20.956c0-3.082,2.511-5.585,5.607-5.585 C8.705,15.371,11.214,17.874,11.214,20.956z"></path> <path d="M26.564,20.956c0,3.091-2.509,5.589-5.606,5.589c-3.097,0-5.607-2.498-5.607-5.589c0-3.082,2.511-5.585,5.607-5.585 C24.056,15.371,26.564,17.874,26.564,20.956z"></path> <path d="M41.915,20.956c0,3.091-2.509,5.589-5.607,5.589c-3.097,0-5.606-2.498-5.606-5.589c0-3.082,2.511-5.585,5.606-5.585 C39.406,15.371,41.915,17.874,41.915,20.956z"></path></g></g></g></svg></div>`)],
                                onclick: async () => {
                                    //code.....
                                }
                            }]

                    }]
                }, {
                    tagName: "label", className: "labelheader", innerHTML: test.Titulo
                }, {
                    tagName: "p", className: "", innerHTML: test.Descripcion
                }, {
                    className: "stats", children: [
                        [
                            WRender.CreateStringNode(`<div><svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24"><g stroke-width="0" id="SVGRepo_bgCarrier"></g><g stroke-linejoin="round" stroke-linecap="round" id="SVGRepo_tracerCarrier"></g><g id="SVGRepo_iconCarrier"> <path stroke-linecap="round" stroke-width="2" d="M12 8V12L15 15"></path> <circle stroke-width="2" r="9" cy="12" cx="12"></circle> </g></svg>
                                ${new Date(test.Fecha_publicacion).toLocaleDateString()}</div>`),
                            {
                                tagName: "a", innerHTML: "Resolver", onclick: async () => {
                                    this.BuildTest(test);
                                }
                            }
                        ]
                    ]
                }, css`
                .task {
                    position: relative;
                    color: #2e2e2f;
                    background-color: var(--secundary-color);
                    overflow: hidden;
                    border-radius: 8px;
                    box-shadow: rgba(99, 99, 99, 0.1) 0px 2px 8px 0px;
                    margin-bottom: 1rem;
                    border: 3px dashed transparent;
                    min-width: 340px;
                    border: solid #e6e6e6 1px;
                    min-height: 140px;
                    height: 160px;
                    display: grid;
                    grid-template-rows: 30px 35px 35px 40px;
                    gap: 5px;
                  }
                  .labelheader {
                    margin: 0px 10px;
                    display: block;
                    font-size: 12px;
                    font-weight: 600;
                    text-transform: capitalize;
                    z-index: 1;
                    color: #fff;
                    background-color: rgba(0, 0, 0, 0.3);
                    padding: 10px;
                    border-radius: 10px;
                  }

                  .task p {
                    font-size: 13px;
                    margin: 0px 10px;                    
                    text-overflow: ellipsis;
                    white-space: nowrap;
                    overflow: hidden;
                    z-index: 1;
                    color: #fff;
                    background-color: rgba(0, 0, 0, 0.3);
                    padding: 10px;
                    border-radius: 10px;
                  }
                  
                  .task:hover {
                    box-shadow: rgba(99, 99, 99, 0.3) 0px 2px 8px 0px;
                    border-color: rgba(162, 179, 207, 0.2) !important;
                  }
                  
                  .tag {
                    border-radius: 100px;
                    padding: 4px 13px;
                    font-size: 12px;
                    color: #ffffff;
                    background-color: #1389eb;
                    display: -webkit-box;
                    -webkit-line-clamp: 2; /* Limitar a 2 líneas */
                    -webkit-box-orient: vertical;
                    overflow: hidden;
                    text-overflow: ellipsis;
                    max-width: 300px;
                  }
                  
                  .tags {
                    width: 100%;
                    display: flex;
                    align-items: center;
                    justify-content: space-between;
                    position: relative;
                  }
                  .tags .img-cover {
                    position: absolute;
                    width: 100%;
                    height: 115px;
                    top: 0;
                    left: 0;   
                    object-fit: cover;                 
                  }
                  
                  .options {
                    background: transparent;
                    border: 0;
                    color: #c4cad3;
                    font-size: 17px;
                  }
                  
                  .options svg {
                    fill: #9fa4aa;
                    width: 20px;
                  }
                  
                  .stats {
                    position: relative;
                    width: -webkit-fill-available;
                    color: #9fa4aa;
                    font-size: 12px;
                    display: flex;
                    align-items: center;
                    justify-content: space-between;  
                    padding: 10px;                
                  }
                  .viewer {
                    justify-content: space-between;
                    display: flex;
                    width: -webkit-fill-available;
                    align-items: center;
                    z-index: 1;
                    margin: 10px;
                  }
                  .stats div {
                    margin-right: 1rem;
                    height: 20px;
                    display: flex;
                    align-items: center;
                    cursor: pointer;
                    width: 100%;
                  }
                  .stats div a{
                    transition: all 0.5s;
                  }
                  .stats div a:hover{
                    color: #2e2e2f;
                  }
                  
                  .stats svg {
                    margin-right: 5px;
                    height: 100%;
                    stroke: #9fa4aa;
                  }
                  
                  .viewer .img-participantes {
                    height: 30px;
                    width: 30px;
                    background-color: rgb(28, 117, 219);
                    margin-right: -10px;
                    border-radius: 50%;
                    border: 1px solid #fff;
                    display: grid;
                    align-items: center;
                    text-align: center;
                    font-weight: bold;
                    color: #fff;
                    padding: 2px;
                  }
                  
                  .viewer span svg {
                    stroke: #fff;
                  }                  
                    `
            ]
        });
    }
}
customElements.define('w-resolve-test-view', ResolveTestViewComponent);
export { ResolveTestViewComponent };
