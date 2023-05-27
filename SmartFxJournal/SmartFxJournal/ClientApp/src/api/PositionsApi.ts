import { JournalApi } from "./JournalApiBase";

export interface TradeData {
    orderId: number,
    executionTime : string,
    filledVolume: number,
    direction: string,
    price: number,
    swap: number,
    commission: number,
    grossProfit: number
}

export interface Position {
    positionId : string,
    accountNo: string,
    symbol : string,
    grossProfit : number,
    fees : number,
    netProfit : number
    openedOrders: TradeData[],
    closedOrders: TradeData[]
}

export class PostionsAPI extends JournalApi<Position> {
    constructor() {
        super('Positions');
    }

    protected getKeyOf(input: Position): string {
        return input.positionId;
    }
}