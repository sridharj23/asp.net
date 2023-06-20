<script lang="ts">
    import Card from '@/components/Card.vue';
    import TradeChart from '@/views/TradeChart.vue';
    import PositionDetailView from '@/views/PositionDetailView.vue';
    import AnalysisView from '@/views/AnalysisView.vue';

    export default {
        components: {
            Card,
            TradeChart,
            PositionDetailView,
            AnalysisView
        },
        data() {
            return {
                selectedEntry : "" as string
            }
        },
        methods: {
            handleEntrySelected(entry : Record<string, string>) {
                this.selectedEntry = entry['type'] + " : " + entry['positionId'];
            }
        }
    }
</script>

<template>
    <div id="AnalysisPage">
        <Card id="PositionCard">
            <template #default>
                <PositionDetailView id="positionDetails" @detail-entry-selected="handleEntrySelected"/>
            </template>
        </Card>

        <div id="MiddleContainer">
            <Card id="MiddleLeft">
                <template #default>
                    <TradeChart id="tradeChartView" ref="tradeChart"/>
                </template>
            </Card>
            <div id="MiddleRight">
                <AnalysisView/>
            </div>
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
    #positionDetails {
        display: block;
        width: 100%;
        height: 100%;
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
        width: 60%;
        margin-right: 5px;
        display: flex;
        flex-direction: column;
    }
    #MiddleRight {
        width: 40%;
        margin-left: 5px;
        display: flex;
        flex-direction: column;
    }
    #tradeChartView {
        height: 100%;
    }
</style>