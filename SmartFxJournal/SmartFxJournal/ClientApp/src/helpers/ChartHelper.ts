import type { ChartOptions, SeriesOptionsType } from "highcharts";

export class ChartHelper {
    public static getDefaultOptions() : ChartOptions {
        return {
            title: {
                text: 'Growth'
            },
            legend: {
                enabled: false
            },
            xAxis: {
                type: 'datetime'
            },
            yAxis: {
                title: {
                    text: 'Euro'
                },
                type: 'linear'
            },
            series:[{
                name: 'Balance',
                data: [] as any[]
            }] 
        } as ChartOptions;
    }
}