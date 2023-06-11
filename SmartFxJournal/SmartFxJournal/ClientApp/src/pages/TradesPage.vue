<script lang="ts">
    import PositionsView from '@/views/PositionsView.vue';
    import TradeChart from '@/views/TradeChart.vue';
    import Card from '@/components/Card.vue';

    export default {
        components: {
            PositionsView,
            TradeChart,
            Card
        },
        data() {
            return {
                selectedPosition: ""
            }
        },
        methods: {
            handlePositionSelection(position : string) {
                this.selectedPosition = position;
            }
        }
    }
</script>

<template>
    <div id="TradesPage">
        <Card id="positionsView">
            <PositionsView @positionSelected="handlePositionSelection"/>
        </Card>
        <div id="TradesPageRight">
            <Card id="tradeGraph">
                <TradeChart id="tradeChartView" ref="tradeChart" :positionId="selectedPosition"/>
            </Card>
            <Card id="tradesNotes"/>
        </div>
    </div>
</template>

<style scoped>
    #TradesPage {
        display: flex;
        flex-direction: row;
        flex-grow: 1;
        height: 90vh;
    }
    #positionsView {
        width: 60%;
        height: 100%;
        margin-right: 5px;
    }
    #TradesPageRight {
        width: 40%;
        margin-left: 5px;
        display: flex;
        flex-direction: column;
        height: 100%;
        flex-grow: 1;
    }
    #tradeGraph {
        height: 50%;
        flex-grow: 1;
    }
    #tradeChartView {
        height: 100%;
    }
    #tradesNotes {
        height: 50%;
        margin-top: 5px;
        flex-grow: 1;
    }

</style>
