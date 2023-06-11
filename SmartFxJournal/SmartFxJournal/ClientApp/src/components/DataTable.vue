<script lang="ts">
    export interface DataColumn {
        property : string;
        title : string;
        propType: string;
    }
    export default {
        emits: ['rowSelected', 'rowDoubleClicked'],
        props : {
            columns: {type: Array<DataColumn>, required: true},
            dataSource : {type: Array<Record<string, string>>, required: true},
            rowIdProperty: {type: String, required: true},
            multiSelect: {type: Boolean}
        },
        data() {
            return {
                selectedRowId: "0",
                draggedObject: {} as Record<string, string>,
                dragStarted: false
            }
        },
        methods: {
            setSelected(rec : Record<string, string>) {
                this.selectedRowId = rec[this.rowIdProperty];
                this.$emit('rowSelected', this.selectedRowId);
            },
            doubleClicked(rec: Record<string, string>) {
                this.$emit("rowDoubleClicked", this.selectedRowId);
            },
            mouseDownHandler(event : any) {
                console.log(event);
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
            getFormat(col : DataColumn, rec: Record<string, string>): string {
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
                <tr :class="selectedRowId == rec[rowIdProperty] ? 'tableRow selectedRow' : 'tableRow'" v-for="rec in dataSource" :key="rowIdProperty" 
                    @click="setSelected(rec)" @dblclick="doubleClicked(rec)">
                    <td v-if="multiSelect" :class="'tableDataCell'">
                        <input type="checkbox" :disabled="rec['isSelected'] == 'disabled'" v-model="rec['isSelected']" true-value="true" false-value="false"/>
                    </td> 
                    <td :class="getFormat(col, rec)" v-for="col in columns">{{ rec[col.property] }}</td>
                </tr>
            </tbody>
        </table>
    </div>
</template>

<style scoped>
.tableContainer {
    display: block;
    overflow-y: scroll;
}
.dataTable {
    width: 100%;
    border-collapse: collapse;
}

.tableHeaderCell {
    color: dodgerblue;
    background-color: lavender;
    border: 2px solid dodgerblue;
    font-weight: bold;
    font-size: large;
    text-align: center;
    padding-top: .3em;
    padding-bottom: .3em;
}

.tableDataCell {
    padding-top: 4px;
    text-align: center;
    font-size: medium;
    padding-bottom: 4px;
    border-bottom: 1px solid gainsboro;
    border-left: 1px solid gainsboro;
    border-right: 1px solid gainsboro;
    cursor: move;
    user-select: none;
}

.tableRow:not(.selectedRow):hover {
    background: #efefef;
    cursor: pointer;
}
.selectedRow {
    background: lightsteelblue ;
    font-weight: bold;
}
.positiveValue {
    color: green;
    font-weight: 550;
}
.negativeValue {
    color: red;
}
</style>