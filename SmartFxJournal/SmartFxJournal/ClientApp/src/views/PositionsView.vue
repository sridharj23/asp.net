<script lang="ts">
    import { CTraderAPI } from '@/api/CTraderApi';
    import { PostionsAPI, type TradeData } from '@/api/PositionsApi';
    import type { Position } from '@/api/PositionsApi';
    import DataTable from '@/components/DataTable.vue';
    import type { DataColumn } from '@/components/DataTable.vue';
    
    const api = new PostionsAPI()
    const charApi = new CTraderAPI()

    export default {
    data() {
        return {
            positions: [] as Record<string, string>[],
            columns: [
                { property: "positionId", title: "Position ID", propType: "default" },
                { property: "symbol", title: "Symbol", propType: "default" },
                { property: "direction", title: "Direction", propType: "default" },
                { property: "volume", title: "Volume", propType: "default" },
                { property: "price", title: "Execution Price", propType: "default" },
                { property: "commission", title: "Commission", propType: "Currency" },
                { property: "swap", title: "Swap", propType: "Currency" },
                { property: "grossProfit", title: "Gross Profit", propType: "Currency" },
                { property: "netProfit", title: "Net Profit", propType: "Currency" },
                { property: "executionTime", title: "Execution Time", propType: "default" }
            ] as DataColumn[],
            selectedRowId: "0"
        };
    },
    methods: {
        loadPostions() {
            api.getAll().then((resp) => {
                resp.forEach(pos => {
                    this.positions.push(this.createRow(pos, pos.openedOrders[0], true));
                    this.positions.push(this.createRow(pos, pos.openedOrders[0], false));
                    pos.closedOrders.forEach(entry => {
                        this.positions.push(this.createRow(pos, entry, false));
                    });
                });
            });
        },
        createRow(pos: Position, td: TradeData, summary: boolean) {
            return {
                group: pos.positionId,
                positionId: summary ? pos.positionId : "",
                symbol: summary ? pos.symbol : "",
                direction: td.direction,
                volume: td.filledVolume.toFixed(2),
                price: td.price.toFixed(5),
                commission: (summary ? pos.commission : td.commission).toFixed(2),
                swap: (summary ? pos.swap : td.swap).toFixed(2),
                grossProfit: (summary ? pos.grossProfit : td.grossProfit).toFixed(2),
                netProfit: (summary ? pos.netProfit : td.netProfit).toFixed(2),
                executionTime: summary ? "-" : td.executionTime
            } as Record<string, string>;
        },
        handleRowSelection(position: string) {
            
        },
        handleRowDblClick(position: string) {
            charApi.getChartData(position);
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
        <DataTable class="dataTable" :columns="columns" :dataSource="positions" :rowIdProperty="'group'" @rowSelected="handleRowSelection" @rowDoubleClicked="handleRowDblClick"/>
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