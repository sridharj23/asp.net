import { defineStore } from 'pinia'

export const usePositionStore = defineStore('selectedposition', {
    state: () => ({ 
        lastSelectedPositionId: "0",
        dblClickedPositionId: "0",
        dblClickedPosition: {} as Record<string, string>,
        lastUpdated: Date.now
    })  
  })