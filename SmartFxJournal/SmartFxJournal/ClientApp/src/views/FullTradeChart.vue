<script lang="ts">
    import {ChartHelper} from '@/helpers/ChartHelper';
    import HighCharts from 'highcharts';
    import stockInit from 'highcharts/modules/stock';
    import chartdata from '@/data/chartdata.json';
    import { usePositionStore } from '@/stores/positionstore';

    stockInit(HighCharts);

    export default {
        setup() {
            const store = usePositionStore();
            return {store};
        },
        props: ['positionId'],
        methods: {
            loadChartForPosition() {
                ChartHelper.loadChartData(this.chartOptions);
            },

        },
        mounted() {
            this.chartOptions.series[0].data = chartdata;
            this.store.$subscribe(this.loadChartForPosition)
            if (+this.store.dblClickedPositionId > 0) {
                this.loadChartForPosition();
            }
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