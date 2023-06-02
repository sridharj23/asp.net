
import { defineStore } from 'pinia'

export const useAccountStore = defineStore('selectedaccount', {

  state: () => ({ 
    selectedAccount: "0"
  })  
})
