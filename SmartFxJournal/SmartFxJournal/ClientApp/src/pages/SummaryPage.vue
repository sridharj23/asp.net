<script setup>
    import SettingsPage from '@/pages/SettingsPage.vue'
    import Card from '@/components/Card.vue';
    import AggregateSummaryView from '@/views/AggregateSummaryView.vue';
    import EquityView from '@/views/EquityView.vue'
</script>
<template>
    <div id="SummaryPage">
        <div id="SummaryTop">
            <Card id="TopLeft">
                <AggregateSummaryView id="SummaryView"/>
            </Card>
            <Card id="TopRight">
                <template #default>
                    <EquityView id="GrowthChart"/>
                </template>
            </Card>
        </div>

        <div id="MiddleContainer">
            <Card id="MiddleLeft">
                <template #default>
                    <p>A paragraph for the main content.</p>
                    <p>And another one.</p>
                </template>
            </Card>
            <Card id="MiddleRight">
                <template #default>
                    <SettingsPage id="SettingsView"/>
                </template>
            </Card>
        </div>
    </div>
</template>

<style scoped>
    #SummaryPage {
        display: flex;
        flex-direction: column;
    }
    #SummaryTop {
        width: 100%;
        height: 30%;
        display: flex;
        flex-direction: row;
    }
    #TopLeft {
        width: 50%;
        margin-right: 5px;
    }
    #TopRight {
        width: 50%;
        margin-left: 5px;
    }
    #MiddleContainer {
        display: flex;
        flex-direction: row;
        flex-grow: 1;
        width: 100%;
        height: 69%;
        margin-top: 10px;
    }
    #MiddleLeft {
        width: 70%;
        margin-right: 5px;
        display: flex;
        flex-direction: column;
    }
    #MiddleRight {
        width: 30%;
        margin-left: 5px;
        display: flex;
        flex-direction: column;
    }
    #SettingsView {
        flex-grow: 1;
        height: 100%;
    }
    #SummaryView {
        height: 100%;
    }
    #GrowthChart {
        height: 100%;
    }
</style>