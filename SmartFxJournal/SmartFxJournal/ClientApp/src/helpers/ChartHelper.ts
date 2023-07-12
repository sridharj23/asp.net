import type { ChartOptions, PlotOptions } from "highcharts";
import { usePositionStore } from '@/stores/positionstore';
import type { AnalysisEntry } from '@/types/CommonTypes';
import { CTraderAPI } from '@/api/CTraderApi';
import {AnalysisApi} from '@/api/AnalysisApi';


export class ChartHelper {

    static readonly store = usePositionStore();
    static readonly api = new CTraderAPI();
    static readonly pApi = new AnalysisApi();

    private static floorToHour(mseconds: number) {
        let mod = mseconds % 3600000;
        return mseconds - mod;
    }

    private static dateToHour(date : string) {
        let dt = new Date(date);
        let ms = dt.getTime() + 7200000;
        let mod = ms % 3600000;
        return ms - mod;
    }

    public static loadChartData(options : ChartOptions) {
        options.yAxis.plotLines[0].value = 0; // takeprofit
        options.yAxis.plotLines[1].value = 0; // stoploss
        options.series[2].data = []; // ideal exit

        this.api.getChartData(this.store.dblClickedPositionId).then(resp => {
            options.title.text = resp.symbol + " : " + resp.timePeriod;
            options.series[0].data = [];
            let data = new Array<number[]>();
            resp.trendBars.forEach(bar => {
                let entry = new Array<number>();
                bar.forEach(n => entry.push(+n));
                data.push(entry);
            });
            options.series[0].data = data;

            options.series[1].data = [{x: this.floorToHour(resp.positionOpenedAt), y: resp.positionOpenPrice},
                                                {x: this.floorToHour(resp.positionClosedAt), y: resp.positionClosePrice}];

            let isProfit = +this.store.dblClickedPosition['netProfit'] > 0;
            options.plotOptions.line.color = isProfit ? 'yellowgreen' : 'purple';

            this.pApi.getAnalysisForPosition(this.store.dblClickedPositionId).then (entries => {
                let idn = {} as AnalysisEntry;
                let idx = {} as AnalysisEntry;
                let idnSet = false, idxSet = false;
                entries.forEach(e => {
                    if (e.analysisScenario == "Actual" && e.analyzedAspect == "StopLoss") {
                        options.yAxis.plotLines[1].value = e.executionPrice;
                    } else if (e.analysisScenario == "Actual" && e.analyzedAspect == "TakeProfit") {
                        options.yAxis.plotLines[0].value = e.executionPrice;
                    } else if (e.analysisScenario == "Ideal" && e.analyzedAspect == "Exit") {
                        idx = e;
                        idxSet = true;
                    } else if (e.analysisScenario == "Ideal" && e.analyzedAspect == "Entry") {
                        idn = e;
                        idnSet = true;
                    }
                });
                if (idnSet && idxSet) {
                    options.series[2].data = [{x: this.dateToHour(idn.executionTime), y: idn.executionPrice},
                                                        {x: this.dateToHour(idx.executionTime), y: idx.executionPrice}];
                }
            });
        });

    }


    public static getDefaultColumnOptions() : ChartOptions {
        return {
            chart: { type: 'column' },
            title: { text: 'P & L' },
            legend: { enabled: false },
            xAxis: { type: 'category' },
            yAxis: {
                title: { text: 'Euro - €' },
                type: 'linear'
            },
            series:[{
                name: 'Net Profit',
                data: [] as any[]
            }], 
            plotOptions: {
                column: {
                    shadow: true,
                    dataLabels: { enabled: true },
                    borderColor: 'grey'
                },
            } as PlotOptions
        } as ChartOptions;
    }

    public static getDefaultLineOptions() : ChartOptions {
        return {
            chart: { type: 'line' },
            title: { text: 'Growth' },
            legend: { enabled: false },
            xAxis: { type: 'datetime' },
            yAxis: {
                title: { text: 'Euro - €' },
                type: 'linear',
                plotLines: [{
                    color: 'purple',
                    width: 3,
                    value: 0,
                    dashStyle: 'solid',
                    label: {
                        text: 'Start Balance',
                        style: {
                            color: 'purple',
                            fontWeight: 'bold'
                        }
                    }
                }]
            },
            series:[{
                name: '€ ',
                data: [] as any[]
            }], 
            plotOptions: {
                line: {
                    color: '#0080ff',
                    dataLabels: { enabled: false },
                    enableMouseTracking: true,
                    lineWidth: 2,
                    marker: {
                        enabled: true,
                        states: {
                            hover: { enabled: true }
                        }
                    }
                },
            } as PlotOptions
        } as ChartOptions;
    }

    public static getDefaultStockChartOptions() : ChartOptions {
        return {
            title: { text: '' },
            rangeSelector: { enabled : false},
            turboThreshold: 5000,
            legend: { enabled: false },
            navigator: { enabled: false },
            xAxis: { 
                type: 'datetime', 
            },
            yAxis: {
                title: { text: 'Exchange Rate' },
                type: 'linear',
                labels : {
                    formatter: function() {
                        return this.value.toFixed(5);
                    }
                },
                plotLines: [{
                    id: "takeprofit",
                    value: 0,
                    color: 'green',
                    dashStyle: 'shortdash',
                    width: 2,
                    label: {
                        text: 'Take profit'
                    }
                }, {
                    id: "stoploss",
                    value: 1.08780,
                    color: 'red',
                    dashStyle: 'shortdash',
                    width: 2,
                    label: {
                        text: 'Stop Loss'
                    }
                }]
            },
            series:[{
                type: 'candlestick',
                name: '',
                data: [] as any[]
            },{
                type: 'line',
                name: 'Actual',
                marker: {symbol: 'cross'},
                data: []
            },{
                type: 'line',
                name: 'Ideal',
                color: 'green',
                lineWidth: 4,
                dashStyle : 'shortdash',
                marker: {symbol: 'cross'},
                data: []
            }], 
            plotOptions: {
                line: {
                    color: 'red',
                    dataLabels: { enabled: false },
                    lineWidth: 6,
                },
                candlestick: {
                    color: 'salmon',
                    upColor: 'lightseagreen'
                }
            } as PlotOptions
        } as ChartOptions;
    }
}