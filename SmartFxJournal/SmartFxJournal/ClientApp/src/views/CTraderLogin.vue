<script lang="ts">
    import { CTraderAPI } from '@/api/CTraderApi';

    const api = new CTraderAPI();

    export default {
        data() {
            return {
                ctClientId : "",
                ctTraderId : "",
                ctSecret: ""
            }
        },
        computed :{
            validInputs() : boolean {
                return (this.ctClientId != "" && this.ctTraderId != "" && this.ctSecret != "");
            }
        },
        methods: {
            isTraderOnboarded() : boolean {
                return api.hasLogin(this.ctTraderId);
            },

            fetchNewAccounts() {
                if (this.isTraderOnboarded()) {
                    //directly request backend for any new accounts
                } else {
                    //initiate first time login
                    this.initiateLogin();
                }
            },

            initiateLogin() {
                if (this.isTraderOnboarded()) {
                    //directly retrieve any new accounts
                } else {
                    // initiate first time login
                    // loop on 
                    api.getLoginTarget(this.ctTraderId, this.ctClientId, this.ctSecret).then(url => {
                        console.log("Login target : " + url);
                        var win = window.open(url, '_blank');
                        win?.focus;
                    });
                }
            }
        }
    }
</script>

<template>
    <section class="container" id="cTraderContainer">
        <div style="margin: 10px;">
            <div class="inputRow">
                <label class="labels" for="ctrader_id">CTrader ID</label>
                <input id="ctrader_id" class="inputControls" type="text" v-model="ctTraderId" required autocomplete="off">
            </div>
            <div class="inputRow">
                <label class="labels" for="client_id">Client ID</label>
                <input id="client_id" class="inputControls" type="text" v-model="ctClientId" required autocomplete="off">
            </div>
            <div class="inputRow">
                <label class="labels" for="client_sec">Client Secret</label>
                <input id="client_sec" class="inputControls" type="password" v-model="ctSecret" required autocomplete="off">
            </div>
            <div class="flow-row">
                <button type="button" @click="initiateLogin" :disabled="!validInputs">Import Accounts</button>
            </div>
        </div>
    </section>
</template>

<style scoped>
    #cTraderContainer {
        flex-grow: 1;
    }
</style>
