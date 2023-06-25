<script lang="ts">
    import type { RowDef, TableRow } from '@/types/CommonTypes';

    export default {
        props: {
            colHeaderKey: {type: String, required: true},
            rowDefs: {type: Array<RowDef>, required: true},
            dataSource: {type: Array<TableRow>, required: true},
            editAllowed: {type: Boolean, default: false}
        },
        emits: ['dataSaveRequested', 'valueChanged'],
        data() {
            return {
                backup: {} as TableRow
            }
        },
        methods: {
            saveData(data: TableRow) {
                data['isInEdit'] = false;
                this.backup = {} as TableRow;
                this.$emit('dataSaveRequested', data);
            },
            editData(data: TableRow) {
                data['isInEdit'] = true;
                Object.keys(data).forEach(k => this.backup[k] = data[k]);
                //console.log(this.backup);
            },
            cancelEditing(data: TableRow) {
                Object.keys(this.backup).forEach(k => data[k] = this.backup[k]);
                data['isInEdit'] = false;
            },
            valueChanged(data: TableRow, key: string) {
                this.$emit('valueChanged', data, key);
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
                    <th class="tableHeaderCell" v-for="data in dataSource">{{ data[colHeaderKey] }}</th>
                </tr>
            </thead>
            <tbody>
                <tr v-for="row in rowDefs" v-show="row.property != colHeaderKey">
                    <td class="tableHeaderCell rowHeaderCell minWidthCell">{{ row.title }}</td>
                    <template v-for="data in dataSource">
                        <td v-if="editAllowed != true || data['isInEdit'] != true" class="tableDataCell minWidthCell">{{ data[row.property] }}</td>
                        <td v-else-if="row.editable && row.dataType=='checkbox'" class="tableDataCell minWidthCell">
                            <input type="checkbox" v-model="data[row.property]" @change="valueChanged(data, row.property)"/>
                        </td>
                        <td v-else-if="row.editable" class="tableDataCell minWidthCell">
                            <input :type="row.dataType" v-model="data[row.property]" @change="valueChanged(data, row.property)"/>
                        </td>
                    </template>
                </tr>
                <tr v-if="editAllowed">
                    <th class="tableHeaderCell minWidthCell">>></th>
                    <td v-for="data in dataSource" class="tableDataCell minWidthCell">
                        <template v-if="data['isReadOnly'] != true">
                            <input type="button" title="Edit" value="E" :disabled="data['isInEdit'] == true" @click="editData(data)"/>
                            <input type="button" title="Save" value="S" :disabled="data['isInEdit'] == false" @click="saveData(data)"/>
                            <input title="Cancel" type="button" value="X" :disabled="data['isInEdit'] == false" @click="cancelEditing(data)"/>
                        </template>
                    </td>
                </tr>
            </tbody>
        </table>
    </div>
</template>

<style scoped>
</style>