<script lang="ts">
    import { useAccountStore } from '@/stores/accountstore';
    import { SummaryAPI, type EquityDataPoint } from '@/api/SummaryApi';
    import { Chart, type ChartOptions } from 'highcharts-vue';
    import {ChartHelper} from '@/helpers/ChartHelper';

    export default {
        setup() {
            const api = new SummaryAPI();
            const store = useAccountStore();
            return {api, store};
        },
        components: {
            equityChart: Chart
        },
        mounted() {
            this.store.$subscribe(this.loadEquityCurve);
        },
        methods: {
            loadEquityCurve() {
                this.api.getEquityCurve(this.store.selectedAccount).then( (resp) => {
                    this.chartOptions.series[0].data = [];
                    resp?.dataPoints.forEach( dp => {
                        this.chartOptions.series[0].data.push({
                            x: dp.timeStamp,
                            y: dp.equity
                        })
                    });
                });
            }
        },
        data() {
            return {
                chartOptions: ChartHelper.getDefaultLineOptions()
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