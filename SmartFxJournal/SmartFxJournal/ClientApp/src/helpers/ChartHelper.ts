import type { ChartOptions, PlotOptions, SeriesOptionsType } from "highcharts";

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
                type: 'linear'
            },
            series:[{
                name: '€ ',
                data: [3, 9, 6] as any[]
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
}