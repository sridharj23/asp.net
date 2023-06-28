<script lang="ts">
    import Card from './Card.vue';

    export default {
        props: {
            showDialog: {type: Boolean, required: true, default: false},
            title: {type: String, required: true},
            showButtons: {type: Array<String>, default: ['Ok', 'Cancel']}
        },
        emits: ['buttonClicked'],
        components: {
            Card
        },
        methods: {
            onButtonClick(button: String) {
                this.$emit('buttonClicked', button);
            }
        }
    }
</script>

<template>
    <div class="modal-dialog" v-show="showDialog">
        <Card class="modal">
            <template #headerSlot>
                <p class="titleBar">{{ title }}</p>
            </template>
            <template #default>
                <slot name="dialogContent"/>
            </template>
            <template #footerSlot>
                <div class="flow-row">
                    <button type="button" v-for="btnTxt in showButtons" @click="onButtonClick(btnTxt)">{{ btnTxt }}</button>
                </div>
            </template>
        </Card>
    </div>
</template>

<style scoped>
    .modal-dialog {
        color: #008cff;
        background-color: rgba(169, 169, 169, 0.427);
        width: 99.5vw;
        height: 99vh;
        left: 5px;
        top: 5px;
        position: absolute;
        align-content: center;
    }
    .modal {
        background-color: white;
        position: relative;
        left: 35%;
        top: 40%;
        width: 30%;
        height: 20%;
    }
</style>