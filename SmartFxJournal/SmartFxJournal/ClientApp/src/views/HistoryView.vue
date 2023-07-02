<script lang="ts">
    import { PostionsAPI } from '@/api/PositionsApi';
    import { Common } from '@/helpers/Common';
    import { usePositionStore } from '@/stores/positionstore';
    import { useAccountStore } from '@/stores/accountstore';
    import DataTable from '@/components/DataTable.vue';
    
    const api = new PostionsAPI()
    
    export default {
        setup() {
            const store = usePositionStore();
            const accountstore = useAccountStore();
            return {store, accountstore};
        },
        emits: ['positionSelected'],
        data() {
            return {
                positions: [] as Record<string, string>[],
                columns: Common.getPositionColumns(),
                selectedRowId: "0"
            };
        },
        methods: {
            loadPostions() {
                let params = new Map();
                params.set("accountNo", this.accountstore.selectedAccount);
                api.getAll(params).then((resp) => {
                    resp.forEach(pos => this.positions.push(Common.convertPositionToRecord(pos)));
                });
            },
            handleRowSelection(position: string) {
                this.store.lastSelectedPositionId = position;
            },
            handleRowDblClick(position: Record<string, string>) {
                this.store.dblClickedPosition = position;
                this.store.dblClickedPositionId = position['positionId'];
                this.$emit('positionSelected', position);
            }
    },
    mounted() {
        this.loadPostions();
    },
    components: { DataTable }
}
</script>

<template>
    <div id="positionsTable">
        <DataTable class="dataTable" :columns="columns" :dataSource="positions" :rowIdProperty="'positionId'" :selectedRowInitial="store.lastSelectedPositionId.toString()" @rowSelected="handleRowSelection" @rowDoubleClicked="handleRowDblClick"/>
    </div>
</template>

<style scoped>
    #positionsTable {
        flex-grow: 1;
    }
</style>