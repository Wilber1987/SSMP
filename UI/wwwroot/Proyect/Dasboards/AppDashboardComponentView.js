//@ts-check
import { StylesControlsV2, StylesControlsV3, StyleScrolls } from "../../WDevCore/StyleModules/WStyleComponents.js";
import { ColumChart } from "../../WDevCore/WComponents/WChartJSComponents.js";
import { WFilterOptions } from "../../WDevCore/WComponents/WFilterControls.js";
import { WPaginatorViewer } from "../../WDevCore/WComponents/WPaginatorViewer.js";
import { ComponentsManager, html, WRender } from "../../WDevCore/WModules/WComponentsTools.js";
import { css } from "../../WDevCore/WModules/WStyledRender.js";
import { Cat_Dependencias_ModelComponent } from "../FrontModel/Cat_Dependencias.js";
import { Tbl_Case_ModelComponent } from "../FrontModel/Tbl_CaseModule.js";
import { Tbl_Comments_ModelComponent } from "../FrontModel/Tbl_Comments.js";
import { CaseDetailComponent } from "../ProyectViews/Proyectos/CaseDetailComponent.js";
import { TaskCard } from "../ProyectViews/Proyectos/TaskManager.js";
import { Dashboard } from "./Dashboard.js";
/**
 * @typedef {Object} ComponentConfig
 * * @property {Object} [propierty]
 */
