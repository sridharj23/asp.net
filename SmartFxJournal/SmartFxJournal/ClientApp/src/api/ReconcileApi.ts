import type { NumberFormatterCallbackFunction } from "highcharts";
import { RestApi } from "./ApiBase";

export interface OrderToReconcile {
    dealId: number,
    orderId: number,
    positionId: number,
    symbol: string,
    direction: string,
    filledVolume: number,
    closedVolume: number,
    isClosing: boolean,
    executionPrice: number,
    commission: number,
    swap: number,
    grossProfit: number,
    balanceAfter: number,
    orderExecutedAt: string
}

export interface ReconcileEntry {
    positionId: number,
    orders: number[]
}

export class ReconcileApi extends RestApi {
    public async getOrderToReconcile(accountNo : string) : Promise<OrderToReconcile[]> {
        let url = "/pendingreconcile/" + accountNo;

        return super.index<OrderToReconcile>(url);
    }

    public async reconcileOrders(accountNo: string, positions: ReconcileEntry[]) : Promise<any> {
        let url = "/pendingreconcile/" + accountNo;
        return super.post<ReconcileEntry[]>(url, positions);
    }
}