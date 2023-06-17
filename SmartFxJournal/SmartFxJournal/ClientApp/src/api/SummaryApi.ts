import { RestApi } from "./ApiBase";
import type { EquityCurve, SummaryAggregate } from "@/types/CommonTypes";

export class SummaryAPI extends RestApi {
    public async getEquityCurve(accountNo : string) : Promise<EquityCurve> {
        return super.single('Summary/' + accountNo +'/equity');
    }

    public async getSummary(accountNo : string) : Promise<SummaryAggregate[]> {
        let res = super.single('Summary/' + accountNo +'/aggregates');
        return super.single('Summary/' + accountNo +'/aggregates');;
    }
}