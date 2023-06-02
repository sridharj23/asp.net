<script lang="ts">
    import { useAccountStore } from '@/stores/accountstore';
    import { SummaryAPI, type EquityDataPoint } from '@/api/SummaryApi';
    import { Chart, type ChartOptions } from 'highcharts-vue';
    import {ChartHelper} from '@/helpers/ChartHelper'
    
    const boldPositive = '#009933';
    const boldNegative = '#e60000';
    const longs = '#b3b3ff';
    const shorts = '#ffbb99';

    export default {
        setup() {
            const api = new SummaryAPI();
            const store = useAccountStore();
            return {api, store};
        },
        components: {
            highcharts: Chart 
        },
        created() {
            this.store.$subscribe(this.initializeChartOptions);
            if(this.store.selectedAccount != "0") {
                this.initializeChartOptions();
            }
        },
        methods: {
            initializeChartOptions() {
                this.api.getSummary(this.store.selectedAccount).then((resp) => {
                    this.chartOptions.series = [];
                    this.chartOptions.series.push({name: 'Net Profit', data: [] as any});
                    this.chartOptions.series.push({name: 'Longs', data: [] as any});
                    this.chartOptions.series.push({name: 'Shorts', data: [] as any});
                    console.log(this.chartOptions.series);
                    resp.forEach(entry => {
                        this.chartOptions.series[0].data.push({ name: entry.aggregateKey, y: entry.totalPL, color: entry.totalPL > 0 ? boldPositive : boldNegative});
                        this.chartOptions.series[1].data.push({ name: entry.aggregateKey, y: entry.plFromLongs, color: longs, borderWidth: 2, borderColor: entry.plFromLongs > 0 ? boldPositive : boldNegative });
                        this.chartOptions.series[2].data.push({ name: entry.aggregateKey, y: entry.plFromShorts, color: shorts, borderWidth: 2, borderColor: entry.plFromShorts > 0 ? boldPositive : boldNegative});
                    });
                });
            }
        },
        data() {
            return {
                chartOptions: ChartHelper.getDefaultColumnOptions()
            }
        }
    }
</script>

<template>
    <div id="ChartContainer">
        <highcharts id="sChart" class="hc" :options="chartOptions"/>
    </div>

</template>

<style scoped>
    #sChart {
        display: block;
        height: 99%;
    }
</style>