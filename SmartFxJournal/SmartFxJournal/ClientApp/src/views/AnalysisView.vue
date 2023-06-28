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
                //Tab control properties
                tabKeys: ["Actual", "Ideal", "WhatIf"],
                selectedTab: "Actual",

                // Table properties
                aspects: ["Entry", "Exit", "StopLoss", "TakeProfit", "MaxProfit"],
                analyzedPositionId: "",
                selectedEntry: {} as TableRow,
                backupEntry: {} as TableRow,
                analysisEntries: new Map<string, TableRow[]>(),
                displayedEntries: [] as TableRow[],
                rowDefs: Analysis.getAnalysisEntryRowDefs(),

                // Dialog properties
                showDialog: false,
                newEntryType: ""
            }
        },
        computed: {
            hasSelectedEntry() {
                return this.selectedEntry != undefined && Object.keys(this.selectedEntry).length > 0;
            },
            isEditable() {
                return this.hasSelectedEntry && this.selectedEntry['isInEdit'] == false;
            },
            isEditing() {
                return this.hasSelectedEntry && this.selectedEntry['isInEdit'] == true;
            }
        },
        methods: {
            resetAnalysisEntries(){
                this.tabKeys.forEach(key => this.analysisEntries.set(key, [] as TableRow[]));
            },
            loadAnalysis() {
                this.resetAnalysisEntries();
                if (+this.store.dblClickedPositionId > 0) {
                    this.analyzedPositionId = this.store.dblClickedPositionId;
                    api.getAnalysisForPosition(this.analyzedPositionId).then(entries => {
                        entries.forEach(entry => {
                            this.analysisEntries.get(entry.analysisScenario)?.push(Analysis.convertToAnalysisRecord(entry));
                        });
                    });
                } else {
                    this.analyzedPositionId = "";
                }
            },
            selectTab(selected: string) {
                this.selectedTab = selected;
                this.displayedEntries = this.analysisEntries.get(selected)?? [];
            },
            setSelectedData(data : TableRow) {
                if ( this.isEditing ) {
                    this.cancelEditing();
                }
                this.selectedEntry = data;
            },
            editData() {
                Object.keys(this.selectedEntry).forEach(k => this.backupEntry[k] = this.selectedEntry[k]);
                this.selectedEntry['isInEdit'] = true;
            },
            cancelEditing() {
                Object.keys(this.backupEntry).forEach(k => this.selectedEntry[k] = this.backupEntry[k]);
                this.backupEntry = {} as TableRow;
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
                    newRec['positionId'] = this.analyzedPositionId;
                    this.analysisEntries.get(scenario)?.push(newRec);
                };
                return valid;
            },
            saveEntry() {
                console.log(this.selectedEntry);
                this.selectedEntry['isInEdit'] = false;
                this.backupEntry = {} as TableRow;
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
                        <DataMatrix class="analysisEntry" :row-defs="rowDefs" :dataKey="'analyzedAspect'" :data-source="displayedEntries" @data-selected="setSelectedData"/>
                    </Tab>
                    <Tab :title="tabKeys[1]" :is-active="selectedTab == tabKeys[1]" :key="tabKeys[1]">
                        <DataMatrix class="analysisEntry" :row-defs="rowDefs" dataKey="analyzedAspect" :data-source="displayedEntries"/>
                    </Tab>
                    <Tab :title="tabKeys[2]" :is-active="selectedTab == tabKeys[2]" :key="tabKeys[2]">
                        <DataMatrix class="analysisEntry" :row-defs="rowDefs" dataKey="analyzedAspect" :data-source="displayedEntries"/>
                    </Tab>
                </template>
            </TabControl>
        </template>
        <template #footerSlot>
            <div class="flow-row">
                <button @click="showDialog = true">Add</button>
                <button :disabled="!isEditable" @click="editData">Edit</button>
                <button :disabled="!isEditing" @click="saveEntry">Save</button>
                <button :disabled="!isEditing" @click="cancelEditing">Cancel</button>
            </div>
        </template>
    </Card>
    <ModalDialog :show-buttons="['Add', 'Cancel']" :show-dialog="showDialog" :title="'New ' + selectedTab + ' entry : '" @button-clicked="handleDialogResult">
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