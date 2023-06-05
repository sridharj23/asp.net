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
            rowIdProperty: {type: String, required: true}
        },
        data() {
            return {
                selectedRowId: "0"
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
        <table class="dataTable">
            <thead>
                <tr class="tableHeaderRow">
                    <th class="tableHeaderCell" v-for="col in columns">{{ col.title }}</th>
                </tr>
            </thead>
            <tbody>
                <tr :class="selectedRowId == rec[rowIdProperty] ? 'tableRow selectedRow' : 'tableRow'" v-for="rec in dataSource" :key="rowIdProperty" @click="setSelected(rec)" @dblclick="doubleClicked(rec)">
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