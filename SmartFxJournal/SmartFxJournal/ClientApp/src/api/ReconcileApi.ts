import { RestApi } from "./ApiBase";
import type { OrderToReconcile, ReconcileEntry } from "@/types/CommonTypes";

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