import type { AnalysisEntry, RowDef } from "@/types/CommonTypes";

export class Analysis {
    static createAnalysisEntry(scenario: string, aspect: string) : Record<string, string> {

        return {
            entryId: "",
            parentId: "",
            parentType: "Position",
            analysisScenario: scenario,
            analyzedAspect: aspect,
            volume: "0.00",
            executionPrice: "0.00",
            executionTime: new Date().toLocaleString(),
            profitLoss: "0.00",
            profitInPips: "0.00",
            profitInPercent: "0.00",
            usedIndicator: "Unknown",
            indicatorStatus: "Unknown",
            usedSystem: "Unknown",
            usedStrategy: "Unknown",
            executionAccuracy: "",
            isValid: 'Yes',
            invalidityReason: "",
        } as Record<string, string>;
    }

    static convertToAnalysisRecord(entry: AnalysisEntry) : Record<string, string> {
        return {
            entryId: entry.entryId.toString(),
            parentId: entry.parentId.toString(),
            parentType: entry.parentType,
            analysisScenario: entry.analysisScenario,
            analyzedAspect: entry.analyzedAspect,
            volume: (entry.volume / 10000000).toFixed(2),
            executionPrice: entry.executionPrice.toFixed(5),
            executionTime: new Date(entry.executionTime).toLocaleString(),
            profitLoss: entry.profitLoss.toFixed(2),
            profitInPips: entry.profitInPips.toFixed(1),
            profitInPercent: entry.profitInPercent.toFixed(2),
            usedIndicator: entry.usedIndicator,
            indicatorStatus: entry.indicatorStatus.toString(),
            usedSystem: entry.usedSystem,
            usedStrategy: entry.usedStrategy.toString(),
            executionAccuracy: entry.executionAccuracy.toString(),
            isValid: entry.isValid ? 'Yes' : 'No',
            invalidityReason: entry.invalidityReason.toString(),
        } as Record<string, string>;
    }

    static getAnalysisEntryRowDefs() {
        return [
            {property: "analyzedAspect", title: "Type", dataType: "text", editable: false},
            {property: "volume", title: "Traded Volume", dataType: "text", editable: false},
            {property: "executionPrice", title: "Execution Price", dataType: "text", editable: true},
            {property: "executionTime", title: "Execution Time", dataType: "text", editable: false},
            {property: "profitLoss", title: "Gross Profit", dataType: "text", editable: false},
            {property: "profitInPips", title: "PIPs Earned", dataType: "text", editable: false},
            {property: "profitInPercent", title: "Gain as %", dataType: "text", editable: false},
            {property: "usedIndicator", title: "Indicator", dataType: "text", editable: false},
            {property: "indicatorStatus", title: "Ind. Confirmation", dataType: "text", editable: false},
            {property: "usedSystem", title: "System Used", dataType: "text", editable: true},
            {property: "usedStrategy", title: "Entry/Exit Strategy", dataType: "text", editable: false},
            {property: "executionAccuracy", title: "Accuracy", dataType: "text", editable: false},
            {property: "isValid", title: "Valid Trade?", dataType: "text", editable: false},
            {property: "invalidityReason", title: "Why Valid?", dataType: "text", editable: false},
         ] as RowDef[];
    }
}