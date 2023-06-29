import type { AnalysisEntry, RowDef, TableRow } from "@/types/CommonTypes";
import { usePositionStore } from '@/stores/positionstore';
import { useStatusStore } from '@/stores/statusstore';

export class Analysis {

    static readonly store = usePositionStore();
    static readonly status = useStatusStore();

    static readonly ASPECT = "analyzedAspect";
    static readonly BUY = "BUY";
    static readonly SELL = "SELL"; 
    static readonly ENTRY = "Entry";
    static readonly EXIT = "Exit";
    static readonly STOP = "StopLoss";
    static readonly TPROFIT = "TakeProfit";
    static readonly PIP_F4 = 0.0001;
    static readonly LOT_SIZE = 100000;

    static GetPips = (from: number, to: number) => (from - to) / this.PIP_F4;
    static PipValue = (price: number, lot: number) => (this.PIP_F4 / price) * lot;
    static Percent = (value: number, base: number) => (100 * value) / base;

    static TagList = new Map<string, Array<string>>();

    static CalculateRiskReward(data : TableRow, context : TableRow[]): TableRow {
        let aspect = data[this.ASPECT];
        let entry : TableRow | undefined = undefined;
        let position = this.store.dblClickedPosition;

        context.forEach(d => {
            if (d[this.ASPECT] == this.ENTRY) {
                entry = d;
            } 
        });

        if (entry == undefined) {
            this.status.setInfo(data['analysisScenario'] +  " Entry details are missing. Using actual values !");
            entry = this.CreateAnalysisEntry('Actual', 'Entry');
            entry['volume'] = position['volume'];
            entry['executionPrice'] = position['entryPrice'];
        }
        let dataPrice = +data['executionPrice'];
        let lots = +entry['volume'] * this.LOT_SIZE;

        if (dataPrice == 0 || lots == 0) {
            this.status.setError(" Execution Price & Lot Size are mandatory but missing !");
            return data;
        }

        let isSell: boolean = this.store.dblClickedPosition['direction'] == this.SELL;
        let balance : number = +this.store.dblClickedPosition['balanceAfter'] - (+this.store.dblClickedPosition['grossProfit']);
        let entryPrice = +entry['executionPrice'];
        let pipVal : number = this.PipValue(dataPrice, lots);

        let pips : number = isSell ? this.GetPips(entryPrice, dataPrice) : this.GetPips(dataPrice, entryPrice);;
        let profitLoss : number = pipVal * pips;
        let profitPercent : number = this.Percent(profitLoss, balance);

        data['profitInPips'] = pips.toFixed(1);
        data['profitLoss'] = profitLoss.toFixed(2);
        data['profitInPercent'] = profitPercent.toFixed(2);

        return data;
    }

    static CreateAnalysisEntry(scenario: string, aspect: string) : TableRow {

        return {
            isInEdit: false,
            isNew: true,
            entryId: "",
            positionId: "",
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
            validTrade: true,
            reasonToTrade: "",
            betterAvoided: false,
            reasonToAvoid: ""
        } as TableRow;
    }

    static ConvertToAnalysisRecord(entry: AnalysisEntry) : TableRow {
        return {
            isInEdit: false,
            isNew: false,
            entryId: entry.entryId.toString(),
            positionId: entry.positionId.toString(),
            analysisScenario: entry.analysisScenario,
            analyzedAspect: entry.analyzedAspect,
            volume: (entry.volume / this.LOT_SIZE).toFixed(2),
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
            validTrade: entry.validTrade,
            reasonToTrade: entry.reasonToTrade.toString(),
            betterAvoided: entry.betterAvoided,
            reasonToAvoid: entry.reasonToAvoid.toString(),
        } as TableRow;
    }

