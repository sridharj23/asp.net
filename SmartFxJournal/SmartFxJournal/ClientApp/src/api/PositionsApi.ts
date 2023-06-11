import { JournalApi } from "./JournalApiBase";

export interface Position {
    accountNo: string,
    positionId : string,
    symbol : string,
    direction: string,
    volume: number,
    volumeInLots: number,
    entryPrice: number,
    exitPrice: number,
    fees : number,
    swap : number,
    commission: number,
    grossProfit : number,
    netProfit : number,
    balanceAfter: number,
    orderOpenedAt: string,
    orderClosedAt: string
}

export class PostionsAPI extends JournalApi<Position> {
    constructor() {
        super('Positions');
    }

    protected getKeyOf(input: Position): string {
        return input.positionId;
    }
}