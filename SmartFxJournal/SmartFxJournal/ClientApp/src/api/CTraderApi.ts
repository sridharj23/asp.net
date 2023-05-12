import { RestApi } from "./ApiBase";

export class CTraderAPI extends RestApi {
    readonly resource = 'ctrader';

    public hasLogin(cTraderId : string) : boolean {
        var res : boolean = false;
        super.single(this.resource + '/haslogin/' + cTraderId).then (r => res = r as boolean);
        return res;
    }

    public getLoginTarget(cTraderId : string, clientId : string) : string {
        let url = this.resource + '/login?' + 'cTraderId=' + cTraderId + '&client_id=' + clientId;
        let res : string = "";
        super.single<string>(url).then(s => res = s as string);
        console.log("Login URL : " + res);
        return res;
    }
}

