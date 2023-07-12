<script lang="ts">
    import Card from '@/components/Card.vue';
    import FullTradeChart from '@/views/FullTradeChart.vue';

    export default {
        components: {
            Card,
            FullTradeChart
        }
    }
</script>

<template>
    <div>
        <Card id="fullScreenCard">
            <template #default>
                <FullTradeChart id="tradeChartView" ref="tradeChart"/>
            </template>
        </Card>
    </div>

</template>

<style>
    #fullScreenCard {
        height: 100%;
        width: 100%;
    }
    #tradeChartView {
        height: 100%;
    }
</style>