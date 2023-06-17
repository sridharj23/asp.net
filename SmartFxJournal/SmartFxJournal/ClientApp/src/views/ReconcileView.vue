<script lang="ts">
    import {ReconcileApi} from '@/api/ReconcileApi';
    import { type ReconcileEntry, type ExecutedOrder, Common, type DataColumn } from '@/types/CommonTypes';
    import DataTable from '@/components/DataTable.vue';
    import { useAccountStore } from '@/stores/accountstore';

    const prePend = { isSelected: "false", isReconciled: "false"} as Record<string, string>;

    export default {
        setup() {
            const api = new ReconcileApi();
            const store = useAccountStore();
            return {api, store};
        },
        data() {
            return {
                orders: [] as Record<string, string>[],
                columns: Common.getExcecutedOrderColumns(),
                selectedRowId: "0",
                reconciled: [] as ReconcileEntry[]
            };
        },
        methods: {
            loadOrders() {
                let acc = this.store.selectedAccount;
                this.orders = [];
                this.api.getOrderToReconcile(acc.toString()).then((resp) => {
                    resp.forEach(pos => this.orders.push(Common.convertOrderToRecord(pos, prePend)));
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
    }
</style>
