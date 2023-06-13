<script lang="ts">
    import { CTraderAPI } from '@/api/CTraderApi';
    import {ChartHelper} from '@/helpers/ChartHelper';
    import HighCharts from 'highcharts';
    import stockInit from 'highcharts/modules/stock';
    import chartdata from '@/data/chartdata.json';
    import { usePositionStore } from '@/stores/positionstore';

    stockInit(HighCharts);

    export default {
        setup() {
            const api = new CTraderAPI();
            const store = usePositionStore();
            return {api, store};
        },
        props: ['positionId'],
        methods: {
            loadChartForPosition() {
                console.log("Trade Chart : Position changed to " + this.store.selectedPositionId );
                this.api.getChartData(this.store.selectedPositionId).then(resp => {
                    console.log(resp);
                    this.chartOptions.title.text = resp.symbol + " : " + resp.timePeriod;
                    this.chartOptions.series[0].data = [];
                    let data = new Array<number[]>();
                    resp.trendBars.forEach(bar => {
                        let entry = new Array<number>();
                        bar.forEach(n => entry.push(+n));
                        data.push(entry);
                    });
                    this.chartOptions.series[0].data = data;
                });
            }
        },
        mounted() {
            this.chartOptions.series[0].data = chartdata;
            this.store.$subscribe(this.loadChartForPosition)
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
        <highcharts id="tradeChart" :constructor-type="'stockChart'" class="hc" :options="chartOptions"/>
    </div>
</template>

<style scoped>
    #tradeChart {
        height: 100%;
    }
</style>