<script setup>
    import PositionsView from '@/views/PositionsView.vue'
    import Card from '@/components/Card.vue';
</script>

<template>
    <div id="TradesOverview">
        <div id="mainContainer">
            <Card id="tradesTable">
                <PositionsView/>
            </Card>
            <Card id="tradesNotes"/>
        </div>
    </div>
</template>

<style scoped>
    #TradesOverview {
        display: flex;
        flex-direction: row;
        background-color: azure;
        flex-grow: 1;
    }
    #mainContainer {
        flex-grow: 1;
        height: 100%;
        display: flex;
        flex-direction: row;
        margin-top: 10px;
    }
    #tradesTable {
        width: 65%;
        height: 100%;
        margin-right: 5px;
    }
    #tradesNotes {
        width: 35%;
        height: 100%;
        margin-left: 5px;
    }

</style>
