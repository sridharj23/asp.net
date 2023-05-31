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
            plotOptions: {
                line: {
                    color: '#1E90FF',
                    dataLabels: {
                        enabled: true
                    },
                    enableMouseTracking: false,
                    lineWidth: 3,
                    marker: {
                        enabled: true,
                        states: {
                            hover: {
                                enabled: false
                            }
                        }
                    }
                }
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