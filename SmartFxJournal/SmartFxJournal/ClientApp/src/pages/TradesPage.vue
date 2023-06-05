<script setup>
    import PositionsView from '@/views/PositionsView.vue';
    import TradeChart from '@/views/TradeChart.vue';
    import Card from '@/components/Card.vue';
</script>

<template>
    <div id="TradesPage">
        <Card id="positionsView">
            <PositionsView/>
        </Card>
        <div id="TradesPageRight">
            <Card id="tradeGraph">
                <TradeChart id="tradeChartView"/>
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
        width: 50%;
        height: 100%;
        margin-right: 5px;
    }
    #TradesPageRight {
        width: 50%;
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
