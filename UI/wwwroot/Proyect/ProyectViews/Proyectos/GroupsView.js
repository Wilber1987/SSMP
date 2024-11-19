//@ts-check

import { StylesControlsV2 } from "../../../WDevCore/StyleModules/WStyleComponents.js";
import { WAppNavigator } from "../../../WDevCore/WComponents/WAppNavigator.js";
import { ModalMessege, ModalVericateAction } from "../../../WDevCore/WComponents/WForm.js";
import { WModalForm } from "../../../WDevCore/WComponents/WModalForm.js";
import { WTableComponent } from "../../../WDevCore/WComponents/WTableComponent.js";
// @ts-ignore
import { FilterData, ModelProperty } from "../../../WDevCore/WModules/CommonModel.js";
import { EntityClass } from "../../../WDevCore/WModules/EntityClass.js";
import { ComponentsManager, html, WRender } from "../../../WDevCore/WModules/WComponentsTools.js";
import { css } from "../../../WDevCore/WModules/WStyledRender.js";
import { GroupState, Tbl_Grupo, Tbl_Grupo_ModelComponent, Tbl_Grupos_Profile, Tbl_Grupos_Profile_ModelComponent } from "../../FrontModel/Tbl_Grupo_ModelComponent.js";


class GroupView extends HTMLElement {
    /**
     * @param {Object} [Config= { MisGrupos: [], Grupos: [], Profile: new Tbl_Profile()}]
     * @property {Array} MisGrupos
     * @property {Array} Grupos
     * @property {Tbl_Profile} Profile
     */
    constructor(Config = { MisGrupos: [], Grupos: [] }) {
        super();
        this.Config = Config;
        this.attachShadow({ mode: 'open' });
        this.TabContainer = WRender.createElement({ type: 'div', props: { class: 'TabContainer', id: "TabContainer" } });
        this.DOMManager = new ComponentsManager({ MainContainer: this.TabContainer });
        this.shadowRoot?.append(
            this.Style,
            StylesControlsV2.cloneNode(true),
            this.GruposNav,
            this.TabContainer
        );
        this.DrawGroupView();
    }
    connectedCallback() { }
    DrawGroupView = async () => {

    }

