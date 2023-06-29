import type { ChartOptions, PlotOptions } from "highcharts";
import { Highcharts } from "highcharts";

export class ChartHelper {

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
                }
            },
            series:[{
                type: 'candlestick',
                name: '',
                data: [] as any[]
            },{
                type: 'line',
                name: 'trade',
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