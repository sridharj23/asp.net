
import { defineStore } from 'pinia'
import type { Account } from '@/types/CommonTypes';

export const useAccountStore = defineStore('selectedaccount', {

  state: () => ({ 
    selectedAccount: "0",
    theSelectedAccount: {} as Account
  })  
})