    InvitarAGrupo() {
        document.body.append(new WModalForm({
            ModelObject: new Tbl_Grupos_Profile_ModelComponent({
                Tbl_Grupo: { type: "WSelect", ModelObject: new Tbl_Grupo_ModelComponent(), Dataset: this.Config.MisGrupos },
                Tbl_Profile: { type: "WSelect",  ModelObject: () =>  new Tbl_Profile_ModelComponent() }
            }),
            title: "Invitar Miembro",
            AutoSave: false,
            ObjectOptions: {
                SaveFunction: (/** @type {Tbl_Grupos_Profile} */ invitado) => {
                    invitado.Estado =  GroupState.INVITADO;
                    document.body.appendChild(ModalVericateAction(async () => {
                        const response = await new Tbl_Grupos_Profile(invitado).Save();
                        document.body.appendChild(ModalMessege(response.message, undefined, true));
                    }, "¿Desea invitar este usuario?"));
                }
            }
        }));
    }
    GroupDiv = (group = (new Tbl_Grupo()), type = "RECOMENDACION") => {
        const GrupoDiv = WRender.Create({
            className: "GroupDiv", children: [
                WRender.Create({ tagName: "h4", innerText: group.Nombre, style: { background: group.Color } }),
                WRender.Create({ tagName: "P", innerHTML: group.Descripcion }),
                //WRender.Create({ tagName: "label", innerText: "Tipo: " + group.Cat_Tipo_Grupo?.Descripcion }),
                WRender.Create({ tagName: "h5", innerText: "Instituciones" }),
                WRender.Create({ tagName: "h5", innerText: "Miembros" }),
                WRender.Create({
                    tagName: "div", children: group.Tbl_Grupos_Profiles?.filter(I => I.Estado == GroupState.ACTIVO).map(I => ({
                        tagName: 'img',
                        src: I.Tbl_Profile?.Foto,
                        title : I.Tbl_Profile?.Nombres
                    }))
                }),
            ]
        })
        //OPTIONS
        switch (type) {
            case "MIEMBRO":
                GrupoDiv.appendChild(WRender.Create({
                    className: "GroupOptions", children: [
                        group.Id_Perfil_Crea == this.Config.Profile.Id_Perfil ? "" : {
                            tagName: 'input', type: 'button', className: 'Btn', value: 'Abandonar grupo', onclick: async () => {
                                document.body.appendChild(ModalVericateAction(async () => {
                                    const response = await new Tbl_Grupos_Profile({
                                        Id_Grupo: group.Id_Grupo,
                                        Id_Perfil: this.Config.Profile.Id_Perfil, Estado: GroupState.INACTIVO
                                    }).Update();
                                    document.body.appendChild(ModalMessege(response.message, undefined, true));
                                }, "¿Desea abandonar este grupo? perderá acceso a los proyectos creado por otros miembros y los otros miembros del grupo no podrán acceder a los proyectos creados por usted."));
                            }
                        },
                        group.Id_Perfil_Crea == this.Config.Profile.Id_Perfil ? {
                            tagName: 'input', type: 'button', className: 'Btn', value: 'Dar de baja', onclick: async () => {
                                document.body.appendChild(ModalVericateAction(async () => {
                                    const response = await new Tbl_Grupo({
                                        Id_Grupo: group.Id_Grupo,
                                        Estado: GroupState.INACTIVO
                                    }).Update();
                                    document.body.appendChild(ModalMessege(response.message, undefined, true));
                                }, "¿Dar de baja este grupo? todos los miembros de este grupo perderan acceso a los proyectos creados por otros miembros y los otros miembros del grupo no podran acceder a los proyectos creados por usted."));
                            }
                        } : "",,
                        group.Id_Perfil_Crea == this.Config.Profile.Id_Perfil ? {
                            tagName: 'input', type: 'button', className: 'Btn', value: 'Editar', onclick: async () => {
                                this.EditarGrupo(group);
                            }
                        } : ""
                    ]
                }))
                break;
            case "RECOMENDACION":
                GrupoDiv.appendChild(WRender.Create({
                    className: "GroupOptions", children: [{
                        tagName: 'input', type: 'button', className: 'Btn', value: 'Unirse', onclick: async () => {
                            console.log(this.Config.Profile);
                            document.body.appendChild(ModalVericateAction(async () => {
                                const response = await new Tbl_Grupos_Profile({
                                    Id_Grupo: group.Id_Grupo,
                                    Id_Perfil: this.Config.Profile.Id_Perfil, Estado: GroupState.SOLICITANTE
                                }).Save();
                                document.body.appendChild(ModalMessege(response.message, undefined, true));
                            }, "¿Desea unirse esta grupo?"));
                        }
                    }]
                }))
                break;
        }
        return GrupoDiv;
    }
    GruposNav = new WAppNavigator({
        NavStyle: "tab",
        //title: "Menu",
        Inicialize: true,
        Elements: [
            {
                name: "MIS GRUPOS", url: "#",
                action: async (ev) => {
                    //const MisGrupos = await WAjaxTools.PostRequest("../api/Group/GetGruposInvestigador");
                    const GruposContainer = WRender.Create({ class: 'ViewProyectsContainer' });
                    this.Config.MisGrupos.forEach(group => {
                        GruposContainer.append(this.GroupDiv(group, "MIEMBRO"), this.Style.cloneNode(true), StylesControlsV2.cloneNode(true))
                    });
                    return GruposContainer;
                }
            }, {
                name: "RECOMENDADOS", url: "#",
                action: async (ev) => {
                    //const Grupos = await WAjaxTools.PostRequest("../api/Group/GetRecomendedGroups");
                    const GruposContainer = WRender.Create({ class: 'ViewProyectsContainer' });
                    this.Config.Grupos.forEach(group => {
                        GruposContainer.append(this.GroupDiv(group, "RECOMENDACION"), this.Style.cloneNode(true), StylesControlsV2.cloneNode(true))
                    });
                    return GruposContainer;
                }
            }, {
                name: "INVITACIONES", url: "#",
                action: async (ev) => {
                    //const Grupos = await WAjaxTools.PostRequest("../api/Group/GetRecomendedGroups");
                    const GruposContainer = WRender.Create({ class: 'ViewProyectsContainer' });
                    GruposContainer.append(html`<button class="BtnSuccess" onclick="${() => this.InvitarAGrupo()}">Invitar a grupo</button>`)
                    GruposContainer.append(new WTableComponent({
                        ModelObject: new Tbl_Grupos_Profile_ModelComponent(),
                        AddItemsFromApi: false,
                        Dataset: await new Tbl_Grupos_Profile({
                            Id_Perfil: this.Config.Profile.Id_Perfil,
                            Estado: "INVITADO"
                        }).Get(),
                        Options: {
                            UserActions: this.GetAceptarRechazarSolicitudes()
                        }
                    }));
                    return GruposContainer;
                }
            }, {
                name: "SOLICITUDES", url: "#",
                action: async (ev) => {
                    //const Grupos = await WAjaxTools.PostRequest("../api/Group/GetRecomendedGroups");
                    const GruposContainer = WRender.Create({ class: 'ViewProyectsContainer' });
                    GruposContainer.append(new WTableComponent({
                        ModelObject: new Tbl_Grupos_Profile_ModelComponent(),
                        AddItemsFromApi: false,
                        Dataset: await new Tbl_Grupos_Profile({
                            //Id_Perfil: this.Config.Profile.Id_Perfil,
                            Estado: "SOLICITANTE",
                            FilterData: [
                                new FilterData({
                                    PropName: "Id_Grupo",
                                    FilterType: "in",
                                    Values: this.Config.MisGrupos.map(I => I.Id_Grupo.toString())
                                })
                            ]
                        }).Get(),
                        Options: {
                            UserActions: this.GetAceptarRechazarSolicitudes()
                        }
                    }));
                    return GruposContainer;
                }
            }
        ]
    })
    Style = css`
        .ViewProyectsContainer,
        .ViewProyectsContainer {
            display: flex;
            flex-wrap: wrap;
        }

        .GroupDiv {
            width: 100%;
            padding: 0px;
            margin: 10px;
            box-shadow: 0 0 3px 0 rgba(0, 0, 0, 0.5);
            border-radius: 5px;
            display: flex;
            justify-content: flex-start;
            flex-direction: column;
            align-items: flex-start;
            color: #2e2e2e;
            background: #fff;
            text-align: justify;
            overflow: hidden;
        }

        .GroupDiv * {
            margin: 0px;
            padding: 10px 30px;
        }

        .GroupDiv h4 {
            margin: 0px;
            margin-bottom: 20px;
            padding: 20px;
            display: flex;
            width: calc(100% - 40px);
            justify-content: center;
            flex-direction: row;
            align-items: center;
            color: #fff;
            background: #468f04;
        }

        .GroupDiv img {
            padding: 0px;
            height: 50px;
            width: 50px;
            border-radius: 50%;
            margin: 10px;
            overflow: hidden;
            box-shadow: 0 0 3px 0 rgba(0, 0, 0, 0.5);
        }

        .GroupOptions {
            display: flex;
            width: calc(100% - 60px);
            justify-content: flex-end;
            flex-direction: row;
            align-items: center;
            border-top: solid 1px #999;
        }

        @media (max-width: 600px) {}
        `;

