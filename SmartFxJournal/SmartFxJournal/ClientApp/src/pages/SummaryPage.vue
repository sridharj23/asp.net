<script setup>
    import SettingsPage from '@/pages/SettingsPage.vue'
    import Card from '@/components/Card.vue';
</script>
<template>
    <div id="SummaryPage">
        <Card id="SummaryTop">
            <template #default>
                <p>A paragraph for the main content.</p>
            </template>
        </Card>
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
        width: auto;
    }
</style>