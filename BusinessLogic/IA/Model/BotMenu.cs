using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppCore.Services;
using APPCORE;
using DocumentFormat.OpenXml.Office.CustomUI;
using DocumentFormat.OpenXml.Office2010.Excel;

namespace BusinessLogic.IA.Model
{
    public class BotMenu
    {
        [PrimaryKey]
        public int? Id { get; set; }
        public TypeLine? Type { get; set; }
        public string? Name { get; set; }
        public string? Code { get; set; }
        public string? Descripcion { get; set; }
        public string? CodeAdapter { get; set; }
        public string? Service { get; set; }
        public int? ServiceId { get; set; }
        public int? ParentMenuId { get; set; }
        public List<BotMenu> SubMenu { get; set; } = [];

        public static List<BotMenu> GetMainMenu()
        {
            return menuList.Where(m => m.ParentMenuId == 0).ToList();
        }


        public static (List<BotMenu>, bool Found) GetMenuByCode(string code, int? ParentMenuId)
        {
            if (ParentMenuId != 0 && ParentMenuId != null)
            {
                BotMenu? menu = GetMenuById(ParentMenuId);
                if (menu != null && menu.SubMenu != null && menu.SubMenu.Count != 0)
                {
                    return GetMenuByCode(code, menu.SubMenu);
                }
            }
            return GetMenuByCode(code, menuList, true);
        }

        private static (List<BotMenu> Menu, bool Found) GetMenuByCode(string code, List<BotMenu> menuList, bool isInitialList = false)
        {
            var menuByCode = menuList?.Find(m => StringUtil.NormalizeString(m.Code) == StringUtil.NormalizeString(code));

            if (menuByCode != null)
            {
                if (menuByCode.SubMenu == null || menuByCode.SubMenu.Count == 0)
                {
                    return (new List<BotMenu> { menuByCode }, true);
                }
                return (menuByCode.SubMenu, true);
            }

            if (isInitialList)
            {
                return (new List<BotMenu>(), false);
            }

            return (menuList ?? new List<BotMenu>(), false);
        }


        public static BotMenu? GetMenuById(int? id)
        {
            return FlattenMenuList(menuList).Where(m => m.Id == id)?.FirstOrDefault();
        }

        public static BotMenu GetMenuByName(string name)
        {
            return menuList.Where(m => m.Name?.Contains(name, StringComparison.OrdinalIgnoreCase) == true).First();
        }

        public static List<BotMenu> GetSub(string name)
        {
            return menuList.FirstOrDefault(m => m.Name?.Equals(name, StringComparison.OrdinalIgnoreCase) == true)?.SubMenu ?? new List<BotMenu>();
        }

