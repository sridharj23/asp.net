<script lang="ts">
    import { Chart, type ChartOptions } from 'highcharts-vue';

    export default {
        components: {
            equityChart: Chart
        },
        data() {
            return {
                chartOptions: {
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
                        name:'Balance',
                        data: [{
                            x: Date.now()- 2436001000,
                            y: 5
                        }, {
                            x: Date.now() - (5*243600100),
                            y: 10
                        }, {
                            x: Date.now() - 4*24360010,
                            y: -20
                        }, {
                            x: Date.now(),
                            y: 50
                        }]
                    }]
                }
            }
        }
    }

</script>

<template>
    <div>
        <equityChart id="eChart" class="hc" :options="chartOptions"/>
    </div>
</template>

<style scoped>
    #eChart {
        display: block;
        height: 99%;
    }
</style>