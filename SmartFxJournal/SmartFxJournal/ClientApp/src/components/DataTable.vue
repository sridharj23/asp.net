<script lang="ts">
import { type ColumDef } from '@/types/CommonTypes';
import TrafficLight from './TrafficLight.vue';

export default {
        components: {TrafficLight},
        emits: ['rowSelected', 'rowDoubleClicked'],
        props : {
            columns: {type: Array<ColumDef>, required: true},
            dataSource : {type: Array<Record<string, string>>, required: true},
            rowIdProperty: {type: String, required: true},
            multiSelect: {type: Boolean},
            selectedRowInitial: {type: String, default: "0"}
        },
        data() {
            return {
                selectedRowId: "0",
                draggedObject: {} as Record<string, string>,
                dragStarted: false
            }
        },
        computed: {
            getSelectedRowId() : string {
                if (this.selectedRowId != "0" ) {
                    return this.selectedRowId;
                } else if (this.selectedRowInitial != "0") {
                    return this.selectedRowInitial;
                }
                return "0";
            }
        },
        methods: {
            setSelected(rec : Record<string, string>) {
                this.selectedRowId = rec[this.rowIdProperty];
                this.$emit('rowSelected', this.selectedRowId);
            },
            doubleClicked(rec: Record<string, string>) {
                this.$emit("rowDoubleClicked", rec);
            },
            mouseDownHandler(event : any) {
                let tab = document.getElementById("theTable");
                tab?.addEventListener('drag', this.dragEventHandler);
                this.draggedObject = event;
            },
            mouseUpHandler(event: any) {
                this.dragStarted = false;
                let tab = document.getElementById("theTable");
                tab?.removeEventListener('drag', this.dragEventHandler);
            },
            dragEventHandler(event: DragEvent) {
                console.log(this.rowIdProperty + " : " + this.draggedObject[this.rowIdProperty]);
            },
            getFormat(col : ColumDef, rec: Record<string, string>): string {
                let cellClass = 'tableDataCell';
                if (col.propType == 'Currency') {
                    let val = Number(rec[col.property]);
                    if (val > 0 ) {
                        cellClass += ' positiveValue';
                    } else if(val < 0) {
                        cellClass += ' negativeValue';
                    }
                }
                return cellClass;
            }
        }
    }
</script>

<template>
    <div class="tableContainer">
        <table class="dataTable" id="theTable">
            <thead>
                <tr class="tableHeaderRow">
                    <th v-if="multiSelect" class="tableHeaderCell">Select</th>
                    <th class="tableHeaderCell" v-for="col in columns">{{ col.title }}</th>
                </tr>
            </thead>
            <tbody>
                <tr :class="getSelectedRowId == rec[rowIdProperty] ? 'tableRow selectedRow' : 'tableRow'" v-for="rec in dataSource" :key="rowIdProperty" 
                    @click="setSelected(rec)" @dblclick="doubleClicked(rec)">
                    <td v-if="multiSelect" :class="'tableDataCell'">
                        <input type="checkbox" :disabled="rec['isSelected'] == 'disabled'" v-model="rec['isSelected']" true-value="true" false-value="false"/>
                    </td>
                    <template v-for="col in columns">
                        <td v-if="col.propType == 'trafficlight'" class="tableDataCell">
                            <TrafficLight :currenValue="rec[col.propType]" :lightValues="col.options??[]"/>
                        </td>
                        <td v-else :class="getFormat(col, rec)">{{ rec[col.property] }}</td>
                    </template>
                </tr>
            </tbody>
        </table>
    </div>
</template>
