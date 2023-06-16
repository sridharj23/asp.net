<script lang="ts">
    import { PostionsAPI } from '@/api/PositionsApi';
    import type { Position } from '@/types/JournalTypes';
    import { usePositionStore } from '@/stores/positionstore';
    import { useAccountStore } from '@/stores/accountstore';
    import DataTable from '@/components/DataTable.vue';
    import type { DataColumn } from '@/components/DataTable.vue';
    
    const api = new PostionsAPI()
    
    export default {
        setup() {
            const store = usePositionStore();
            const accountstore = useAccountStore();
            return {store, accountstore};
        },
        emits: ['positionSelected'],
        data() {
            return {
                positions: [] as Record<string, string>[],
                columns: [
                    { property: "positionId", title: "Position", propType: "default" },
                    { property: "symbol", title: "Symbol", propType: "default" },
                    { property: "direction", title: "Direction", propType: "default" },
                    { property: "volume", title: "Volume", propType: "default" },
                    { property: "entryPrice", title: "Entry Price", propType: "default" },
                    { property: "exitPrice", title: "Exit Price", propType: "default" },
                    { property: "commission", title: "Comm.", propType: "Currency" },
                    { property: "swap", title: "Swap", propType: "Currency" },
                    { property: "grossProfit", title: "Gross P/L", propType: "Currency" },
                    { property: "netProfit", title: "Net P/L", propType: "Currency" },
                    { property: "balanceAfter", title: "Balance", propType: "Currency" },
                    { property: "orderOpenedAt", title: "Opened At", propType: "default" },
                    { property: "orderClosedAt", title: "Closed At", propType: "default" }
                ] as DataColumn[],
                selectedRowId: "0"
            };
        },
        methods: {
            loadPostions() {
                let params = new Map();
                params.set("accountNo", this.accountstore.selectedAccount);
                api.getAll(params).then((resp) => {
                    resp.forEach(pos => this.positions.push(this.createRow(pos)));
                });
            },
            createRow(pos: Position) {
                return {
                    positionId: pos.positionId,
                    symbol: pos.symbol,
                    direction: pos.direction,
                    volume: pos.volumeInLots.toFixed(2),
                    entryPrice: pos.entryPrice.toFixed(5),
                    exitPrice: pos.exitPrice.toFixed(5),
                    commission: pos.commission.toFixed(2),
                    swap: pos.swap.toFixed(2),
                    grossProfit: pos.grossProfit.toFixed(2),
                    netProfit: pos.netProfit.toFixed(2),
                    balanceAfter: pos.balanceAfter.toFixed(2),
                    orderOpenedAt: new Date(pos.orderOpenedAt).toLocaleString(),
                    orderClosedAt: new Date(pos.orderClosedAt).toLocaleString(),
                } as Record<string, string>;
            },
            handleRowSelection(position: string) {
                this.store.lastSelectedPositionId = position;
            },
            handleRowDblClick(position: Record<string, string>) {
                this.store.dblClickedPosition = position;
                this.store.dblClickedPositionId = position['positionId'];
                this.$emit('positionSelected', position);
            }
    },
    mounted() {
        this.loadPostions();
    },
    components: { DataTable }
}
</script>

<template>
    <div id="positionsTable">
        <DataTable class="dataTable" :columns="columns" :dataSource="positions" :rowIdProperty="'positionId'" :selectedRowInitial="store.lastSelectedPositionId.toString()" @rowSelected="handleRowSelection" @rowDoubleClicked="handleRowDblClick"/>
    </div>
</template>

<style scoped>
    #positionsTable {
        flex-grow: 1;
    }
    .dataTable {
        max-height: 88vh;
    }
</style>