<script lang="ts">
    import {AnalysisApi} from '@/api/AnalysisApi';
    import type {AnalysisEntry, RowDef} from '@/types/CommonTypes';
    import TabControl from '@/components/TabControl.vue';
    import Tab from '@/components/Tab.vue';
    import DataMatrix from '@/components/DataMatrix.vue';
import { Common } from '@/types/CommonTypes';

    const api = new AnalysisApi();

    export default {
        props: ['analysisEntry'],
        components: {
            TabControl, Tab, DataMatrix
        },
        data() {
            return {
                selectEntryType: "",
                selectEntryId: "",
                analysisEntries: [] as Record<string,string>[],
                rowDefs: Common.getAnalysisEntryRowDefs()
            }
        },
        watch: {
            analysisEntry: function(newVal : string, oldVal : string) {
                if (newVal) {
                    let arr = newVal.split(":");
                    this.selectEntryType = arr[0];
                    this.selectEntryId = arr[1];
                } else {
                    this.selectEntryType = "";
                    this.selectEntryId = "";
                }
                api.getAnalysisForPosition(this.selectEntryId).then(entries => {
                    this.analysisEntries = [];
                    entries.forEach(entry => this.analysisEntries.push(Common.convertToAnalysisRecord(entry)));
                    console.log(this.analysisEntries);
                });
            }
        }
    }
</script>

<template>
    <TabControl id="parentTab">
        <template #default>
            <Tab :title="'Actual'" :is-active="true">
                <DataMatrix id="actualEntries" :data-source="analysisEntries" :col-header-key="'analyzedAspect'" :row-defs="rowDefs"/>
            </Tab>
            <Tab :title="'Ideal'"/>
            <Tab :title="'What-If'"/>
        </template>
    </TabControl>
</template>

<style scoped>
    #parentTab {
        height: 100%;
    }
    #actualEntries {
        display: block;
        height: 100%;
    }
</style>