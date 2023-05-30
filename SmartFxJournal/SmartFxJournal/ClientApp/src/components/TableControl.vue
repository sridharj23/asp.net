<script lang="ts">
import { computed } from '@vue/reactivity';

    export default {
        props : {
            headerSource : Array,
            dataSource : { type: Array},
            dataKeys : Array
        }
    }
</script>

<template>
    <div class="tableContainer">
        <table>
            <thead>
                <tr class="tableHeaderRow">
                    <th class="tableHeaderCell" v-for="head in headerSource">{{ head }}</th>
                </tr>
            </thead>
            <tbody>
                <tr class="tableRow" v-for="entry in dataSource">
                    <td class="tableDataCell" v-for="key in entry">{{ key }}</td>
                </tr>
            </tbody>
        </table>
    </div>
</template>

<style scoped>
    .tableContainer {
        display: block;
        height: 100%;
        max-height: 89vh;
        overflow-y: scroll;
    }
    table {
        width: 100%;
    }
    .tableHeaderRow, .tableRow {
        width: 100%;
    }
    .tableHeaderCell {
        color: dodgerblue;
        background-color: rgba(0, 191, 255, 0.178);
        font-weight: bold;
        font-size: large;
        text-align: center;
        padding-top: .3em;
        padding-bottom: .3em;
    }
    .tableDataCell {
        padding-top: 4px;
        padding-bottom: 4px;
        border: 1px solid gainsboro;
    }



</style>