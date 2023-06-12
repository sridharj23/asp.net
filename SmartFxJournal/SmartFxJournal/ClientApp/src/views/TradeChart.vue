<script lang="ts">
    import { CTraderAPI } from '@/api/CTraderApi';
    import {ChartHelper} from '@/helpers/ChartHelper';
    import HighCharts from 'highcharts';
    import stockInit from 'highcharts/modules/stock';
    import chartdata from '@/data/chartdata.json';

    stockInit(HighCharts);

    export default {
        setup() {
            const api = new CTraderAPI();
            return {api};
        },
        props: ['positionId'],
        watch: {
            positionId(newVal, oldVal) {
                console.log("Position ID changed : " + newVal + " | was " + oldVal);
                this.api.getChartData(newVal).then(resp => {
                    console.log(resp);
                    this.chartOptions.title.text = resp.symbol + " : " + resp.timePeriod;
                    this.chartOptions.series[0].data = resp.trendBars;
                });
            }
        },
        methods: {

        },
        mounted() {
            this.chartOptions.series[0].data = chartdata;
        },
        data() {
            return {
                chartOptions: ChartHelper.getDefaultStockChartOptions(),
                pos: "something"
            }
        }
    }

</script>

<template>
    <div>
        <highcharts id="tradeChart" :constructor-type="'stockChart'" :positionId="pos" class="hc" :options="chartOptions"/>
    </div>
</template>

<style scoped>
    #tradeChart {
        height: 100%;
    }
</style>