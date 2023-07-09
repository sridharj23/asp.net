import { RestApi } from "./ApiBase";
import type { EquityCurve, SummaryAggregate } from "@/types/CommonTypes";

export class SummaryAPI extends RestApi {
    public async getEquityCurve(accountNo : string) : Promise<EquityCurve> {
        return super.single('Summary/' + accountNo +'/equity');
    }

    public async getSummary(accountNo : string) : Promise<SummaryAggregate[]> {
        return super.single('Summary/' + accountNo +'/overview');
    }

    public async getAggregates(accountNo : string) : Promise<Map<string,Array<string[]>>> {
        return super.single('Summary/' + accountNo +'/aggregates');
    }
}