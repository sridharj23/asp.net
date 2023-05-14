<script lang="ts">
import {AccountsAPI} from '@/api/AccountsApi';
import type { Account } from '@/api/AccountsApi';

const api = new AccountsAPI();

export default {
    name: "AccountsView",
    data() {
        return {
            theAccounts: [] as Account[],
            activeAccount: {} as Account,
            selectedAccount: {} as Account,
            selectedAcNo: "",
            lastSelect: "",
            editMode: false,
            createMode: false
        }
    },

    computed : {
        isReadOnly() {
            return !(this.editMode || this.createMode);
        },
        isEditable() {
            return this.isReadOnly && (this.theAccounts.length > 0);
        }
    },

    methods: {
        addAccount(ac : Account) {
            this.theAccounts.push(ac);
            if (ac.isDefault) {
                this.activeAccount = ac;
                this.selectedAccount = ac;
                this.selectedAcNo = ac.accountNo;
            }
        },
        loadAccounts() {
            if (this.theAccounts.length > 0) {
                this.theAccounts.length = 0;
                this.selectedAccount = {} as Account;
            }
            let result = api.getAll();
            result.then((resp) => {
                resp?.forEach(entry => {
                  this.addAccount(entry);
                })
                if (this.selectedAcNo == "" && resp.length > 0) {
                    this.selectedAcNo = resp[0].accountNo;
                    this.setSelected();
                }

            });
        },
        setSelected: function() {
            console.log("Selected account : " + this.selectedAcNo)
            this.selectedAccount = {} as Account;
            for(var ac of this.theAccounts) {
                if (ac.accountNo == this.selectedAcNo) {
                    this.selectedAccount = ac;
                }
            }
        },
        enableEditing() {
            this.editMode = true;
            this.createMode = false;
        },
        enableCreate() {
            this.editMode = false;
            this.createMode = true;
            this.selectedAccount = {} as Account;
            let dt = new Date().toISOString();
            this.selectedAccount.openedOn = dt.substring(0, dt.indexOf('T'));
        },
        saveAccount(e : Event) {
            e.preventDefault();
            if (this.createMode) {
                this.addAccount(this.selectedAccount);
                this.selectedAcNo = this.selectedAccount.accountNo;
                api.createNew(this.selectedAccount);
            } else if(this.editMode) {
                api.update(this.selectedAccount);
            }
            this.createMode = false;
            this.editMode = false;
        },
        cancelEditing() {
            this.createMode=false;
            this.editMode=false;
            this.setSelected();
        },
        deleteAccount() {
            if (confirm("The account and all the associated trades and stored data will be deleted ! Are you sure you want to delte?")) {
                api.delete(this.selectedAccount).then(() => this.loadAccounts());
            }
        }
    },
    mounted() {
        this.loadAccounts();
    },

}

</script>

<template>
    <section id="avContainer">
        <form id="accountForm" class="container" v-on:submit="saveAccount">
            <div class="inputRow">
                <label class="labels" for="account_no">Account Number</label>
                <select :class="createMode ? 'hidden' : 'inputControls visible' " id="account_no" v-model="selectedAcNo" @change="setSelected()" :disabled="editMode">
                    <option v-for="account in theAccounts" :value="account.accountNo">
                    {{ account.accountNo }}
                    </option>
                </select>
                <input id="acc_no" :class="createMode ? 'inputControls visible' : 'hidden'" v-model="selectedAccount.accountNo" type="text" :required="createMode" autocomplete="off">
            </div>
            <div class="inputRow">
                <label class="labels">Account Type</label>
                <input class="inputControls" id="live" name="account_type" type="radio" :disabled="isReadOnly" v-model="selectedAccount.accountType" value="Live"/>
                <label class="labels" for="live" style="width: auto;">Live</label>
                <input class="inputControls" id="demo" name="account_type" type="radio" :disabled="isReadOnly" v-model="selectedAccount.accountType" value="Demo" style="margin-left: 20px;"/>
                <label class="labels" for="demo" style="width: auto;">Demo</label>
            </div>
            <div class="inputRow">
                <label class="labels" for="is_default">Is Default</label>
                <input id="is_default" class="inputControls" type="checkbox" v-model="selectedAccount.isDefault" :disabled="isReadOnly">
            </div>
            <div class="inputRow">
                <label class="labels" for="nick_name">Nick Name</label>
                <input id="nick_name" class="inputControls" type="text" v-model="selectedAccount.nickName"  :readonly="isReadOnly" required autocomplete="off">
            </div>
            <div class="inputRow">
                <label class="labels" for="broker_name">Broker Name</label>
                <input id="broker_name" class="inputControls" type="text" v-model="selectedAccount.brokerName"  :readonly="isReadOnly" required autocomplete="off">
            </div>
            <div class="inputRow">
                <label class="labels" for="currency_type">Account Currency</label>
                <select class="inputControls" id="currency_type" v-model="selectedAccount.currencyType" :disabled="editMode" required>
                    <option value="EUR">EUR</option>
                    <option value="USD">USD</option>
                </select>
            </div>
            <div class="inputRow">
                <label class="labels" for="start_bal">Starting Balance</label>
                <input id="start_bal" class="inputControls" type="text" v-model="selectedAccount.startBalance" :readonly="isReadOnly" required autocomplete="off">
            </div>
            <div class="inputRow">
                <label class="labels" for="cur_bal">Current Balance</label>
                <input id="cur_bal" class="inputControls" type="text" v-model="selectedAccount.currentBalance" :readonly="isReadOnly" required autocomplete="off">
            </div>
            <div class="inputRow">
                <label class="labels" for="opened_on">Opened On</label>
                <input id="opened_on" class="inputControls" type="date" v-model="selectedAccount.openedOn" :readonly="isReadOnly" required autocomplete="off">
            </div>
            <div class="inputRow">
                <label class="labels">Import Mode</label>
                <input class="inputControls" id="ctrader" name="import_mode" type="radio" :disabled="isReadOnly" v-model="selectedAccount.importMode" value="cTrader"/>
                <label class="labels" for="ctrader" style="width: auto;">cTrader</label>
                <input class="inputControls" id="csv" name="import_mode" type="radio" :disabled="isReadOnly" v-model="selectedAccount.importMode" value="CSV" style="margin-left: 20px;"/>
                <label class="labels" for="csv" style="width: auto;">CSV</label>
            </div>
            <div class="inputRow">
                <label class="labels" for="last_imported_on">Last Import on</label>
                <input id="last_imported_on" class="inputControls" type="text" v-model="selectedAccount.lastImportedOn" readonly autocomplete="off" style="border: 0px;">
            </div>
            <div id="spacer"/>
            <div id="footer" class="flow-row">
                <button type="button" @click="loadAccounts()">Refresh</button>
                <button type="button" @click="enableEditing()" :disabled="!isEditable">Edit</button>
                <button type="button" @click="enableCreate()" :disabled="!isReadOnly">Add</button>
                <button type="submit" :disabled="isReadOnly">Save</button>
                <button type="button" @click="cancelEditing()" :disabled="isReadOnly">Cancel</button>
                <button type="button" @click="deleteAccount()" :disabled="!isEditable">Delete</button>
            </div>
        </form>
    </section>
</template>

<style scoped>
#avContainer {
    display: flex;
    flex-direction: column;
}
#accountForm {
    flex-grow: 1;
    height: calc(100% - 25px);
    margin: 10px;
}
#spacer {
    flex-grow: 1;
}
#footer {
    bottom: 0%;
}
.visible {
    display: flex;
}
.hidden {
    display: none;
}
</style>

