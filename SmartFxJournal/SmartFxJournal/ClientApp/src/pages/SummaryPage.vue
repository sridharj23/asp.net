<script setup>
    import AccountsView from '@/views/AccountsView.vue';
    import Card from '@/components/Card.vue';
</script>
<template>
    <div id="SummaryPage">
        <Card id="SummaryTop">
            <template #header>
                <div class="chead active titleBar">Summary Top</div>
            </template>

            <template #default>
            <p>A paragraph for the main content.</p>
            <p>And another one.</p>
            </template>

            <template #footer>
            <p>Here's some contact info</p>
            </template>
        </Card>
        <div id="MiddleContainer">
            <Card id="MiddleLeft">
                <template #default>
                <p>A paragraph for the main content.</p>
                <p>And another one.</p>
                </template>
            </Card>
            <div class="card" id="MiddleRight"><AccountsView id="accountsView"/></div>
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
    #accountsView {
        flex-grow: 1;
        height: 100%;
    }
</style>