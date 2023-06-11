<script lang="ts">
    import { CTraderAPI } from '@/api/CTraderApi';
    import {ChartHelper} from '@/helpers/ChartHelper';

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
                    this.chartOptions.title.text = resp.symbol + " : " + resp.chartPeriod;
                    this.chartOptions.series[0].data = resp.trendBars;
                });
            }
        },
        methods: {
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