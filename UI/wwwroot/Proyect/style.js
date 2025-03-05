import { css } from "../WDevCore/WModules/WStyledRender.js"

const activityStyle = css`
@import url(/WDevCore/StyleModules/css/scrolls.css);
@import url(/WDevCore/StyleModules/css/variables.css);
@import url(/WDevCore/StyleModules/css/form.css);
.dashBoardView{
    display: grid;
    grid-template-columns: auto auto ;  
    grid-gap: 20px  
}
.OptionContainer {
    margin: 0 0 20px 0;
}
.dashBoardView w-colum-chart { 
    grid-column: span 2;
}
.actividadDetail{
    container-type: inline-size;
    container-name: details;
    max-height: 240px;
}
.actividad {
    border: 1px solid var(--fourth-color);
    padding: 15px;
    margin-bottom: 10px;   
    color: var(--font-primary-color);
    border-radius: 15px;
    gap: 10px;
    display: flex;
    flex-direction: column;    
    background-color: var(--secundary-color);
    display: grid;
    grid-template-columns: calc(100% - 210px) 200px;

}
.actividad h4 {
    margin: 5px 0px;
    color: var(--font-secundary-color);
    display: flex;
    justify-content: space-between;
    grid-column: span 2;
}
.actividad .propiedades {
    font-size: 12px;
    display: flex;
    gap: 10px;
    flex-wrap: wrap;
    white-space: nowrap;
    overflow: hidden;
    text-overflow: ellipsis;
    white-space: nowrap;
    overflow: hidden;
    text-overflow: ellipsis;
    display: grid;
    grid-template-columns: repeat(2, 1fr);
}
.actividad label {    
    flex-wrap: wrap;
    white-space: nowrap;
    overflow: hidden;
    text-overflow: ellipsis;
    display:flex;
}
.actividad .options {
    display: flex;
    justify-content: flex-end;    
}
.OptionContainer2 {
    display: flex;
    gap: 5px;
    padding: 10px;
}
@container details (max-width: 700px) {
    .actividad { 
        flex-direction: column;
    }
    .actividad .propiedades {
        gap: 10px;
    }
}`
export { activityStyle }