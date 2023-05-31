
import { defineStore } from 'pinia'

export const useAccountStore = defineStore('selectedaccount', {

  state: () => ({ selectedAccount: "0"}),
  
  actions: {
    setSelectedAccount(accountNo : string) {
      this.selectedAccount = accountNo
    }
  },
  getters : {
    getSelectedAccount: (state) => state.selectedAccount,
  }
})