        private static List<BotMenu> FlattenMenuList(List<BotMenu> menus)
        {
            List<BotMenu> flatList = new();
            void Flatten(BotMenu menu)
            {
                flatList.Add(menu);
                foreach (var subMenu in menu.SubMenu)
                {
                    Flatten(subMenu);
                }
            }

            foreach (var menu in menus)
            {
                Flatten(menu);
            }
            return flatList;
        }
        private static readonly List<BotMenu> menuList =
        [
          new BotMenu { Id = 1, ParentMenuId = 0, Code = "1", Name = "-",
                Descripcion = "Visítanos en el Palacio de Correos.", CodeAdapter = "INFORMACION_SOBRE_DOCUMENTOS",
                SubMenu = [
                    //SUB MENU 1
                    new BotMenu { Id = 1, ParentMenuId = 1, Name = "-",
                        Type = TypeLine.LINE,
                        Descripcion = "Ayúdanos a servirte mejor, tu eres… ", CodeAdapter = "INFORMACION_SOBRE_DOCUMENTOS" }, 
                    //OPCIONES  SUB MENU 1                             
                                                                              
                    new BotMenu { Id = 6, ParentMenuId = 1, Code = "a", Name = "-",
                        Descripcion = "El propietario del paquete", CodeAdapter = "INFORMACION_SOBRE_DOCUMENTOS" ,
                        SubMenu = [
                            //SUB MENU 6
                            new BotMenu { Id = 8, ParentMenuId = 6, Name = "-",
                                Type = TypeLine.LINE,
                                Descripcion = "Recuerda traer tu DPI, Numero de NIT, Factura o comprobante de compra (Si aplica). ¿Eres tú quién vendrá?", CodeAdapter = "INFORMACION_SOBRE_DOCUMENTOS" }, 
                            //OPCIONES  SUB MENU 6
                            
                            new BotMenu { Id = 9, ParentMenuId = 6, Code = "SI", Name = "-",
                                Descripcion = "", CodeAdapter = "RASTREO_Y_SEGUIMIENTOS",
                                SubMenu = [
                                    //SUB MENU 9
                                    new BotMenu { Id = 11, ParentMenuId = 9, Name = "DIRECCION",
                                        Type = TypeLine.RESPONSE,
                                        Descripcion = "Te esperamos en la oficina de Fardos Postales, ubicada en el Palacio de Correos, en 7ma avenida 12-11, Zona 1, 2do nivel, Of. 203. Ciudad Capital Guatemala Centro América.",
                                        CodeAdapter = "INFORMACION_SOBRE_DOCUMENTOS" },
                                    new BotMenu { Id = 12, ParentMenuId = 9, Name = "-",
                                        Type = TypeLine.LINE,
                                        Descripcion = "\nEscribe “MENU” para regresar", CodeAdapter = "INFORMACION_SOBRE_DOCUMENTOS" },
                            ]},
                            new BotMenu { Id = 10, ParentMenuId = 6, Code = "NO", Name = "-",
                                Descripcion = "", CodeAdapter = "RASTREO_Y_SEGUIMIENTOS" ,
                                SubMenu = [
                                    //SUB MENU 10
                                    new BotMenu { Id = 13, ParentMenuId = 10, Name = "RETIRO",
                                        Type = TypeLine.RESPONSE,
                                        Descripcion = @"Tercera Persona
* Envía una carta de autorización con el número de Tracking y que contenga el 
Numero de DPI del propietario del paquete y el número de DPI de la persona 
autorizada.  
* Nit del propietario  
* Comprobante de Compra del articulo (Si aplica) 
* Si eres familiar de la persona que envía el paquete, adjunta certificado de 
nacimiento. 
Si eres menor de 18 años 
* Fotocopia del DPI del padre o madre del menor de edad. 
* Certificado de nacimiento del menor de edad. 
* Comprobante de Compra del articulo (Si aplica). 
* Opcional. Los padres pueden autorizar a un tercero por medio de una carta de 
autorización. 
* Envía una carta de autorización del padre o madre.

  
Te esperamos en la oficina de Fardos Postales, ubicada en el Palacio de Correos, en 
7ma avenida 12-11, Zona 1, 2do nivel, Of. 203. Ciudad Capital Guatemala Centro 
América. ",
                                        CodeAdapter = "INFORMACION_SOBRE_DOCUMENTOS" },
                                     new BotMenu { Id = 14, ParentMenuId = 10, Name = "-",
                                        Type = TypeLine.LINE,
                                        Descripcion = "\nEscribe “MENU” para regresar", CodeAdapter = "INFORMACION_SOBRE_DOCUMENTOS" },
                                ]},
                        ]},
                    new BotMenu { Id = 7, ParentMenuId = 1, Code = "b", Name = "-",
                        Descripcion = "La empresa es el propietario", CodeAdapter = "INFORMACION_SOBRE_DOCUMENTOS",  SubMenu = [
                                    //SUB MENU 10
                                    new BotMenu { Id = 15, ParentMenuId = 7, Name = "-",
                                        Type = TypeLine.LINE,
                                        Descripcion = @"* Envía una carta de autorización con hoja membretada y firmada por el 
propietario o representante legal. 
* Fotocopia de la representación legal. 
* Fotocopia del DPI del representante legal y de la persona autorizada. 
* Nit de la empresa. 
* Factura de compra.
  
Te esperamos en la oficina de Fardos Postales, ubicada en el Palacio de Correos, en 
7ma avenida 12-11, Zona 1, 2do nivel, Of. 203. Ciudad Capital Guatemala Centro 
América. ",
                                        CodeAdapter = "INFORMACION_SOBRE_DOCUMENTOS" },
                                    new BotMenu { Id = 16, ParentMenuId = 7, Name = "-",
                                        Type = TypeLine.LINE,
                                        Descripcion = "\nEscribe “MENU” para regresar", CodeAdapter = "INFORMACION_SOBRE_DOCUMENTOS" },
                                ]},
                ]},
            new BotMenu { Id = 2, ParentMenuId = 0, Code = "2", Name = "-",
                Descripcion = "Utiliza nuestro servicio de desaduanaje remoto.", CodeAdapter = "INFORMACION_SOBRE_DOCUMENTOS" ,
                SubMenu = [
                    new BotMenu { Id = 17, ParentMenuId = 2, Name = "-",
                        Type = TypeLine.LINE,
                        Descripcion = @"* Gracias por confiar en nuestro nuevo servicio de desaduanaje remoto. 
- 1er. Paso descarga y completa el documento PDF.  
- 2do. Paso, envía el documento a este número de Whatsapp. ", CodeAdapter = "RASTREO_Y_SEGUIMIENTOS" },

                    new BotMenu { Id = 18, ParentMenuId = 2, Name = "PDF_SERVICIO_REMOTO",
                        Type = TypeLine.RESPONSE,
                        Descripcion = "https://chatbot.correos.gob.gt:8443/Media/Upload/proceso_via_whattsapp.pdf", CodeAdapter = "INFORMACION_SOBRE_DOCUMENTOS" },

                    new BotMenu { Id = 19, ParentMenuId = 2, Name = "-",
                        Type = TypeLine.LINE,
                        Descripcion = "Gracias te contactaremos", CodeAdapter = "INFORMACION_SOBRE_DOCUMENTOS" },
                ]},
            new BotMenu { Id = 3, ParentMenuId = 0, Code = "3", Name = "-",
                Descripcion = "Consulta el estado de tu paquete", CodeAdapter = "RASTREO_Y_SEGUIMIENTOS",
                SubMenu = [
                    new BotMenu { Id = 20, ParentMenuId = 3, Name = "-",
                        Type = TypeLine.LINE,
                        Descripcion = "Por favor digita el número de tracking del paquete", CodeAdapter = "RASTREO_Y_SEGUIMIENTOS" },
                    new BotMenu { Id = 22, ParentMenuId = 3, Name = "-",
                            Type = TypeLine.LINE,
                            Descripcion = "\nEscribe “MENU” para regresar", CodeAdapter = "INFORMACION_SOBRE_DOCUMENTOS" },
                ] },
            new BotMenu { Id = 4, ParentMenuId = 0, Code = "4", Name = "-",
                Descripcion = "Horario de atención en agencias. ", CodeAdapter = "CONSULTA_DE_CONTACTO",
                   SubMenu = [
                    new BotMenu { Id = 20, ParentMenuId = 4, Name = "DIRECCION",
                        Type = TypeLine.RESPONSE,
                        Descripcion = "Te esperamos en la oficina de Fardos Postales, ubicada en el Palacio de Correos, en 7ma avenida 12-11, Zona 1, 2do nivel, Of. 203. Ciudad Capital Guatemala Centro América.",
                        CodeAdapter = "INFORMACION_SOBRE_DOCUMENTOS" },
                    new BotMenu { Id = 21, ParentMenuId = 4, Name = "PDF_SERVICIO_DIRECCIONES_AGENCIAS",
                        Type = TypeLine.RESPONSE,
                        Descripcion = "https://chatbot.correos.gob.gt:8443/Media/Upload/DireccionesAgencias.pdf", CodeAdapter = "INFORMACION_SOBRE_DOCUMENTOS" },
                    new BotMenu { Id = 23, ParentMenuId = 4, Name = "-",
                        Type = TypeLine.LINE,
                        Descripcion = "\nEscribe “MENU” para regresar", CodeAdapter = "INFORMACION_SOBRE_DOCUMENTOS" },

                ]},
            new BotMenu { Id = 5, ParentMenuId = 0, Code = "5", Name = "-",
                Descripcion = "Comunicarme con un agente de servicio", CodeAdapter = "SOLICITUD_DE_ASISTENCIA",   SubMenu = [
                    new BotMenu { Id = 25, ParentMenuId = 5, Name = "SOLICITUD_DE_ASISTENCIA_RESPONSE",
                        Type = TypeLine.RESPONSE,
                        Descripcion = "Entendido. Te pondré en contacto con un agente de servicio al cliente para que puedan asistirte directamente. Por favor, espera un momento. ",
                        CodeAdapter = "SOLICITUD_DE_ASISTENCIA" },
                    new BotMenu { Id = 26, ParentMenuId = 5, Name = "-",
                        Type = TypeLine.LINE,
                        Descripcion = "\nEscribe “MENU” para regresar", CodeAdapter = "INFORMACION_SOBRE_DOCUMENTOS" },
                ]},
            new BotMenu { Id = 24, ParentMenuId = 0, Name = "-",
                Type = TypeLine.LINE,
                Descripcion = "\n¡Ingresa un número de opción!", CodeAdapter = "SOLICITUD_DE_ASISTENCIA" },
      ];

    }

    public enum TypeLine
    {
        OPTION, LINE, RESPONSE
    }
}