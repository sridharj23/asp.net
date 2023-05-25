import { JournalApi } from "./JournalApiBase";

export interface Account {
    accountNo : string,
    isLive: boolean,
    isDefault : boolean,
    nickName : string,
    broker : string,
    accountCurrency: string,
    startBalance : number,
    currentBalance : number,
    openedOn : string,
    importMode: string,
    lastImportedOn?: string,
    createdOn?: string,
    modifiedOn?: string
}

export class AccountsAPI extends JournalApi<Account> {

    constructor() {
        super('Accounts');
    }
    
    protected getKeyOf(input: Account): string {
        return input.accountNo;
    }
}
