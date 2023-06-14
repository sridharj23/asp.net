<script setup>
    import Card from '@/components/Card.vue';
    import TradeChart from '@/views/TradeChart.vue';
</script>

<template>
    <div id="AnalysisPage">
        <Card id="PositionCard">
            <template #default>
                <p>A paragraph for the main content.</p>
                <p>And another one.</p>
            </template>
        </Card>

        <div id="MiddleContainer">
            <Card id="MiddleLeft">
                <template #default>
                    <TradeChart id="tradeChartView" ref="tradeChart" :positionId="selectedPosition"/>
                </template>
            </Card>
            <Card id="MiddleRight">
                <template #default>
                    <p>A paragraph for the main content.</p>
                    <p>And another one.</p>
                </template>
            </Card>
        </div>
    </div>
</template>

<style scoped>
    #AnalysisPage {
        display: flex;
        flex-direction: column;
    }
    #PositionCard {
        width: 100%;
        height: 30%;
    }
    #MiddleContainer {
        display: flex;
        flex-direction: row;
        flex-grow: 1;
        width: 100%;
        height: 69%;
        margin-top: 10px;
    }
    #MiddleLeft {
        width: 70%;
        margin-right: 5px;
        display: flex;
        flex-direction: column;
    }
    #MiddleRight {
        width: 30%;
        margin-left: 5px;
        display: flex;
        flex-direction: column;
    }
    #tradeChartView {
        height: 100%;
    }
</style>