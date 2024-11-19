

//@ts-check
import { Tbl_Case_ModelComponent } from '../../FrontModel/Tbl_CaseModule.js';
import { StylesControlsV2, StylesControlsV3 } from '../../../WDevCore/StyleModules/WStyleComponents.js';
import { ColumChart, RadialChart } from '../../../WDevCore/WComponents/WChartJSComponents.js';
import { WFilterOptions } from '../../../WDevCore/WComponents/WFilterControls.js';
import { WModalForm } from '../../../WDevCore/WComponents/WModalForm.js';
import { PageType, WReportComponent } from '../../../WDevCore/WComponents/WReportComponent.js';
import { WTableComponent } from '../../../WDevCore/WComponents/WTableComponent.js';
import { WRender } from '../../../WDevCore/WModules/WComponentsTools.js';
import { css } from '../../../WDevCore/WModules/WStyledRender.js';
import { WArrayF } from "../../../WDevCore/WModules/WArrayF.js";
import { Tbl_Tareas_ModelComponent } from "../../FrontModel/Tbl_Tareas.js";

class CaseDashboardComponent extends HTMLElement {
    /**
     * @param {undefined} [Dataset]
     */
    constructor(Dataset) {
        //console.log(Dataset);
        super();
        this.Dataset = [];
        
        this.append(this.WStyle);
        this.OptionContainer = WRender.Create({ className: "options-container" });
        this.append(StylesControlsV2.cloneNode(true), StylesControlsV3.cloneNode(true));
        this.DrawCaseDashboardComponent();

    }
    connectedCallback() { }
    DrawCaseDashboardComponent = async () => {
        this.dashBoardView();
    }

    SetOptions = (/** @type {Array} */ casosMap,
        /** @type {Array} */ casosProcesados,
        /** @type {Array} */ casosEtiquetadosPorMes) => {

        this.OptionContainer.innerHTML = "";
        const TITLE_1 = "Casos por dependencia";
        this.OptionContainer.append(WRender.Create({
            tagName: 'input', type: 'button',
            className: 'Block-Primary', value: TITLE_1, onclick: () =>
                this.drawReport(
                    WArrayF.GroupArray(casosMap, ["Dependencia", "Estado"], ["casos"]),
                    TITLE_1
                )
        }))

        const TITLE_2 = "Frecuencia de solicitudes";
        this.OptionContainer.append(WRender.Create({
            tagName: 'input', type: 'button',
            className: 'Block-Primary', value: TITLE_2, onclick: () =>
                this.drawReport(
                    WArrayF.GroupArray(casosEtiquetadosPorMes, ["Mes", "Estado"], ["casos"]),
                    TITLE_2
                )
        }))       
    }
    drawReport = (/** @type {any[]} */ MapData, /**@type {String} */ title, /**@type {Object} */ model) => {
        console.log(this.FilterOptions?.FilterControls);
        this.append(new WModalForm({
            ObjectModal: new WReportComponent({
                Dataset: MapData,
                Logo: "/Media/img/logo.png",
                Header: title,
                SubHeader: `Del ${this.FilterOptions?.FilterControls[0].childNodes[0].value} al ${this.FilterOptions?.FilterControls[0].childNodes[1].value}`,
                PageType: PageType.OFICIO_HORIZONTAL,
                //ModelObject: model
            })
        }))
    }
    dashBoardView = async () => {
        this.Modelcase = new Tbl_Case_ModelComponent({});
        this.ModelTareas = new Tbl_Tareas_ModelComponent();
        const dasboardContainer = WRender.Create({
            className: "dashBoardView content-container",
            children: []
        });
        this.FilterOptions = new WFilterOptions({
            Dataset: this.Dataset,
            AutoSetDate: true,
            ModelObject: {
                Fecha_Inicio: { type: 'date' },
                Estado: { type: "Select", Dataset: ["Activo", "Espera", "Pendiente", "Finalizado"] }
            },
            Display: true,
            UseEntityMethods: false,
            FilterFunction: async (/** @type {any} */ FilterData) => {

                this.Dataset = await new Tbl_Case_ModelComponent({ FilterData: FilterData }).Get();
                this.TareasDataset = await new Tbl_Tareas_ModelComponent({ FilterData: FilterData }).Get();
                const { columChart,
                    radialChartDependencias,
                    columChartAperturaCasos,
                    columChartMonth,
                    radialChart } = this.buildCharts();
                const tableTareas = await this.taskData();
                dasboardContainer.innerHTML = "";
                dasboardContainer.append(tableTareas,
                    columChart,
                    radialChartDependencias,
                    columChartAperturaCasos,
                    columChartMonth,
                    radialChart);
            }
        });
        this.append(WRender.Create({ class: "header-container", children: [this.OptionContainer, this.FilterOptions] }), dasboardContainer);
        await this.FilterOptions.filterFunction();
    }
    WStyle = css`
        .DivContainer {
            display: flex;
            flex-direction: column;
            gap:20px;
        }
        .dashBoardView{
            display: grid;
            grid-template-columns: 37% 37% 23%;  
            grid-gap: 10px;
            grid-template-rows: 400px 400px;         
        }
        .dashBoardView #ColumnCasosPorDependencia { 
            grid-column: span 1;
        }
        .header-container{
            display: flex;
            gap: 20px;
        }
        w-filter-option {
                display: block;
            width: -webkit-fill-available;
        }
        .options-container {
            display: flex;
            flex-direction: column;
            gap: 5px
        }
    `

