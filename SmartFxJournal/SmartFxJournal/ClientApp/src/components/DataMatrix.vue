<script lang="ts">
    import type { RowDef, ColumDef } from '@/types/CommonTypes';

    export default {
        props: {
            colHeaderKey: {type: String, required: true},
            rowDefs: {type: Array<RowDef>, required: true},
            dataSource: {type: Array<Record<string, string>>, required: true}
        }
    }
</script>

<template>
    <div class="tableContainer">
        <table class="dataTable" id="theTable" v-if="dataSource.length > 0">
            <thead>
                <tr class="tableHeaderRow">
                    <th class="tableHeaderCell"></th>
                    <th class="tableHeaderCell" v-for="data in dataSource">{{ data[colHeaderKey] }}</th>
                </tr>
            </thead>
            <tbody>
                <tr v-for="row in rowDefs" v-show="row.property != colHeaderKey">
                    <td class="tableHeaderCell rowHeaderCell">{{ row.title }}</td>
                    <td v-if="!row.editable" class="tableDataCell" v-for="data in dataSource"  >{{ data[row.property] }}</td>
                    <td v-else-if="row.editable && row.dataType == 'text'" v-for="data in dataSource" class="tableDataCell">
                        <input type="text" v-model="data[row.property]"/>
                    </td>
                </tr>
            </tbody>
        </table>
    </div>
</template>

<style scoped>
</style>