class AppDashboardComponentView extends HTMLElement {
    /**
     * 
     * @param {ComponentConfig} props 
     */
    constructor(props) {
        super();        
        this.OptionContainer = WRender.Create({ className: "OptionContainer" });
        this.ButtonsContainer = WRender.Create({ className: "options-container" });
        this.OptionContainer.append(this.ButtonsContainer);
        this.TabContainer = WRender.Create({ className: "TabContainer", id: "content-container" });
        this.Manager = new ComponentsManager({ MainContainer: this.TabContainer, SPAManage: true });
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

    async SetOption() {       
        this.ButtonsContainer.append(WRender.Create({
            tagName: 'button', className: 'Block-Primary', innerText: 'Home',
            onclick: async () => this.Manager.NavigateFunction("id", await this.MainComponent())
        }))
        this.Manager.NavigateFunction("id", await this.MainComponent());


    }
    async MainComponent() {
        /**@type {Dashboard} */
        // @ts-ignore
        let data = await new Dashboard().GetDasboard({
            // @ts-ignore
            Desde: new Date().subtractDays(30).toISO(),
            // @ts-ignore
            Hasta: new Date().toISO()
        });
        const component = WRender.Create({ className: "dashboard-component" });

        if (this.FilterOptions == undefined) {
            this.FilterOptions = new WFilterOptions({
                Dataset: [],
                UseEntityMethods: false,
                Display: true,
                AutoSetDate: true,
                Direction: "row",
                ModelObject: {
                    FilterData: [],
                    // @ts-ignore
                    Fechas: { type: "DATE", defaultValue: new Date().toISO() },
                },
                FilterFunction: async (/** @type {Array | undefined} */ DFilt) => {
                    // @ts-ignore
                    data = await new Dashboard().GetDasboard({ Desde: DFilt[0]?.Values[0], Hasta: DFilt[0]?.Values[1] });
                    this.update(component, data);
                }
            });
            this.OptionContainer?.append(this.FilterOptions)
        }
        this.update(component, data);
        return component;
    }
    update(component, data) {
        component.innerHTML = "";
        const dependencies = WRender.Create({ className: "dashboard-dependencies" });
        const chartCase = WRender.Create({ className: "dashboard-chart-case" });
        const comment = WRender.Create({
            className: "dashboard-comment", children: [
                WRender.CreateStringNode(`<h2>Actividad</h2>`)
            ]
        });

        const caseList = WRender.Create({ className: "dashboard-caseList" });
        const task = WRender.Create({ className: "dashboard-task" });
        const taskContainer = WRender.Create({
            className: "dashboard-task-container", children: [
                WRender.CreateStringNode(`<h2>Tareas asignadas</h2>`), task
            ]
        });

        const dataCase = data.caseTickets.map(element => this.caseView(element));
        // @ts-ignore
        const paginator = new WPaginatorViewer({ Dataset: dataCase, maxElementByPage: 1 });
        caseList.append(paginator);
        // @ts-ignore
        data.caseTickets.forEach(t => t.Dependencia = t.Cat_Dependencias?.Descripcion ?? "Sin dependencia");
        chartCase.append(new ColumChart({
            Dataset: data.caseTickets,
            AttNameEval: "Estado",
            groupParams: ["Dependencia"],
            Title: "Estado de los casos",
            //TypeChart: "Line",
        }));

        data.comments.forEach(element => {
            comment.append(this.chatView(element));
        });

        data.task.forEach(element => {
            task.append(TaskCard(element, this.Manager));
        });
        dependencies.innerHTML = "";
        data.dependencies.forEach(element => {
            dependencies.append(this.dependencieCards(element));
        });

        if (this.OptionContainer.querySelector(".dashboard-dependencies") == null) {
            this.OptionContainer.append(dependencies);
        }

        component.append(chartCase, caseList, comment, taskContainer);
        //return { dependencies, chartCase, comment, caseList, taskContainer };
    }

    /**
     * @param {Cat_Dependencias_ModelComponent} element
     */
    dependencieCards(element) {
        const card = html`<div class="card-dependencia card-style">
            <div class="top-section">
                <div class="bottom-section">
                <span class="title">${element.Descripcion}</span>
                <span class="subtitle">${element.Username}</span>
                <div class="row row1">
                    <div class="item">
                        <span class="big-text">${element.NCasos}</span>
                        <span class="regular-text">Proceso</span>
                    </div>
                    <div class="item">
                        <span class="big-text">${element.NCasosFinalizados}</span>
                        <span class="regular-text">Finalizados</span>
                    </div>
                    <div class="item">
                        <span class="big-text">${element.Tbl_Dependencias_Usuarios?.length ?? 0}</span>
                        <span class="regular-text">Miembros activos</span>
                    </div>
                </div>
            </div>
        </div>`;
        card.append(css`@import url("/css/cardDependencias.css"); `);
        return card;
    }
    /**
     * @param {import("../FrontModel/Tbl_CaseModule.js").Tbl_Case_ModelComponent} element
     */
    caseView(element) {
        return WRender.Create({
            className: "case-detail blog-card", children: [
                html`<div class="meta">
                            <div class="photo" style="background-image: url(https://storage.googleapis.com/chydlx/codepen/blog-cards/image-1.jpg)"></div>
                            <ul class="details">
                                <li class="author">${element.Mail}</li>
                                <li class="date">${element.Fecha_Inicio}</li>                          
                            </ul>
                    </div>`, {
                    className: "description", children: [
                        { tagName: "h1", innerHTML: `CASO #${element.Id_Case} - ${element.Titulo}` },
                        // @ts-ignore
                        { tagName: "h2", innerHTML: element.Cat_Dependencias?.Descripcion, class: "h2" },
                        { tagName: "p", innerHTML: element.Descripcion },
                        {
                            tagName: "a", innerHTML: "Ver detalles", onclick: async () => {
                                const find = await new Tbl_Case_ModelComponent({ Id_Case: element.Id_Case }).Get()
                                const CaseDetail = new CaseDetailComponent(find[0]);
                                this.Manager.NavigateFunction("Detail" + element.Id_Case, CaseDetail)
                            }
                        },
                        css`.blog-card {
                            width: 100%;
                            display: flex;
                            flex-direction: column;
                            margin: 1rem auto;
                            box-shadow: 0 3px 7px -1px rgba(0, 0, 0, 0.1);
                            margin-bottom: 1.6%;
                            background: #fff;
                            line-height: 1.4;
                            font-family: Source Sans Pro;
                            border-radius: 5px;
                            overflow: hidden;
                            z-index: 0;
                            position: relative;
                            font-size: 11px;
                            height: 200px;
                            min-height: 150px;
                        }
                        .blog-card a {
                            color: #fff;
                            cursor: pointer;
                            position: absolute;
                            bottom: 5px;
                            right: 5px;
                            padding: 5px;
                            border-radius: 5px;
                            background-color: #5995fd;
                            transition: all 0.3s;
                        }
                        .blog-card a:hover {
                            background-color: #345b9e;
                        }
                        .blog-card:hover .photo {
                            transform: scale(1.3) rotate(3deg);
                        }
                        .blog-card .meta {
                            position: relative;
                            z-index: 0;
                            height: 200px;
                        }
                        .blog-card .photo {
                            position: absolute;
                            top: 0;
                            right: 0;
                            bottom: 0;
                            left: 0;
                            background-size: cover;
                            background-position: center;
                            transition: transform 0.2s;
                        }
                        .blog-card .details,
                        .blog-card .details ul {
                            margin: auto;
                            padding: 0;
                            list-style: none;
                        }
                        .blog-card .details {
                            position: absolute;
                            top: 0;
                            bottom: 0;
                            left: -100%;
                            margin: auto;
                            transition: left 0.2s;
                            background: rgba(0, 0, 0, 0.6);
                            color: #fff;
                            padding: 10px;
                            width: 100%;
                            font-size: 12px;
                            display: flex;
                            flex-direction: column;
                            justify-content: flex-start;
                        }
                        
                        .blog-card .details ul li {
                            display: inline-block;
                            list-style: none;
                        }
                        
                        
                        .blog-card .details .tags li {
                            margin-right: 2px;
                        }
                        .blog-card .details .tags li:first-child {
                            margin-left: -4px;
                        }
                        .blog-card .description {
                            padding: 1rem;
                            background: #fff;
                            position: relative;
                            z-index: 1;
                            overflow: hidden;
                        }
                        .blog-card .description h1,
                        .blog-card .description h2 {
                            font-family: Source Sans Pro;
                        }
                        .blog-card .description h1 {
                            line-height: 1;
                            margin: 0;
                            font-size: 13px;
                        }
                        .blog-card .description .h2 {
                            font-size: 12px;
                            font-weight: 300;
                            text-transform: uppercase;
                            color: #a2a2a2;
                            margin: 5px 0px;
                            border-bottom: #357aa5 solid 1px;

                        }
                        .blog-card .description .read-more {
                            text-align: right;
                        }
                        .blog-card .description .read-more a {
                            color: #5ad67d;
                            display: inline-block;                            
                        }
                        .blog-card .description .read-more a:after {
                            content: "";
                            font-family: FontAwesome;
                            margin-left: -10px;
                            opacity: 0;
                            vertical-align: middle;
                            transition: margin 0.3s, opacity 0.3s;
                        }
                        .blog-card .description .read-more a:hover:after {
                            margin-left: 5px;
                            opacity: 1;
                        }
                        .blog-card p {
                            position: relative;
                            margin: 5px 0 0;
                            text-overflow: ellipsis;
                            white-space: nowrap;
                            overflow: hidden;
                            max-height: 120px;
                            padding: 0 10px;
                            font-size: 9px !important;
                            text-transform: lowercase !important;
                            overflow-y: auto;
                        }  
                        .blog-card p *:not(img):not(style) { 
                            width: 90% !important;
                            display: flex;
                            font-size: 12px !important;
                            flex-direction: column;
                            background: none !important;
                            margin: 0 !important;
                            padding: 0 !important;
                            position: relative !important;
                            gap:2px;
                        }
                        .blog-card p style { 
                            width: unset !important;
                        }
                        .blog-card p:first-of-type:before {
                            content: "";
                            height: 5px;
                            display: block;
                            background: #5ad67d;
                            width: 35px;
                            border-radius: 3px;
                            margin-bottom:5px;
                        }
                        .blog-card:hover .details {
                            left: 0%;
                        }
                        @media (max-width: 1400px) {
                            .dashboard-component {
                                grid-template-columns: 400px calc(100% - 420px);
                            }
                            .dashboard-task-container {
                                grid-column: span 1;
                            }
                        }
                        @media (min-width: 640px) {
                            .blog-card {
                                flex-direction: row;
                            }
                            .blog-card .meta {
                                flex-basis: 20%;
                                height: auto;
                            }
                            .blog-card .description {
                                flex-basis: 80%;
                            }                            
                            .blog-card.alt {
                                flex-direction: row-reverse;
                            }
                            .blog-card.alt .description:before {
                                left: inherit;
                                right: -10px;
                                transform: skew(3deg);
                            }
                            .blog-card.alt .details {
                                padding-left: 25px;
                            }
                        }
                        `
                    ]
                }


            ]
        });
    }

    /**
     * @param {Tbl_Comments_ModelComponent} element
     */
    chatView(element) {
        return WRender.Create({
            className: "case-dependencie cookieCard", children: [
                { tagName: "p", className: "cookieHeading", innerHTML: `Mensaje de: ${element.NickName} CASO: #${element.Id_Case}` },
                // { tagName: "p", className: "cookieDescription", innerHTML: element.Body?.replaceAll("<br>", "") ?? "adjunto" },
                {
                    tagName: 'input', type: 'button', className: 'acceptButton', value: 'ver', onclick: async () => {
                        //const find = await new Tbl_Tareas({ Id_Tarea: element.Id_Tarea }).Get()
                        //const CaseDetail = new TareaDetailView({ Task: find[0] });
                        //this.Manager.NavigateFunction("Detail" + element.Id_Tarea, CaseDetail)
                        // @ts-ignore
                        const find = await new Tbl_Case_ModelComponent({ Id_Case: element.Id_Case }).Get()
                        const CaseDetail = new CaseDetailComponent(find[0]);
                        this.Manager.NavigateFunction("Detail" + element.Id_Case, CaseDetail)
                    }
                }, css`
                        .cookieCard {
                            width: 300px;
                            height:120px;
                            min-height:80px;                           
                            display: flex;
                            flex-direction: column;
                            align-items: flex-start;
                            justify-content: flex-start;
                            gap: 5px;
                            padding: 10px;
                            position: relative;
                            overflow: hidden;
                            position: relative;
                            border-radius: 5px;
                            background-color: var(--secundary-color);
                            padding: 1rem;
                            border-radius: 8px;
                            box-shadow: rgba(99, 99, 99, 0.1) 0px 2px 8px 0px;
                            border: solid 1PX #e1e1e1;
                        }

                        .cookieCard::before {
                            opacity: 0.5;
                            width: 150px;
                            height: 150px;
                            content: "";
                            background: linear-gradient(to right,rgb(142, 110, 255),rgb(208, 195, 255));
                            position: absolute;
                            z-index: 1;
                            border-radius: 50%;
                            right: -25%;
                            top: -25%;
                        }

                        .cookieHeading {
                            font-size: 12px;
                            padding: 0px;
                            margin: 5px;
                            font-weight: 600;
                            color: #505050;
                            z-index: 2;
                        }

                        .cookieDescription {
                            font-size: 0.9em;
                            color:#505050;
                            z-index: 2;
                            padding: 0px;
                            margin: 0px;
                            text-overflow: ellipsis;
                            white-space: nowrap;
                            overflow: hidden;
                            width: calc(100% - 40px);
                        }

                        .cookieDescription *{
                            appearance: none;
                            margin: 0px;
                            padding: 0px;
                            text-overflow: ellipsis;
                            white-space: nowrap;
                            overflow: hidden;
                        }

                        .cookieDescription a {
                            color: rgb(241, 241, 241);
                            }

                        .acceptButton {
                            padding: 5px 10px;
                            background-color: #7b57ff;
                            transition-duration: .2s;
                            border: none;
                            color: rgb(241, 241, 241);
                            cursor: pointer;
                            font-weight: 600;
                            z-index: 2;
                            position: absolute;
                            right: 10px;
                            bottom: 10px;
                            width: 100px !important;
                        }

                        .acceptButton:hover {
                            background-color: #714aff;
                            transition-duration: .2s;
                            }
                    `
            ]
        });
    }
    
    CustomStyle = css`
        .OptionContainer {
            display: grid;
            grid-template-columns: 100px 530px auto;
            gap: 20px;
            margin-bottom: 10px;
            padding-bottom: 10px;
        }
        .dashboard-component{
           display: grid;
           grid-template-columns: 400px calc(100% - 800px) 350px;
           grid-template-rows: 335px 270px;
           gap: 20px;
        }  
        w-filter-option {
            min-width: 440px;
        }
        .dashboard-component h2{
            font-size:16px;
            margin: 8px 0px;
        }       
        .dashboard-dependencies,
        .dashboard-comment,
        .dashboard-caseList,
        .dashboard-task {
            display: flex;
            gap: 5px; 
            flex-direction: column;
            height: 100%;
            overflow: auto;
            max-height: 350px;
        } 
        w-paginator {
            height: 350px;
        }
        .dashboard-task-container {
            grid-column: span 2;
        }
        .dashboard-dependencies{
            flex-direction: row;
        }   
        .dashboard-comment {
            grid-row: span 2;
            max-height: 650px;
        } 
        .dashboard-task{
            flex-direction: row;
            flex-wrap: wrap;
            max-height: 240px;
        }    
        .dashboard-comment h2 {
            position: sticky;
            position: -webkit-sticky;
            top: 0; /* required */
            z-index: 3;
            background-color: var(--secundary-color);
        }      
        @media screen and (max-width: 900px) {
            .OptionContainer {
                display: flex;
                flex-direction: column;
               
            }
            .dashboard-component {
                grid-template-columns: 100%;
                grid-template-rows: auto;
            }
        }  
    `
}
customElements.define('w-app-dasboard', AppDashboardComponentView);
export { AppDashboardComponentView };
