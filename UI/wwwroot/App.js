import { AppDashboardComponentView } from "./Proyect/Dasboards/AppDashboardComponentView.js";
import { WRender } from "./WDevCore/WModules/WComponentsTools.js";
import { css } from "./WDevCore/WModules/WStyledRender.js";

const ColorsList = ["#044fa2", "#0088ce", "#f6931e", "#eb1c24", "#01c0f4", "#00bff3", "#e63da4", "#6a549f"];

const OnLoad = async () => {
   //const dep = await new Cat_Dependencias().Get();
    const container = WRender.Create({ className: "dep-container" });
    const mainComponent = new AppDashboardComponentView();
    container.appendChild(mainComponent);  
    // dep.forEach((element, index) => {
    //     const cont = WRender.Create({
    //         className: "card", style: { backgroundColor: ColorsList[index] }, children: [
    //             { tagName: "h1", innerText: element.Descripcion },
    //             { tagName: "label", innerText: element.Username },
    //             {
    //                 className: "cont-mini-cards", children: (element.Tbl_Servicios?.map(s =>
    //                     WRender.Create({
    //                         className: "mini-card", children:
    //                             [{ tagName: "IMG", src: "data:image/png;base64," + s.Cat_Tipo_Servicio.Icon },
    //                             s.Descripcion_Servicio]
    //                     }))) ?? []
    //             }
    //         ]
    //     })
    //     container.append(cont)
    // });
    Main.append( container);
}
const cssCus = css`
    .dep-container{
        display: grid;
        grid-template-columns: 30% 30% 30%;
        gap: 20px;
        padding: 20px;
    }
    .card {
        padding: 20px;
        background-color: #0e8bd9;
        display: grid;
        grid-template-rows: 40px 20px 70px;
        color: #fff;
        border-radius: 15px;
    }
    .card h1 {
        font-size: 24px;
        margin: 5px 0px;
    }
    .cont-mini-cards {
        width: 100%;
        display: flex;
        gap: 5px;
    }
    .mini-card {
        padding: 8px;
        background-color: var(--secundary-color);
        font-size: 11px;
        border-radius: 10px;
        color: var(--font-fourth-color);
        margin: 10px 0px;
        display: flex;
        height: 21px;
        align-items: center;
        gap:5px;
    }
    .mini-card label{
        display: block;
    }
    .mini-card img{ 
        height: 20px;
        width: 20px;
        object-fit: contain;
    }
    @media (max-width: 1300px){
        .dep-container{
            display: grid;
            grid-template-columns: 50% 50%;
            gap: 20px;
            padding: 20px;            
        }
        .cont-mini-cards {
            flex-direction: column;
        }
        .mini-card {
            padding: 8px;
            color: var(--font-fourth-color);
            margin: 10px 0px 0px 0px;
            height: 21px;
        }
        .card {
            padding: 20px;
            background-color: #0e8bd9;
            display: grid;
            grid-template-rows: 40px 20px auto;
            color: #fff;
            border-radius: 15px;
        }
    }
    @media (max-width: 800px){
        .dep-container{
            display: grid;
            grid-template-columns: 100%;
            gap: 20px;
            padding: 20px;
        }

    }
`
window.onload = OnLoad;