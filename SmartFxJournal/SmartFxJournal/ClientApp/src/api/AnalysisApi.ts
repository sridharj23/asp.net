import { RestApi } from "./ApiBase";
import type { AnalysisEntry } from "@/types/CommonTypes";

export class AnalysisApi extends RestApi {
    readonly resource = 'analysis';

    public getAnalysisForPosition(positionId : string) {
        let url = this.resource + "/for_position/" + positionId;
        return super.index<AnalysisEntry>(url);
    }
}