<script lang="ts">
    import { Chart, type ChartOptions } from 'highcharts-vue';
    import {ChartHelper} from '@/helpers/ChartHelper'
    
    const boldPositive = '#009933';
    const boldNegative = '#e60000';
    const positive = '#99cc00';
    const negative = '#e65c00';

    export default {
        components: {
            highcharts: Chart 
        },
        created() {
            this.initializeChartOptions()
        },
        methods: {
            initializeChartOptions() {
                let opt = ChartHelper.getDefaultColumnOptions();
                opt.series[0].data.push({name: '2022', y: 100, color: boldPositive,});
                opt.series[0].data.push({name: '2023', y: 200, color: boldPositive,});
                opt.series[0].data.push({name: 'April-2023', y: -75, color: boldNegative,});
                opt.series[0].data.push({name: 'May-2023', y: 300, color: boldPositive,});

                opt.series.push({name: 'Longs', data: [] as any});
                opt.series[1].data.push({name: '2022', y: 250, color: positive,});
                opt.series[1].data.push({name: '2023', y: 100, color: positive,});
                opt.series[1].data.push({name: 'April-2023', y: 75, color: positive,});
                opt.series[1].data.push({name: 'May-2023', y: 100, color: positive,});

                opt.series.push({name: 'Shorts', data: [] as any});
                opt.series[2].data.push({name: '2022', y: -150, color: negative,});
                opt.series[2].data.push({name: '2023', y: 100, color: positive,});
                opt.series[2].data.push({name: 'April-2023', y: -150, color: negative,});
                opt.series[2].data.push({name: 'May-2023', y: 200, color: positive,});

                this.chartOptions = opt;
            }
        },
        data() {
            return {
                chartOptions: {} as ChartOptions
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