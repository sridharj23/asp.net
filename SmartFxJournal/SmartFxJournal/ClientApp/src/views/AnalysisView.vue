<script lang="ts">
    import {AnalysisApi} from '@/api/AnalysisApi';
    import Card from '@/components/Card.vue';
    import TabControl from '@/components/TabControl.vue';
    import Tab from '@/components/Tab.vue';
    import DataMatrix from '@/components/DataMatrix.vue';
    import ModalDialog from '@/components/ModalDialog.vue';
    import { Analysis } from '@/helpers/AnalysisHelper';
    import { usePositionStore } from '@/stores/positionstore';
    import type { TableRow } from '@/types/CommonTypes';
import { Common } from '@/helpers/Common';

    const api = new AnalysisApi();

    export default {
        setup() {
            const store = usePositionStore();
            return {store};
        },
        components: {
            TabControl, Tab, DataMatrix, ModalDialog, Card
        },
        data() {
            return {
                tabKeys: ["Actual", "Ideal", "WhatIf"],
                aspects: ["Entry", "Exit", "StopLoss", "TakeProfit", "MaxProfit"],
                selectedTab: "",
                showDialog: false,
                analysisTargetType: "",
                analysisTargetId: "",
                analysisEntries: new Map<string, TableRow[]>(),
                displayedEntries: [] as TableRow[],
                createdEntries: [] as TableRow[],
                rowDefs: Analysis.getAnalysisEntryRowDefs(),
                newEntryType: ""
            }
        },
        methods: {
            resetAnalysisEntries(){
                this.tabKeys.forEach(key => this.analysisEntries.set(key, [] as TableRow[]));
            },
            loadAnalysis() {
                this.resetAnalysisEntries();
                if (+this.store.dblClickedPositionId > 0) {
                    this.analysisTargetType = 'Position';
                    this.analysisTargetId = this.store.dblClickedPositionId;
                    api.getAnalysisForPosition(this.analysisTargetId).then(entries => {
                        entries.forEach(entry => {
                            this.analysisEntries.get(entry.analysisScenario)?.push(Analysis.convertToAnalysisRecord(entry, true));
                        });
                    });
                } else {
                    this.analysisTargetType = "";
                    this.analysisTargetId = "";
                }
            },
            selectTab(selected: string) {
                this.selectedTab = selected;
                this.displayedEntries = this.analysisEntries.get(selected)?? [];
            },
            displayDialog() {
                this.showDialog = true;
            },
            handleDialogResult(result: string) {
                if (result == "Add") {
                    if (! this.createEntry(this.newEntryType, this.selectedTab)) {
                        return;
                    }
                }
                this.showDialog = false;
                this.newEntryType = "";
            },
            createEntry(entryType : string, scenario: string) : boolean {
                let entries = this.analysisEntries.get(scenario);
                let valid = true;
                entries?.forEach((val) => {
                    if (val['analysisScenario'] == scenario && val['analyzedAspect'] == entryType) {
                        alert("Error: Entry type " + entryType + " already exists in " + scenario);
                        valid = false;
                    }
                });
                if (valid) {
                    let newRec = Analysis.createAnalysisEntry(scenario, entryType);
                    newRec['parentId'] = this.analysisTargetId;
                    newRec['parentType'] = this.analysisTargetType;
                    this.analysisEntries.get(scenario)?.push(newRec);
                    this.createdEntries.push(newRec);
                };
                return valid;
            },
            saveEntry(data: TableRow) {
                console.log(Analysis.convertToAnalysisObject(data));
            }
        },
        beforeMount() {
            this.resetAnalysisEntries();
        },
        mounted() {
            this.store.$subscribe(this.loadAnalysis);
            this.loadAnalysis();
            this.selectTab("Actual");
        }
    }

</script>

<template>
    <Card id="analysisCard">
        <template #default>
            <TabControl id="parentTab" @selection-changed="selectTab">
                <template #default>
                    <Tab :title="tabKeys[0]" :is-active="selectedTab == tabKeys[0]" :key="tabKeys[0]">
                        <DataMatrix class="analysisEntry" :row-defs="rowDefs" :col-header-key="'analyzedAspect'" :data-source="displayedEntries" :editAllowed='true' @data-save-requested="saveEntry"/>
                    </Tab>
                    <Tab :title="tabKeys[1]" :is-active="selectedTab == tabKeys[1]" :key="tabKeys[1]">
                        <DataMatrix class="analysisEntry" :row-defs="rowDefs" :col-header-key="'analyzedAspect'" :data-source="displayedEntries" :editAllowed="true"/>
                    </Tab>
                    <Tab :title="tabKeys[2]" :is-active="selectedTab == tabKeys[2]" :key="tabKeys[2]">
                        <DataMatrix class="analysisEntry" :row-defs="rowDefs" :col-header-key="'analyzedAspect'" :data-source="displayedEntries" :editAllowed="true"/>
                    </Tab>
                </template>
            </TabControl>
        </template>
        <template #footerSlot>
            <div class="flow-row">
                <button @click="displayDialog">Add New</button>
            </div>
        </template>
    </Card>
    <ModalDialog :show-buttons="['Add', 'Cancel']" :show-dialog="showDialog" :title="'New entry type for scenario : ' + selectedTab" @button-clicked="handleDialogResult">
        <template #dialogContent>
            <div class="inputRow">
                <label class="labels" for="entry_type">Entry Type</label>
                <select class="inputControls" id="entry_type" v-model="newEntryType" >
                    <option v-for="aspect in aspects" :value="aspect">{{ aspect }}</option>
                </select>
            </div>
        </template>
    </ModalDialog>
</template>

<style scoped>
    #analysisCard {
        height: 100%;
    }
    #parentTab {
        flex-grow: 1;
    }
    .analysisEntry {
        display: block;
        height: 100%;
    }
</style>