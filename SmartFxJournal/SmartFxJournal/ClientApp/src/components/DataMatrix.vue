<script lang="ts">
    import type { RowDef, TableRow } from '@/types/CommonTypes';

    export default {
        props: {
            dataKey: {type: String, required: true},
            rowDefs: {type: Array<RowDef>, required: true},
            dataSource: {type: Array<TableRow>, required: true},
        },
        emits: [ 'valueChanged', 'dataSelected'],
        data() {
            return {
                selectedCol: ""
            }
        },
        methods: {
            valueChanged(data: TableRow, key: string) {
                this.$emit('valueChanged', data, key);
            },
            dataSelected(data: TableRow) {
                if (this.selectedCol == data[this.dataKey]) {
                    this.selectedCol = "";
                    this.$emit('dataSelected', undefined);
                } else  {
                    this.selectedCol = data[this.dataKey].toString();
                    this.$emit('dataSelected', data);
                }
            }
        }
    }
</script>

<template>
    <div class="tableContainer">
        <table class="dataTable" id="theTable" v-if="dataSource.length > 0">
            <thead>
                <tr class="tableHeaderRow">
                    <th class="tableHeaderCell"></th>
                    <th :class="data[dataKey] == selectedCol ? 'tableHeaderCell selectedHeader' : 'tableHeaderCell'" v-for="data in dataSource" @click="dataSelected(data)">{{ data[dataKey] }}</th>
                </tr>
            </thead>
            <tbody>
                <tr v-for="row in rowDefs" v-show="row.property != dataKey">
                    <td class="tableHeaderCell rowHeaderCell minWidthCell">{{ row.title }}</td>
                    <template v-for="data in dataSource">
                        <td v-if="data['isInEdit'] != true || ! row.editable" class="tableDataCell minWidthCell">{{ data[row.property] }}</td>
                        <td v-else-if="row.dataType=='checkbox'" class="tableDataCell minWidthCell">
                            <input type="checkbox" v-model="data[row.property]" @change="valueChanged(data, row.property)"/>
                        </td>
                        <td v-else-if="row.dataType=='select'" class="tableDataCell minWidthCell">
                            <select v-model="data[row.property]">
                                <option value="unknown">Unknown</option>
                            </select>
                        </td>
                        <td v-else-if="row.dataType=='multiselect'" class="tableDataCell minWidthCell">
                            <input type="text" v-model="data[row.property]" disabled="true"/>
                            <input type="button" value="V"/>
                        </td>
                        <td v-else class="tableDataCell minWidthCell">
                            <input :type="row.dataType" v-model="data[row.property]" @change="valueChanged(data, row.property)"/>
                        </td>
                    </template>
                </tr>
            </tbody>
        </table>
    </div>
</template>
