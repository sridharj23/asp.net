import { RestApi } from "./ApiBase";

export interface EquityCurve {
    accountNumber : number,
    dataPoints : EquityDataPoint[]
}

export interface EquityDataPoint {
    equity: number,
    timeStamp : number
}

export class SummaryAPI extends RestApi {
    public async getEquityCurve(accountNo : string) : Promise<EquityCurve> {
        return super.single('Summary/' + accountNo +'/equity');
    }
}