    static ConvertToAnalysisObject(data: TableRow) : AnalysisEntry {
        return {
            entryId: +data['entryId'],
            positionId: +data['positionId'],
            analysisScenario: data['analysisScenario'].toString(),
            analyzedAspect: data['analyzedAspect'].toString(),
            executionPrice: +data['executionPrice'],
            executionTime: new Date(data['executionTime'].toString()).toISOString(),
            volume: +data['volume'] * this.LOT_SIZE,
            profitLoss: +data['profitLoss'],
            profitInPips: +data['profitInPips'],
            profitInPercent: +data['profitInPercent'],
            usedIndicator: data['usedIndicator'].toString(),
            indicatorStatus: data['indicatorStatus'].toString().split(','),
            usedSystem: data['usedSystem'].toString(),
            usedStrategy: data['usedStrategy'].toString().split(','),
            executionAccuracy: data['executionAccuracy'].toString().split(','),
            validTrade: Boolean(data['validTrade']),
            reasonToTrade: data['reasonToTrade'].toString().split(','),
            betterAvoided: Boolean(data['betterAvoided']),
            reasonToAvoid: data['reasonToAvoid'].toString().split(',')
        };
    }
    
    static GetAnalysisEntryRowDefs() {
        this.InitializeTags();
        return [
            {property: "analyzedAspect", title: "Type", dataType: "text", editable: false},
            {property: "volume", title: "Traded Volume", dataType: "text", editable: true},
            {property: "executionPrice", title: "Execution Price", dataType: "text", editable: true},
            {property: "executionTime", title: "Execution Time", dataType: "datetime", editable: true},
            {property: "profitLoss", title: "Gross Profit", dataType: "text", editable: false},
            {property: "profitInPips", title: "PIPs Earned", dataType: "text", editable: false},
            {property: "profitInPercent", title: "Gain as %", dataType: "text", editable: false},
            {property: "usedIndicator", title: "Indicator", dataType: "select", editable: true, inputHelp: this.TagList.get('usedIndicator')},
            {property: "indicatorStatus", title: "Ind. Confirmation []", dataType: "select", editable: true, inputHelp: this.TagList.get('indicatorStatus')},
            {property: "usedSystem", title: "System Used", dataType: "select", editable: true, inputHelp: this.TagList.get('usedSystem')},
            {property: "usedStrategy", title: "Entry/Exit Strategy []", dataType: "select", editable: true, inputHelp: this.TagList.get('entryExitStrategy')},
            {property: "executionAccuracy", title: "Accuracy []", dataType: "select", editable: true, inputHelp: this.TagList.get('accuracy')},
            {property: "validTrade", title: "Valid Trade?", dataType: "checkbox", editable: true},
            {property: "reasonToTrade", title: "Reasons to Trade []", dataType: "multiselect", editable: true, inputHelp: this.TagList.get('reasons')},
            {property: "betterAvoided", title: "Better Avoided?", dataType: "checkbox", editable: true},
            {property: "reasonToAvoid", title: "Reasons to Avoid []", dataType: "multiselect", editable: true, inputHelp: this.TagList.get('reasons')},
         ] as RowDef[];
    }

    static InitializeTags() {
        if (this.TagList.size > 0) return;

        this.TagList.set('usedIndicator', new Array("MFI_MA", "PriceAction"));
        this.TagList.set('indicatorStatus', new Array("FullConfirmation", "SoftConfirmation", "ContraIndicating", "DivergenceFor", "DivergenceAgainst"));
        this.TagList.set('usedSystem', new Array("TrendRSIMomentum", "V-Strategy", "CounterPullBack", "None"));
        this.TagList.set('entryExitStrategy', new Array("PreviousBarHighLow", "PreviousSwingHighLow", "Impulsive", "DontRemember"));
        this.TagList.set('accuracy', new Array("Impulsive", "Wrong", "TryingOut", "TooEarly", "TooLate", "BeforeBarClose", "OnBarClose"));
        this.TagList.set('reasons', new Array("AgainstIndicator", "DontRemember", "NotOnBarClose"));
    }
}