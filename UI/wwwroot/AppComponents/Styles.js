import { css } from "../WDevCore/WModules/WStyledRender.js";

const priorityStyles = css`
    .prioridad_Alta,
    .prioridad_Media, 
    .prioridad_Baja, .prioridad_undefined {
        position: relative;
        margin-left: 30px !important;
        display: block;
    }
    .prioridad_Alta::before,
    .prioridad_undefined::before,
    .prioridad_Media::before,
    .prioridad_Baja::before {
        content: " ";
        display: block;
        border-radius: 50%;
        min-height: 15px;
        height: 15px;
        min-width: 15px;
        width: 15px;
        background-color: var(--secundary-color);
        position: absolute;
        left: -30px;
    }
    
    .prioridad_Baja::before {
        background-color: #0365d6;
    }
    .prioridad_Media::before {
        background-color: #f87427;
    }
    .prioridad_Alta::before {
        background-color: #ff0000;
    }
    .prioridad_undefined::before {
        background-color: #797575;
    }
`

export { priorityStyles}