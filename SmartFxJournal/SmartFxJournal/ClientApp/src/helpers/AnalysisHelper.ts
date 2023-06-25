import type { AnalysisEntry, RowDef, TableRow } from "@/types/CommonTypes";

export class Analysis {
    static createAnalysisEntry(scenario: string, aspect: string) : TableRow {

        return {
            isReadOnly: false,
            isInEdit: false,
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
            isValid: 'true',
            invalidityReason: "",
        } as TableRow;
    }

    static convertToAnalysisRecord(entry: AnalysisEntry, editable: boolean = false ) : TableRow {
        return {
            isReadOnly: editable,
            isInEdit: false,
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
            isValid: entry.isValid ? 'true' : 'false',
            invalidityReason: entry.invalidityReason.toString(),
        } as TableRow;
    }

    static convertToAnalysisObject(data: TableRow) : AnalysisEntry {
        return {
            entryId: +data['entryId'],
            parentId: +data['parentId'],
            parentType: data['parentType'].toString(),
            analysisScenario: data['analysisScenario'].toString(),
            analyzedAspect: data['analyzedAspect'].toString(),
            executionPrice: +data['executionPrice'],
            executionTime: new Date(data['executionTime'].toString()).toUTCString(),
            volume: +data['volume'] * 10000000,
            profitLoss: +data['profitLoss'],
            profitInPips: +data['profitInPips'],
            profitInPercent: +data['profitInPercent'],
            usedIndicator: data['usedIndicator'].toString(),
            indicatorStatus: data['indicatorStatus'].toString().split(','),
            usedSystem: data['usedSystem'].toString(),
            usedStrategy: data['usedStrategy'].toString().split(','),
            executionAccuracy: data['executionAccuracy'].toString().split(','),
            isValid: data['isValid'] == 'true' ? true : false,
            invalidityReason: data['invalidityReason'].toString().split(','),
        };
    }
    
    static getAnalysisEntryRowDefs() {
        return [
            {property: "analyzedAspect", title: "Type", dataType: "text", editable: false},
            {property: "volume", title: "Traded Volume", dataType: "text", editable: true},
            {property: "executionPrice", title: "Execution Price", dataType: "text", editable: true},
            {property: "executionTime", title: "Execution Time", dataType: "datetime", editable: true},
            {property: "profitLoss", title: "Gross Profit", dataType: "text", editable: false},
            {property: "profitInPips", title: "PIPs Earned", dataType: "text", editable: false},
            {property: "profitInPercent", title: "Gain as %", dataType: "text", editable: false},
            {property: "usedIndicator", title: "Indicator", dataType: "text", editable: true},
            {property: "indicatorStatus", title: "Ind. Confirmation []", dataType: "text", editable: true},
            {property: "usedSystem", title: "System Used", dataType: "text", editable: true},
            {property: "usedStrategy", title: "Entry/Exit Strategy []", dataType: "text", editable: true},
            {property: "executionAccuracy", title: "Accuracy []", dataType: "text", editable: true},
            {property: "isValid", title: "Valid Trade?", dataType: "checkbox", editable: true},
            {property: "invalidityReason", title: "Why Valid ? []", dataType: "text", editable: true},
         ] as RowDef[];
    }
}