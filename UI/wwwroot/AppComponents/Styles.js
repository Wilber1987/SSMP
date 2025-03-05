import { css } from "../WDevCore/WModules/WStyledRender.js";

const priorityStyles = css`
    .prioridad_Alta,
    .prioridad_Media, 
    .prioridad_Baja, .prioridad_undefined {
        position: relative;        
        display: block;
    }
    .prioridad_Alta::before,
    .prioridad_undefined::before,
    .prioridad_Media::before,
    .prioridad_Baja::before {
        content: " ";
        display: inline-block;
        border-radius: 50%;
        min-height: 15px;
        height: 15px;
        min-width: 15px;
        width: 15px;
        margin-right:5px;
        background-color: var(--secundary-color);        
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
        background-color: #b6b6b6;
    }

    .platform_WHATSAPP, 
    .platform_WEBAPI,
    .platform_MESSENGER,
    .platform_X,
    .platform_INSTAGRAM,
    .platform_MAIL {
        display: block;
        height: 30px;
        background-image: url(/Media/Image/webapp.svg);
        background-repeat: no-repeat;
        background-size: contain; /* O cover, según necesites */
        background-position: 0 center; /* Centra el fondo */
    }
    .platform_undefined, .platform_SSMP {
        background-image: url(/Media/Image/SSMPLogo.png);
    }

    .platform_WHATSAPP {
        background-image: url(/Media/Image/whatsapp.svg);
    }
    .platform_MESSENGER {
        background-image: url(/Media/Image/messenger.svg);
    }
    .platform_X {
        background-image: url(/Media/Image/x.svg);
    }
    .platform_INSTAGRAM {
        background-image: url(/Media/Image/instagram.svg);
    }
    .platform_MAIL {
        background-image: url(/Media/Image/mail.svg);
    }
`

export { priorityStyles }