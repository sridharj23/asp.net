import { JournalApi } from "./JournalApiBase";

export interface Account {
    accountNo : string,
    accountType: string,
    isDefault : boolean,
    nickName : string,
    brokerName : string,
    currencyType: string,
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
