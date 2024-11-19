import { Tbl_Case_ModelComponent } from "../../FrontModel/Tbl_CaseModule.js";
import { CaseDetailComponent } from "./CaseDetailComponent.js";

const OnLoad = async () => {
    const caseD = JSON.parse(sessionStorage.getItem("detailCase"));
    const find = await new Tbl_Case_ModelComponent({ Id_Case: caseD.Id_Case }).Get()
    const CaseDetail = new CaseDetailComponent(find[0]);
    Main.appendChild(CaseDetail);
}
window.onload = OnLoad;