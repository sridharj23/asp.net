<script lang="ts">
import { PostionsAPI } from '@/api/PositionsApi';
import type { Position } from '@/api/PositionsApi';

const api = new PostionsAPI();

export default {
    data() {
        return {
            positions : [] as Position[]
        }
    },

    methods : {
        loadPostions() {
            api.getAll().then((resp) => {
                console.log(resp);
                resp.forEach(p => this.positions.push(p));
            })
        }
    },

    mounted() {
        this.loadPostions();
    }
}
</script>

<template>
    <div id="tableContainer">
        <table>
            <thead>
                <tr>
                    <th>Account</th>
                    <th>Position</th>
                    <th>Symbol</th>
                    <th>Size</th>
                    <th>Type</th>
                    <th>Executed Price</th>
                    <th>Fees</th>
                    <th>Gross Profit</th>
                    <th>NetProfit</th>
                    <th style="width: 235px;">Execution Time</th>
                </tr>
            </thead>
            <tbody>
                <tr v-for="position in positions" :key="position.positionId">
                    <td>{{ position.accountNo }}</td>
                    <td>{{ position.positionId }}</td>
                    <td>{{ position.symbol }}</td>
                    <td>{{ position.openedOrders[0].filledVolume }}</td>
                    <td>{{ position.openedOrders[0].direction }}</td>
                    <td>{{ position.openedOrders[0].price }}</td>
                    <td>{{ position.fees }}</td>
                    <td>{{ position.grossProfit }}</td>
                    <td>{{ position.netProfit }}</td>
                    <td style="width: 235px;">{{ position.openedOrders[0].executionTime }}</td>
                </tr>
            </tbody>
        </table>
    </div>
</template>

<style scoped>
    table {
        display: block;
        border: 1px solid black;
    }
    thead {
        display: block;
        text-align: center;
        margin-right: 1em;
    }
    th {
        border-bottom: 2px solid dodgerblue;
    }
    td {
        border-bottom: 1px solid gainsboro;
    }
    td, th {
        border-left: 1px solid gainsboro;
        width: 10%;
        padding: 0.5em;
    }
    tbody {
        display: block;
        overflow-y: scroll;
        max-height: 300px;
    }
</style>