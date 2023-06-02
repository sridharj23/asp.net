<script lang="ts">
    import { CTraderAPI, type AccountEntry } from '@/api/CTraderApi';

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
                return (this.ctTraderId != "");
            }
        },
        methods: {
            fetchNewAccounts() {
                api.hasLogin(this.ctTraderId).then(has => {
                    if (has) {
                        //directly request backend for any new accounts
                        api.importAccounts(this.ctTraderId).then(a => {
                            api.displayInfo(a + " Please refresh your accounts page to see the changes.");
                        })
                    } else {
                        //initiate first time login
                        this.initiateLogin();
                    }
                })
            },

            initiateLogin() {
                if (this.ctClientId == "" || this.ctSecret == "") {
                    alert("Ctrader ID seems to be new. Client ID and Client Secret are mandatory to onboard new CTrader IDs.")
                    return;
                }
                api.getLoginTarget(this.ctTraderId, this.ctClientId, this.ctSecret).then(url => {
                    console.log("Login target : " + url);
                    var win = window.open(url, '_blank');
                    win?.focus;
                });
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
                <button type="button" @click="fetchNewAccounts" :disabled="!validInputs">Import Accounts</button>
            </div>
        </div>
    </section>
</template>

<style scoped>
    #cTraderContainer {
        flex-grow: 1;
    }
</style>
