<script lang="ts">
    import { CTraderAPI } from '@/api/CTraderApi';
    import {AnalysisApi} from '@/api/AnalysisApi';
    import {ChartHelper} from '@/helpers/ChartHelper';
    import HighCharts from 'highcharts';
    import stockInit from 'highcharts/modules/stock';
    import chartdata from '@/data/chartdata.json';
    import { usePositionStore } from '@/stores/positionstore';
    import type { AnalysisEntry } from '@/types/CommonTypes';

    stockInit(HighCharts);

    export default {
        setup() {
            const api = new CTraderAPI();
            const pApi = new AnalysisApi();
            const store = usePositionStore();
            return {api, pApi, store};
        },
        props: ['positionId'],
        methods: {
            loadChartForPosition() {
                console.log("Chart loading...");
                this.chartOptions.yAxis.plotLines[0].value = 0; // takeprofit
                this.chartOptions.yAxis.plotLines[1].value = 0; // stoploss
                this.chartOptions.series[2].data = []; // ideal exit

                this.api.getChartData(this.store.dblClickedPositionId).then(resp => {
                    this.chartOptions.title.text = resp.symbol + " : " + resp.timePeriod;
                    this.chartOptions.series[0].data = [];
                    let data = new Array<number[]>();
                    resp.trendBars.forEach(bar => {
                        let entry = new Array<number>();
                        bar.forEach(n => entry.push(+n));
                        data.push(entry);
                    });
                    this.chartOptions.series[0].data = data;

                    this.chartOptions.series[1].data = [{x: this.floorToHour(resp.positionOpenedAt), y: resp.positionOpenPrice},
                                                        {x: this.floorToHour(resp.positionClosedAt), y: resp.positionClosePrice}];

                    let isProfit = +this.store.dblClickedPosition['netProfit'] > 0;
                    this.chartOptions.plotOptions.line.color = isProfit ? 'yellowgreen' : 'purple';

                    this.pApi.getAnalysisForPosition(this.store.dblClickedPositionId).then (entries => {
                        let idn = {} as AnalysisEntry;
                        let idx = {} as AnalysisEntry;
                        let idnSet = false, idxSet = false;
                        entries.forEach(e => {
                            if (e.analysisScenario == "Actual" && e.analyzedAspect == "StopLoss") {
                                this.chartOptions.yAxis.plotLines[1].value = e.executionPrice;
                            } else if (e.analysisScenario == "Actual" && e.analyzedAspect == "TakeProfit") {
                                this.chartOptions.yAxis.plotLines[0].value = e.executionPrice;
                            } else if (e.analysisScenario == "Ideal" && e.analyzedAspect == "Exit") {
                                idx = e;
                                idxSet = true;
                            } else if (e.analysisScenario == "Ideal" && e.analyzedAspect == "Entry") {
                                idn = e;
                                idnSet = true;
                            }
                        });
                        if (idnSet && idxSet) {
                            this.chartOptions.series[2].data = [{x: this.dateToHour(idn.executionTime), y: idn.executionPrice},
                                                                {x: this.dateToHour(idx.executionTime), y: idx.executionPrice}];
                        }
                    });
                });
            },
            floorToHour(mseconds: number) {
                let mod = mseconds % 3600000;
                return mseconds - mod;
            },
            dateToHour(date : string) {
                let dt = new Date(date);
                let ms = dt.getTime() + 7200000;
                let mod = ms % 3600000;
                return ms - mod;
            }
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