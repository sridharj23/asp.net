<script setup lang="ts">
    import Card from '@/components/Card.vue'
    import ReconcileView from '@/views/ReconcileView.vue'
</script>

<template>
    <div id="reconcilePage">
        <Card id="ordersView">
            <ReconcileView id="reconcileView"/>
        </Card>
    </div>
</template>

<style scoped>
    #reconcilePage {
        display: flex;
        flex-direction: row;
        height: 90vh;
    }
    #ordersView {
        width: 100%;
        height: 100%;
    }
    #reconcileView {
        display: block;
        height: 100%;
    }

</style>