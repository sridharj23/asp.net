<script lang="ts">
    import { useAccountStore } from '@/stores/accountstore';
    import { SummaryAPI } from '@/api/SummaryApi';

    export default {
        setup() {
            const api = new SummaryAPI();
            const store = useAccountStore();
            return {api, store};
        },
        data() {
            return {
                aggregates: {} as Map<string, Array<Array<string>>>,
                titles : {} as string[]
            }
        },
        methods: {
            loadAggregates() {
                this.api.getAggregates(this.store.selectedAccount).then(res => {
                    this.aggregates = res;
                    this.titles = Object.keys(res);
                    console.log(this.aggregates);
                });

            }
        },
        mounted() {
            this.store.$subscribe(this.loadAggregates);
            if(this.store.selectedAccount != "0") {
                this.loadAggregates();
            }
        }
    }
</script>

<template>
    <div id="aggregateContainer">
        <template v-for="title in titles" class="tableRow"> 
            <div class="title"> {{ title }}</div>
            <table id="dataTable">
                <tr v-for="(entry, index) in aggregates[title]">
                    <template v-if="index == 0" v-for="col in entry">
                        <th v-if="index == 0" class="tableHeaderCell minWidthCell">{{ col }}</th>
                    </template>
                    <template v-else v-for="(col, index) in entry">
                        <td :class="index > 0 ? 'tableDataCell' : 'tableHeaderCell'">{{ col }}</td>
                    </template>
                </tr>
            </table>
        </template>
    </div>
</template>

<style>
    #aggregateContainer {
        display: grid;
        grid-template-columns: 50px 200px 50px 200px;
    }
    #dataTable {
        width: auto;
        border-collapse: collapse;
    }
    .title {
        color: dodgerblue;
        font-weight: bold;
        padding: 0.5em;
    }
</style>