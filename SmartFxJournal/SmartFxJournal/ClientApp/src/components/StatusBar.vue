<script lang="ts">
    import { useStatusStore } from '@/stores/statusstore';

    export default {
        setup() {
            const store = useStatusStore();
            return {store};
        },
        mounted() {
            this.store.$subscribe(this.setStatusMessage);
        },
        methods: {
            setStatusMessage() {
                this.messageText = this.store.statusMessage;
                this.messageCategory = this.store.messageCategory;
                let ts = setTimeout(() => {this.clearStatus()}, 10000);
            },
            clearStatus() {
                this.messageCategory = "";
                this.messageText = "";
            }
        },
        data() {
            return {
                messageCategory: "",
                messageText: ""

            }
        }
    }
</script>

<template>
    <div :class="messageCategory">{{ messageCategory }} : {{ messageText }}</div>
</template>

<style scoped>
    #statusContainer {
        display: block;
        height: 100%;
    }
    .Error {
        color: red;
        font-weight: bold;
    }
    .Info {
        color: blue;
        font-weight: bold;
    }
</style>