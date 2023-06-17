import { JournalApi } from "./JournalApiBase";
import type { Position } from "@/types/CommonTypes";

export class PostionsAPI extends JournalApi<Position> {
    constructor() {
        super('Positions');
    }

    protected getKeyOf(input: Position): string {
        return input.positionId;
    }
}