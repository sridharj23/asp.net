
export interface Account {
    accountNo : string,
    isLive: boolean,
    isDefault : boolean,
    nickName : string,
    broker : string,
    accountCurrency: string,
    startBalance : number,
    currentBalance : number,
    openedOn : string,
    importMode: string,
    cTraderId: string,
    lastImportedOn?: string,
    createdOn?: string,
    modifiedOn?: string
}

export interface AccountEntry {
    cTraderId : string,
    accountId : string,
    accountCurrency : string,
    brokerName: string,
    isLive : boolean,
    isImported : boolean,
    balance : number,
    opendedOn: string
}

export interface Position {
    accountNo: string,
    positionId : string,
    symbol : string,
    direction: string,
    volume: number,
    volumeInLots: number,
    entryPrice: number,
    exitPrice: number,
    fees : number,
    swap : number,
    commission: number,
    grossProfit : number,
    netProfit : number,
    balanceAfter: number,
    orderOpenedAt: string,
    orderClosedAt: string,
    executedOrders: ExecutedOrder[],
    analysisStatus: string
}

export interface ExecutedOrder {
    dealId: number,
    orderId: number,
    positionId: number,
    symbol: string,
    direction: string,
    filledVolume: number,
    closedVolume: number,
    isClosing: boolean,
    executionPrice: number,
    commission: number,
    swap: number,
    grossProfit: number,
    balanceAfter: number,
    orderExecutedAt: string
}

export interface AnalysisEntry {
    entryId: number,
    positionId: number,
    analysisScenario: string,
    analyzedAspect: string,
    executionPrice: number,
    executionTime: string,
    volume: number,
    profitLoss: number,
    profitInPips: number,
    profitInPercent: number,
    usedIndicator: string,
    indicatorStatus: string[],
    usedSystem: string,
    usedStrategy: string[],
    executionAccuracy: string[],
    validTrade: boolean,
    reasonToTrade: string[],
    betterAvoided: boolean,
    reasonToAvoid: string[]
}

export interface ReconcileEntry {
    positionId: number,
    orders: number[]
}

export interface EquityCurve {
    accountNumber : number,
    dataPoints : EquityDataPoint[]
}

export interface EquityDataPoint {
    equity: number,
    timeStamp : number
}

export interface SummaryAggregate {
    aggregateKey: string,
    totalPL : number,
    plFromShorts : number,
    plFromLongs : number
}

// UI specific types

export type TableRow = Record<string, string | boolean>;

export interface TrendBars {
    timePeriod: string;
    positionOpenedAt: number;
    positionOpenPrice: number;
    positionClosedAt: number;
    positionClosePrice: number;
    symbol: string;
    trendBars: number[][];
}

export interface ColumDef {
    property : string;
    title : string;
    propType: string;
}

export interface RowDef {
    property : string,
    title : string,
    dataType : string,
    editable: boolean,
    inputHelp: Array<string> | undefined
}
