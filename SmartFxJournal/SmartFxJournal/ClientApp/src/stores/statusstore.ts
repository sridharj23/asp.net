
import { defineStore } from 'pinia'

export const useStatusStore = defineStore('statusmessage', {

  state: () => ({ 
    statusMessage: "",
    messageCategory:""
  }), 
  actions: {
    setError(msg: string) {
        this.statusMessage = msg;
        this.messageCategory = "Error";
    },
    setInfo(msg: string) {
        this.statusMessage = msg;
        this.messageCategory = "Info";
    }
  }
})
