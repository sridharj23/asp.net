<script lang="ts">
    import { CTraderAPI } from '@/api/CTraderApi';
    import { Chart, type ChartOptions } from 'highcharts-vue';
    import {ChartHelper} from '@/helpers/ChartHelper';

    export default {
        setup() {
            const api = new CTraderAPI();
            return {api};
        },
        components: {
            tradeChart: Chart
        },
        methods: {
        },
        data() {
            return {
                chartOptions: ChartHelper.getDefaultLineOptions()
            }
        }
    }

</script>

<template>
    <div>
        <tradeChart id="tradeChart" class="hc" :options="chartOptions"/>
    </div>
</template>

<style scoped>
    #tradeChart {
        height: 100%;
    }
</style>