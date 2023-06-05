<script setup>
    import PositionsView from '@/views/PositionsView.vue'
    import Card from '@/components/Card.vue';
</script>

<template>
    <div id="TradesPage">
        <Card id="positionsView">
            <PositionsView/>
        </Card>
        <div id="TradesPageRight">
            <Card id="tradeGraph"/>
            <Card id="tradesNotes"/>
        </div>
    </div>
</template>

<style scoped>
    #TradesPage {
        display: flex;
        flex-grow: 1;
        flex-direction: row;
        flex-grow: 1;
        height: 99%;
    }
    #positionsView {
        width: 50%;
        height: 100%;
        margin-right: 5px;
    }
    #TradesPageRight {
        width: 50%;
        margin-left: 5px;
        display: flex;
        flex-direction: column;
        height: 90%;
        flex-grow: 1;
        border: 2px solid blue;
    }
    #tradeGraph {
        height: 50%;
        flex-grow: 1;
    }
    #tradesNotes {
        height: 50%;
        margin-top: 5px;
        flex-grow: 1;
    }

</style>
