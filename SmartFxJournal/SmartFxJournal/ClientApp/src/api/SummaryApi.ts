import { RestApi } from "./ApiBase";

export interface EquityCurve {
    accountNumber : number,
    dataPoints : EquityDataPoint[]
}

export interface EquityDataPoint {
    equity: number,
    timeStamp : number
}

export interface SummaryAggregate {
    aggregateKey: string,
    totalPL : number,
    plFromShorts : number,
    plFromLongs : number
}

export class SummaryAPI extends RestApi {
    public async getEquityCurve(accountNo : string) : Promise<EquityCurve> {
        return super.single('Summary/' + accountNo +'/equity');
    }

    public async getSummary(accountNo : string) : Promise<SummaryAggregate[]> {
        let res = super.single('Summary/' + accountNo +'/aggregates');
        console.log(res);
        return super.single('Summary/' + accountNo +'/aggregates');;
    }
}