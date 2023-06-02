<script lang="ts">
    import { PostionsAPI, type TradeData } from '@/api/PositionsApi';
    import type { Position } from '@/api/PositionsApi';
    import TableControl from '@/components/TableControl.vue';

    const api = new PostionsAPI()

    export class PositionEntry {
        constructor(pos : Position, td : TradeData, summary : boolean) {
            this.Position = summary ? pos.positionId : "-";
            this.Symbol = summary ? pos.symbol : "-";
            this.Direction = td.direction;
            this.Volume = td.filledVolume.toFixed(2);
            this.ExecutionPrice = td.price.toFixed(5);
            this.Commission = (summary ? pos.commission : td.commission).toFixed(2);
            this.Swap = (summary ? pos.swap : td.swap).toFixed(2);
            this.GrossProfit = (summary ? pos.grossProfit : td.grossProfit).toFixed(2);
            this.NetProfit = (summary ? pos.netProfit : td.netProfit).toFixed(2);
            this.ExecutionTime = summary ? "-" : td.executionTime;
        }
        Position!: string;
        Symbol!: string;
        Volume!: string;
        Direction!: string;
        ExecutionPrice!: string;
        ExecutionTime?: string;
        Swap!: string;
        Commission!: string;
        GrossProfit!: string;
        NetProfit!: string;
    }

    export default {
    data() {
        return {
            positions: [] as PositionEntry[],
            headers: ["Position ID", "Symbol", "Direction", "Volume", "Executed Price", "Commission", "Swap", "Gross Profit", "Net Profit", "Execution Time"],
            propKeys: ["Position", "Symbol", "Direction", "Volume", "ExecutionPrice", "Commission", "Swap", "GrossProfit", "NetProfit", "ExecutionTime"]
        };
    },
    methods: {
        loadPostions() {
            api.getAll().then((resp) => {
                resp.forEach(p => this.addPosition(p));
            });
        },
        addPosition(pos: Position) {
            this.positions.push(new PositionEntry(pos, pos.openedOrders[0], true));
            this.positions.push(new PositionEntry(pos, pos.openedOrders[0], false));
            pos.closedOrders.forEach(entry => {
                this.positions.push(new PositionEntry(pos, entry, false));
            });
        }
    },
    mounted() {
        this.loadPostions();
    },
    components: { TableControl }
}
</script>

<template>
    <div id="tableContainer">
        <TableControl id="positionsTable" :dataSource="positions" :headerSource="headers" :dataKeys="propKeys"/>
    </div>
</template>

<style scoped>
    #tableContainer {
        display: flex;
        flex-direction: column;
        flex-grow: 1;
        height: 99%;
    }
    #positionsTable {
        flex-grow: 1;
    }
</style>