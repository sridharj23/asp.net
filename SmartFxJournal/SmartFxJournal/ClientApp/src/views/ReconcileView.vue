<script lang="ts">
    import {ReconcileApi} from '@/api/ReconcileApi';
    import type { ReconcileEntry, OrderToReconcile } from '@/types/JournalTypes';
    import DataTable from '@/components/DataTable.vue';
    import type { DataColumn } from '@/components/DataTable.vue';
    import { useAccountStore } from '@/stores/accountstore';

    export default {
        setup() {
            const api = new ReconcileApi();
            const store = useAccountStore();
            return {api, store};
        },
        data() {
            return {
                orders: [] as Record<string, string>[],
                columns: [
                    { property: "orderId", title: "Order ID", propType: "default" },
                    { property: "positionId", title: "Position ID", propType: "default" },
                    { property: "symbol", title: "Symbol", propType: "default" },
                    { property: "direction", title: "Direction", propType: "default" },
                    { property: "filledVolume", title: "Filled Volume", propType: "default" },
                    { property: "closedVolume", title: "Filled Volume", propType: "default" },
                    { property: "executionPrice", title: "Execution Price", propType: "default" },
                    { property: "commission", title: "Commission", propType: "Currency" },
                    { property: "swap", title: "Swap", propType: "Currency" },
                    { property: "grossProfit", title: "Gross Profit", propType: "Currency" },
                    { property: "balanceAfter", title: "Balance", propType: "Currency" },
                    { property: "orderExecutedAt", title: "Execution Time", propType: "default" }
                ] as DataColumn[],
                selectedRowId: "0",
                reconciled: [] as ReconcileEntry[]
            };
        },
        methods: {
            loadOrders() {
                let acc = this.store.selectedAccount;
                this.orders = [];
                this.api.getOrderToReconcile(acc.toString()).then((resp) => {
                    resp.forEach(pos => this.orders.push(this.createRow(pos)));
                });
            },
            handleGroupingRequest() {
                let pos:string = '';
                this.orders.forEach(o => {
                    if (o.isSelected == 'true') {
                        if (pos == '') {
                            pos = o.dealId;
                        }
                        o.isSelected = 'disabled';
                        o.positionId = pos;
                        o.isReconciled = 'true';
                    }
                })
            },
            handleReconcileSave() {
                this.reconciled = [];
                let entries : Map<string, ReconcileEntry> = new Map<string, ReconcileEntry>();

                this.orders.forEach(o => {
                    if (o.isReconciled == 'true' || +o.positionId > 0 ) {
                        let entry = entries.get(o.positionId);
                        if (entry == undefined) {
                            entry = {positionId: +o.positionId, orders: [] };
                            entries.set(o.positionId, entry);
                        }
                        entry.orders.push(+o.orderId);
                    }
                })

                entries.forEach(e => this.reconciled.push(e));
                this.api.reconcileOrders(this.store.selectedAccount, this.reconciled).then(res => {
                    this.api.displayInfo(res.result);
                    this.loadOrders();
                });
            },
            createRow(pos: OrderToReconcile) {
                return {
                    isSelected: "false",
                    isReconciled: "false",
                    dealId: pos.dealId.toString(),
                    positionId: pos.positionId.toString(),
                    orderId: pos.orderId.toString(),
                    symbol: pos.symbol,
                    direction: pos.direction,
                    filledVolume: pos.filledVolume.toString(),
                    closedVolume: pos.closedVolume.toString(),
                    executionPrice: pos.executionPrice.toFixed(5),
                    commission: pos.commission.toFixed(2),
                    swap: pos.swap.toFixed(2),
                    grossProfit: pos.grossProfit.toFixed(2),
                    balanceAfter: pos.balanceAfter.toFixed(2),
                    orderExecutedAt: new Date(pos.orderExecutedAt).toLocaleString(),
                } as Record<string, string>;
            },
            handleOnDrag(event : DragEvent) {
                console.log(event);
            }
        },
        mounted() {
            this.loadOrders();
        },
        components: { DataTable }
    }
</script>

<template>
    <div id="reconcileContainer">
        <div id="orderTable">
            <DataTable class="dataTable" :columns="columns" :dataSource="orders" :rowIdProperty="'orderId'" :multiSelect="true" :ondragstart="handleOnDrag" />
        </div>
        <div id="actions" class="flow-row">
            <button type="button" @click="handleGroupingRequest">Group Selected</button>
            <button type="button" @click="handleReconcileSave">Save Reconciled</button>
        </div>
    </div>
</template>

<style scoped>
    #reconcileContainer {
        display: flex;
        flex-direction: column;
        flex-grow: 1;
        height: 100%;
        width: 100%;
    }
    #orderTable {
        flex-grow: 1;
        width: 100%;
        height: calc(100% - 50px);
    }
    #actions {
        width: 100%;
        height: 50px;
        bottom: 0;
    }
    .dataTable {
        max-height: 85vh;
        border: 1px solid purple;
    }
</style>
