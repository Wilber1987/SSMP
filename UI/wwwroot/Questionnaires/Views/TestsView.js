//@ts-check
import { TestsViewManager } from "./Component/TestsViewManager.js";
window.onload = ()=>{
    // @ts-ignore
    Main.append(new TestsViewManager({}));
}
