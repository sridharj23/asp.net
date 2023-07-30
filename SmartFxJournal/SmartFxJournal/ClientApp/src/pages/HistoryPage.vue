<script lang="ts">
    import HistoryView from '@/views/HistoryView.vue';
    import Card from '@/components/Card.vue';
    import JournalView from '@/views/JournalView.vue';

    export default {
        components: {
            HistoryView,
            Card,
            JournalView
        },
        data() {
            return {
                selectedPosition: ""
            }
        },
        methods: {
            handlePositionSelection(position : Record<string, string>) {
                this.selectedPosition = position['positionId'];
            }
        }
    }
</script>

<template>
    <div id="TradesPage">
        <Card id="positionsView">
            <HistoryView @positionSelected="handlePositionSelection"/>
        </Card>
        <div id="tradesNotes">
            <JournalView id="jView"/>
        </div>
    </div>
</template>

<style scoped>
    #TradesPage {
        display: flex;
        flex-direction: row;
        flex-grow: 1;
        height: 90vh;
    }
    #positionsView {
        width: 65%;
        height: 100%;
        margin-right: 5px;
    }
    #tradesNotes {
        width: 35%;
        height: 100%;
        margin-left: 5px;
        flex-grow: 1;
    }
    #jView {
        width: 100%;
        height: 100%;
    }
</style>
