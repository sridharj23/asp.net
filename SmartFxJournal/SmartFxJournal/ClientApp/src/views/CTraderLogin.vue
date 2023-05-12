<script lang="ts">
    import { CTraderAPI } from '@/api/CTraderApi';

    const api = new CTraderAPI();
    const frame : HTMLIFrameElement = document.getElementById("loginForm") as HTMLIFrameElement;

    export default {
        methods: {
            queryOnboardingStatus(cTraderId : string) : boolean {
                return api.hasLogin(cTraderId);
            },

            initiateLogin(cTraderId : string, clientId : string) {
                if (frame != null) {
                    frame.src = api.getLoginTarget(cTraderId, clientId);
                }
            }
        }
    }
</script>

<template>
    <section class="container" id="cTraderContainer">
        <div class="inputRow">
            <label class="labels" for="ctrader_id">CTrader ID</label>
            <input id="ctrader_id" class="inputControls" type="text" required autocomplete="off">
            <input type="button" value="Import Accounts" style="width: auto;">
        </div>
        <iframe id="loginForm" title="Login into CTrader"/>
    </section>
</template>

<style scoped>
    #cTraderContainer {
        flex-grow: 1;
    }
    #loginForm {
        flex-grow: 1;
        width: 100%;
        height: 95%;
        border: 1px solid silver;
    }
</style>
