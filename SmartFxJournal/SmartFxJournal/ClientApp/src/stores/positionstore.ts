import { defineStore } from 'pinia'

export const usePositionStore = defineStore('selectedposition', {
    state: () => ({ 
        selectedPositionId: "0",
        selectedPosition: {} as Record<string, string>
    })  
  })