    GetAceptarRechazarSolicitudes() {
        return [
            {
                name: "Aceptar", action: (solicitud) => {
                    document.body.appendChild(ModalVericateAction(async () => {
                        solicitud.Estado = GroupState.ACTIVO;
                        const response = await solicitud.Update();
                        document.body.appendChild(ModalMessege(response.message, undefined, true));
                    }, "¿Desea aceptar esta solicitud?"));
                }
            }, {
                name: "Rechazar", action: (solicitud) => {
                    document.body.appendChild(ModalVericateAction(async () => {
                        solicitud.Estado = GroupState.RECHAZADO;
                        const response = await solicitud.Update();
                        document.body.appendChild(ModalMessege(response.message, undefined, true));
                    }, "¿Desea rechazar esta solicitud?"));
                }
            }
        ];
    }
    EditarGrupo(group) {
        this.shadowRoot?.append(new WModalForm({
            title: "Editar Grupo",
            AutoSave: true,
            ModelObject: new Tbl_Grupo_ModelComponent(),
            StyleForm: "columnX1",
            EditObject: group, 
            ObjectOptions: {
                SaveFunction: (ObjectF, response) => {
                    document.body.appendChild(ModalMessege(response.message, undefined, true));
                }
            }
        }));
    }
}
customElements.define('w-group_view', GroupView);
export { GroupView };


class Tbl_Profile_ModelComponent extends EntityClass {
    constructor(props) {
        super(props, 'EntityHelpdeskPublic');
        for (const prop in props) {
            this[prop] = props[prop];
        }
    }
    /**@type {ModelProperty}*/ Id_Perfil = { type: 'number', primary: true };
    /**@type {ModelProperty}*/ Nombres = { type: 'text' };
    /**@type {ModelProperty}*/ Apellidos = { type: 'text' };
    /**@type {ModelProperty}*/ Foto = { type: 'img', require: false };
    /**@type {ModelProperty}*/ Correo_institucional = { type: 'text', label: "correo", disabled: true, hidden: true };
    /** campos de investigaciones */
    //**@type {ModelProperty}*/ Tbl_Grupos_Profiles = { type: 'masterdetail', require: false , ModelObject: ()=> new Tbl_Grupos_Profiles_ModelComponent() };
    /**@type {ModelProperty}*/ ORCID = { type: 'text', require: false };
    //PROPIEDADES DE HELPDESK
   
}