<script lang="ts">
    import type { RowDef, TableRow } from '@/types/CommonTypes';
    import ModalDialog from '@/components/ModalDialog.vue';

    export default {
        components: {
            ModalDialog
        },
        props: {
            dataKey: {type: String, required: true},
            rowDefs: {type: Array<RowDef>, required: true},
            dataSource: {type: Array<TableRow>, required: true},
            resetSelection: {type: Boolean, required: true},
        },
        emits: [ 'valueChanged', 'dataSelected'],
        watch: {
            resetSelection(newVal : Boolean, oldVal: Boolean) {
                if (newVal) {
                    this.selectedCol = "";
                }
            }
        },
        data() {
            return {
                selectedCol: "",
                selectedData: {} as TableRow,
                selectedProperty: "",
                showDialog: false,
                selectOptions: [] as string[],
                multiSelectResult: [] as string[]
            }
        },
        methods: {
            valueChanged(data: TableRow, key: string) {
                this.$emit('valueChanged', data, key);
            },
            dataSelected(data: TableRow) {
                if (this.selectedCol == data[this.dataKey]) {
                    this.selectedCol = "";
                    this.selectedData = {} as TableRow;
                    this.$emit('dataSelected', undefined);
                } else  {
                    this.selectedCol = data[this.dataKey].toString();
                    this.selectedData = data;
                    this.$emit('dataSelected', data);
                }
            },
            multiSelectRequest(key: string, row: RowDef, data: TableRow) {
                this.selectOptions = row.inputHelp as string[];
                this.multiSelectResult = data[key].toString().split(',');
                this.selectedProperty = key;
                this.showDialog = true;
            },
            handleDialogResult() {
                this.showDialog = false;
                this.selectedData[this.selectedProperty] = this.multiSelectResult.toString();
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
                            <select v-model="data[row.property]" @change="valueChanged(data, row.property)" style="width: 175px;">
                                <option v-for="opt in row.inputHelp" :value="opt">{{opt}}</option>
                            </select>
                        </td>
                        <td v-else-if="row.dataType=='multiselect'" class="tableDataCell minWidthCell">
                            <input type="text" v-model="data[row.property]" disabled="true"/>
                            <input type="button" value="V" @click="multiSelectRequest(row.property, row, data)"/>
                        </td>
                        <td v-else class="tableDataCell minWidthCell">
                            <input :type="row.dataType" v-model="data[row.property]" @change="valueChanged(data, row.property)"/>
                        </td>
                    </template>
                </tr>
            </tbody>
        </table>
        <ModalDialog :show-buttons="['Ok', 'Cancel']" :show-dialog="showDialog" :title="'Select values : '" @button-clicked="handleDialogResult">
            <template #dialogContent>
                <div class="inputRow">
                    <label class="labels" for="entry_type">Entry Type</label>
                    <select class="inputControls" id="entry_type" v-model="multiSelectResult" multiple>
                        <option v-for="opt in selectOptions" :value="opt">{{ opt }}</option>
                    </select>
                </div>
            </template>
        </ModalDialog>
    </div>
</template>
