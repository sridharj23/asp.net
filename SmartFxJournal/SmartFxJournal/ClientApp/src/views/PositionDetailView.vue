<script lang="ts">
    import { PostionsAPI } from '@/api/PositionsApi';
    import { Common } from '@/helpers/Common';
    import {type ExecutedOrder, type ColumDef } from '@/types/CommonTypes';
    import { usePositionStore } from '@/stores/positionstore';
    import DataTable from '@/components/DataTable.vue';
    import Card from '@/components/Card.vue';
    
    const api = new PostionsAPI()
    const cols : ColumDef[] = [
        {property: "type", title: "Type", propType: "default"}
    ];
    export default {
        setup() {
            const store = usePositionStore();
            return {store};
        },
        components: {DataTable, Card},
        emits: ['detailEntrySelected'],
        data() {
            return {
                positions: [] as Record<string, string>[],
                columns: Common.getPositionColumns(cols),
                selectedRowId: "0",
                selectedRowType : ""
            };
        },
        mounted() {
            this.store.$subscribe(this.loadPosition);
            this.loadPosition();
        },
        computed: {
            isAnalysisComplete() {
                if (this.positions.length > 0) {
                    return this.positions[0]['analysisStatus'] == "Complete";
                }
                return true;
            }
        },
        methods: {
            loadPosition() {
                this.positions = [];
                
                let pos = this.store.dblClickedPositionId;
                if (pos != "0") {
                    api.get(this.store.dblClickedPositionId).then(pos => {
                        let rec = Common.convertPositionToRecord(pos);
                        rec['type'] = "Position";
                        this.positions.push(rec);
                        pos.executedOrders.forEach(order => {
                            this.positions.push(this.convertOrderToPositionRecord(order));
                        });
                    });
                }
            },
            convertOrderToPositionRecord(order : ExecutedOrder) {
                let rec : Record<string, string> = {
                    type: 'Order',
                    positionId: order.orderId.toString(),
                    symbol: order.symbol,
                    direction: order.direction,
                    commission: order.commission.toFixed(2),
                    swap: order.swap.toFixed(2),
                    grossProfit: order.grossProfit.toFixed(2),
                    balanceAfter: order.balanceAfter.toFixed(2),
                    netProfit: '',
                };

                if (order.isClosing) {
                    rec['volume'] = (order.closedVolume/10000000).toFixed(2);
                    rec['exitPrice'] = order.executionPrice.toFixed(5);
                    rec['orderClosedAt'] = new Date(order.orderExecutedAt).toLocaleString();
                } else {
                    rec['volume'] = (order.filledVolume/10000000).toFixed(2);
                    rec['entryPrice'] = order.executionPrice.toFixed(5);
                    rec['orderOpenedAt'] = new Date(order.orderExecutedAt).toLocaleString();
                }
                return rec;
            },
            handleRowSelect(pos : string) {
                this.positions.forEach (entry => {
                    if(entry.positionId == pos) {
                        this.$emit('detailEntrySelected', entry);
                    }
                })
            },
            markAnalysisComplete() {
                api.get(this.store.dblClickedPositionId).then(pos => {
                    pos.analysisStatus = "Complete";
                    pos.executedOrders = [];
                    api.update(pos).then(p => this.loadPosition());
                });
            }
        }
    }
</script>

<template>
    <Card id="positionDetailCard">
        <template #default>
            <div id="tableWrapper">
                <DataTable class="dataTable" :columns="columns" :dataSource="positions" :rowIdProperty="'positionId'" @rowSelected="handleRowSelect"/>
            </div>
        </template>
        <template #footerSlot>
            <div id="buttonRow" class="flow-row">
                <button @click="markAnalysisComplete" :disabled="isAnalysisComplete">Mark Analysis Complete</button>
            </div>
        </template>
    </Card>
</template>

<style scoped>
    #positionDetailCard{
        height: 100%;
    }
    .dataTable {
        max-height: 40vh;
    }
    #tableWrapper {
        flex-grow: 1;
        height: 100%;
    }
</style>