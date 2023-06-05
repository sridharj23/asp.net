import { RestApi } from "./ApiBase";

export interface AccountEntry {
    cTraderId : string,
    accountId : string,
    accountCurrency : string,
    brokerName: string,
    isLive : boolean,
    isImported : boolean,
    balance : number,
    opendedOn: string
}

export class CTraderAPI extends RestApi {
    readonly resource = 'ctrader';

    public hasLogin(cTraderId : string) : Promise<boolean> {
        return super.single(this.resource + '/haslogin/' + cTraderId);
    }

    public async getLoginTarget(cTraderId : string, clientId : string, secret: string) : Promise<string> {
        let url = "/ctrader/login?cTraderId=" + cTraderId + "&client_id=" + clientId + "&client_secret=" + secret;
        return await super.single<string>(url);
    }

    public async importAccounts(cTraderId : string) : Promise<string> {
        let res = await super.post<string>(this.resource + '/import/' + cTraderId, cTraderId);
        return res;
    }

    public async getChartData(positionId: string) {
        let res = await super.single(this.resource + '/trendbars/' + positionId);
        console.log(res);
        return res;
    }
}

