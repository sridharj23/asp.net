import { JournalApi } from "./JournalApiBase";

export interface TradeData {
    orderId: number,
    executionTime : string,
    filledVolume: number,
    direction: string,
    price: number,
    swap: number,
    commission: number,
    grossProfit: number,
    netProfit : number
}

export interface Position {
    accountNo: string,
    positionId : string,
    symbol : string,
    grossProfit : number,
    fees : number,
    swap : number,
    commission: number,
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