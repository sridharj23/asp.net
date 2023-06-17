import { JournalApi } from "./JournalApiBase";
import { type Account } from "@/types/CommonTypes";

export class AccountsAPI extends JournalApi<Account> {

    constructor() {
        super('Accounts');
    }
    
    protected getKeyOf(input: Account): string {
        return input.accountNo;
    }
}
