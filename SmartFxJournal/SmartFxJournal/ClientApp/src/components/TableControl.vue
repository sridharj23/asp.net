<script lang="ts">
    export default {
        props : {
            headerSource : {type : Array},
            dataSource : { type: Array}
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
                    <td class="tableDataCell" v-for="val in entry">{{ val }}</td>
                </tr>
            </tbody>
        </table>
    </div>
</template>

<style scoped>
    .tableContainer {
        display: block;
        height: 100%;
        overflow-y: scroll;
    }
    table {
        width: 100%;
        height: 90vh;
    }
    .tableHeaderRow, .tableRow {
        width: 100%;
    }
    .tableHeaderCell {
        color: dodgerblue;
        font-weight: bold;
        font-size: large;
        text-align: center;
        padding-top: .3em;
        padding-bottom: .3em;
        border-bottom: 2px solid dodgerblue;
    }
    .tableDataCell {
        padding-top: 4px;
        padding-bottom: 4px;
        border: 1px solid gainsboro;
    }



</style>