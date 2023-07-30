import { JournalApi } from "./JournalApiBase";
import { type JournalEntry } from "@/types/CommonTypes";

export class JournalNotesApi extends JournalApi<JournalEntry> {

    constructor() {
        super('Journal');
    }
    
    protected getKeyOf(input: JournalEntry): string {
        return input.journalId;
    }
}