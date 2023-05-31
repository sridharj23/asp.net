<script lang="ts">
    import { Chart } from 'highcharts-vue';
    import type {DefaultChartOptions} from '@/helpers/ChartHelper'
    
    export default {
        components: {
            highcharts: Chart 
        },
        data() {
            return {
                chartOptions: {
                    chart: {
                        type: 'column'
                    },
                    title: {
                        text: 'P & L Overview'
                    },
                    legend: {
                        enabled: false
                    },
                    xAxis: {

                        type: 'category'
                    },
                    yAxis: {
                        title: {
                            text: 'Euro'
                        },
                    },
                    series: [{
                        name: 'Net Profit',
                        data: [{
                                name: '2022',
                                y: 100,
                                color: '#B0BF1A',
                            }, {
                                name: 2023,
                                y: 200,
                                color: '#B0BF1A',
                                drilldown: '2023'
                            }, {
                                name: 'April-2023',
                                y: -75,
                                color: '#BF4F51',
                                drilldown: 'April-2023'
                            }, {
                                name: 'May-2023',
                                y:300,
                                color: '#B0BF1A',
                                drilldown: 'May-2023'
                            }
                        ]
                    }]
                }
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