    async taskData() {
        const TareasMap = [];
        this.TareasDataset?.forEach(t => {
            t.Tbl_Participantes?.forEach((p) => {
                if (TareasMap.find(f => f.Id_Perfil == p.Id_Perfil) == null) {
                    const tp = this.TareasDataset?.filter(tf => tf.Tbl_Participantes?.filter((tpf) => tpf.Id_Perfil == p.Id_Perfil).length > 0);
                    TareasMap.push({
                        Id_Perfil: p.Id_Perfil,
                        Tecnico: (p.Tbl_Profile?.Nombres ?? "") + " " + (p.Tbl_Profile?.Apellidos ?? ""),
                        Proceso: tp?.filter(tf => tf.Estado == "Proceso").length,
                        Finalizado: tp?.filter(tf => tf.Estado == "Finalizado").length,
                        Espera: tp?.filter(tf => tf.Estado == "Espera").length
                    });
                }
            });
        });
        //console.log(TareasMap);
        const tableTareas = new WTableComponent({
            maxElementByPage: 8,
            ModelObject: {
                Id_Perfil: { type: "text", hidden: true },
                Tecnico: { type: "text" },
                Proceso: { type: "text" },
                Finalizado: { type: "text" },
                Espera: { type: "text" }
            },
            Dataset: TareasMap, Options: {}
        });
        return tableTareas;
    }

    buildCharts() {
        const casosMap = this.Dataset.map(x => {
            x.Dependencia = x.Cat_Dependencias?.Descripcion ?? "No asignado";
            x.casos = 1;
            return x;
        });
        const casosProcesados = this.Dataset.filter(c => !c.Estado.includes("Pendiente") && !c.Estado.includes("Solicitado")
        ).map(c => ({
            Estado: c.Estado,
            Servicio: c.Tbl_Servicios?.Descripcion_Servicio ??  "No asignado",
            Caso: "Caso",
            Mes: c.Fecha_Inicio.getMonthFormatEs(),
            casos: 1
        }));

        const casosEtiquetadosPorMes = this.Dataset.map(c => ({
            Estado: c.Estado,
            Caso: "Caso",
            Mes: `${c.Fecha_Inicio.getMonthFormatEs()} ${new Date(c.Fecha_Inicio).getFullYear()}`,
            casos: 1
        }));
        //console.log(casosEtiquetadosPorMes);
        this.SetOptions(casosMap, casosProcesados, casosEtiquetadosPorMes)

        const columChart = new ColumChart({
            // @ts-ignore
            Title: "Estado de los Casos por dependencia",
            Dataset: casosMap, percentCalc: true,
            EvalValue: "casos",
            AttNameEval: "Estado",
            groupParams: ["Dependencia"]
        });
        columChart.id = "ColumnCasosPorDependencia";

        /*this.OptionContainer.append(WRender.Create({
            tagName: 'input', type: 'button',
            className: 'Btn-Mini', value: "title1", onclick: () => {
                console.log(columChart.GroupsProcessData);
                this.drawReport(columChart.GroupsProcessData, "")
            }
        }))*/
        const radialChartDependencias = new RadialChart({
            // @ts-ignore
            Title: "Casos por dependencia",
            Dataset: WArrayF.GroupBy(casosMap, "Dependencia"), EvalValue: "count", AttNameEval: "Dependencia"
        });
        const radialChart = new RadialChart({
            // @ts-ignore
            Title: "Estado de los Casos",
            Dataset: WArrayF.GroupBy(this.Dataset, "Estado"), EvalValue: "count", AttNameEval: "Estado"
        });
        // const casosProcesadosAll = this.Dataset.map(c => ({
        //     Estado: c.Estado,
        //     Caso: "Caso",
        //     Mes: c.Fecha_Inicio.getMonthFormatEs(),
        //     casos: 1
        // }));
        const columChartMonth = new ColumChart({
            // @ts-ignore
            Title: "Cumplimiento del SLA por mes",
            //TypeChart: "Line",
            Dataset: casosProcesados,
            EvalValue: "casos",
            AttNameEval: "Estado",
            groupParams: ["Servicio", "Mes"]
        });
        const columChartAperturaCasos = new ColumChart({
            Title: "Frecuencia de solicitudes por mes",
            // @ts-ignore
            TypeChart: "Line",
            Dataset: casosEtiquetadosPorMes,
            EvalValue: "casos",
            AttNameEval: "Caso",
            groupParams: ["Mes"]
        });
        return { columChart, radialChartDependencias, columChartAperturaCasos, columChartMonth, radialChart };
    }
}
customElements.define('w-case-dasboard', CaseDashboardComponent);
export { CaseDashboardComponent };

