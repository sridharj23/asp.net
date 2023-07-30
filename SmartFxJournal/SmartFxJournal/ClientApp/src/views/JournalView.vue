<script lang="ts">
    import CKEditor from '@ckeditor/ckeditor5-vue';
    import ClassicEditor from '@ckeditor/ckeditor5-build-classic';
    import Card from '@/components/Card.vue';
    import { usePositionStore } from '@/stores/positionstore';
    import { type JournalEntry } from "@/types/CommonTypes";
    import {JournalNotesApi} from "@/api/JournalNotesApi"

    export default {
        setup() {
            const api = new JournalNotesApi();
            const store = usePositionStore();
            const defText = "<p><strong>Entry:</strong></p><ul><li>&nbsp;</li></ul><p><strong>Exit:</strong></p><ul><li>&nbsp;</li></ul><p><strong>Stop:</strong></p><ul><li>&nbsp;</li></ul><p><strong>TakeProfit:</strong></p><ul><li>&nbsp;</li></ul>";
            return {api, store, defText};
        },
        components: {
            Card,
            ckeditor: CKEditor.component

        },
        data() {
            return {
                editor: ClassicEditor,
                selectedEntry : {} as JournalEntry,
                selectedPositionId: "",
                hasContent: false,
                isEditing: false,
                editorData: ""
            }
        },
        computed: {
            canEdit() {
                return this.hasContent == true && this.isEditing == false;
            },
            canCreate() {
                return this.hasSelectedEntry && this.hasContent == false && this.isEditing == false;
            },
            hasSelectedEntry() {
                return +this.selectedPositionId > 0;
            }
        },
        methods: {
            setSelectedData(entry : JournalEntry) {
                this.selectedEntry = entry;
                this.editorData = entry.journalText;
                this.hasContent = +entry.journalId > 0;
            },
            setEditing() {
                this.isEditing = true;
                if (this.hasContent == false) {
                    this.editorData = this.defText;
                }
            },
            cancelEditing() {
                this.editorData = this.selectedEntry.journalText;
                this.isEditing = false;
            },
            saveJournal() {
                this.selectedEntry.journalText = this.editorData;
                if (+this.selectedEntry.journalId > 0) {
                    this.api.update(this.selectedEntry)
                } else {
                    this.api.createNew(this.selectedEntry).then(resp => this.selectedEntry = resp);
                }
                this.isEditing = false;
            },
            loadContent() {
                if (this.selectedPositionId == this.store.lastSelectedPositionId || +this.store.lastSelectedPositionId == 0) return;

                this.selectedPositionId = this.store.lastSelectedPositionId;
                this.editorData = "";
                this.isEditing = false;
                this.hasContent = false;

                if (+this.selectedPositionId > 0) {
                    this.api.get(this.selectedPositionId)
                        .then(resp => this.setSelectedData(resp))
                        .catch(err => {
                            if(err.code == "ERR_BAD_REQUEST") {
                                let je = { parentId : this.selectedPositionId, journalText: "No Journal found"} as JournalEntry;
                                this.setSelectedData(je);
                            }
                    });
                }
            }
        },
        mounted() {
            this.store.$subscribe(this.loadContent);
            this.loadContent();
        }
    }
</script>

<template>
    <div>
        <Card id="notesCard">
            <template #default>
                <div class="redBorder">
                    <ckeditor id="notesEditor" :disabled="isEditing==false" v-model="editorData" :editor="editor"/>
                </div>
            </template>
            <template #footerSlot>
                <div class="flow-row">
                    <button :disabled="!canCreate" @click="setEditing">Create</button>
                    <button :disabled="!canEdit" @click="setEditing">Edit</button>
                    <button :disabled="!isEditing" @click="saveJournal">Save</button>
                    <button :disabled="!isEditing" @click="cancelEditing">Cancel</button>
                </div>
            </template>
        </Card>
    </div>
</template>

<style>
    #notesCard {
        width: 100%;
        height: 100%;
    }
    .redBorder {
        width: 100%;
        height: 100%;
        display: flex;
        flex-direction: column;
    }
    .ck-editor__editable {
        flex-grow: 1;
        width: 97.3%;
        height: 85%;
        max-width: 100%;
        min-height: 50vh;
        max-height: 85vh;
    }
</style>