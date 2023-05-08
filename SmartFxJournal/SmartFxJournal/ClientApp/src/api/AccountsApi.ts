import { JournalApi } from "./ApiBase";

export interface Account {
    accountNo : string,
    accountType: string,
    default : boolean,
    nickName : string,
    brokerName : string,
    currencyType: string,
    startBalance : number,
    currentBalance : number,
    openingDate : string,
    importMode: string,
    lastImportedAt?: string,
    createdAt?: string,
    modifiedAt?: string
}

export class AccountsAPI extends JournalApi<Account> {

    constructor() {
        super('accounts');
    }
    
    protected getKeyOf(input: Account): string {
        return input.accountNo;
    }
}
