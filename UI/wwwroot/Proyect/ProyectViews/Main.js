import { WSecurity } from '../../WDevCore/Security/WSecurity.js';
import { StyleScrolls } from "../../WDevCore/StyleModules/WStyleComponents.js";
import { WAppNavigator } from "../../WDevCore/WComponents/WAppNavigator.js";
import { WModalForm } from "../../WDevCore/WComponents/WModalForm.js";
import { WRender } from '../../WDevCore/WModules/WComponentsTools.js';

Aside.append(WRender.Create({ tagName: "h3", innerText: "Navegación General" }));
Aside.append(WRender.createElement({
    type: "w-app-navigator",
    props: {
        Inicialize: false,
        alignItems: "flex-end",
        DisplayMode: "right",
       
        Direction: "column",
        Elements: [
            {
                name: "Home", url: "#",
                action: async (ev) => {
                    window.location = location.origin + "/";
                }
            }, {
                name: "Gestión de Actividades", url: "#",
                action: async (ev) => {
                    window.location = location.origin + "/ProyectViews/Proyectos";
                }
            },{
                name: "Gestión de Perfil", url: "#",
                action: async (ev) => {
                    window.location = location.origin + "/ProyectViews/PerfilView";
                }
            },  {
                name: "Opciones", url: "#",
                action: (ev) => {
                    const ModalPerfil = new WModalForm({
                        title: "Eliminar",
                        ObjectModal: new WAppNavigator({
                            Direction: "column",
                            NavStyle: "tab",
                            Elements: [
                                {
                                    name: "Perfil", url: "#",
                                    action: (ev) => {
                                    }
                                }, {
                                    name: "Agenda", url: "#",
                                    action: (ev) => {
                                    }
                                }, {
                                    name: "Cerrar Sesión", url: "#",
                                    action: (ev) => {
                                        WSecurity.LogOut();
                                    }
                                }
                            ]
                        })
                    });
                    AdminTemplate.append(ModalPerfil);
                }
            }
        ]
    }
}));
AdminTemplate.append(WRender.createElement(StyleScrolls));