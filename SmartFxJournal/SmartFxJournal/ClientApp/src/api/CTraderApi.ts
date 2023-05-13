import { RestApi } from "./ApiBase";

export class CTraderAPI extends RestApi {
    readonly resource = 'ctrader';

    public hasLogin(cTraderId : string) : boolean {
        var res : boolean = false;
        super.single(this.resource + '/haslogin/' + cTraderId).then (r => res = r as boolean);
        return res;
    }

    public async getLoginTarget(cTraderId : string, clientId : string, secret: string) : Promise<string> {
        let url = "/ctrader/login?cTraderId=" + cTraderId + "&client_id=" + clientId + "&client_secret=" + secret;
        return await super.single<string>(url);
    }
}

