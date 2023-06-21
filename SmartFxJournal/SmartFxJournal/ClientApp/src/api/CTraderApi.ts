import { RestApi } from "./ApiBase";
import type { TrendBars } from "@/types/CommonTypes";

export class CTraderAPI extends RestApi {
    readonly resource = 'ctrader';

    public hasLogin(cTraderId : string) : Promise<boolean> {
        return super.single(this.resource + '/haslogin/' + cTraderId);
    }

    public async getLoginTarget(cTraderId : string, clientId : string, secret: string) : Promise<string> {
        let url = "/ctrader/login?cTraderId=" + cTraderId + "&client_id=" + clientId + "&client_secret=" + secret;
        return await super.single<string>(url);
    }

    public async importAccounts(cTraderId : string, accountNo?: string ) : Promise<string> {
        let url = this.resource + '/import/' + cTraderId;
        if (accountNo != undefined) {
            url=url + "?accountNo=" + accountNo?? "";
        }
        let res = await super.post<string>(url, cTraderId);
        return res;
    }

    public async getChartData(positionId: string) : Promise<TrendBars> {
        return await super.single<TrendBars>(this.resource + '/trendbars/' + positionId);
    }
}

