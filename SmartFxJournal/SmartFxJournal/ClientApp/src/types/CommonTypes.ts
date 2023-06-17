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
    executedOrders: ExecutedOrder[]
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

export interface TrendBars {
    timePeriod: string;
    positionOpenedAt: number;
    positionOpenPrice: number;
    positionClosedAt: number;
    positionClosePrice: number;
    symbol: string;
    trendBars: number[][];
}

export interface DataColumn {
    property : string;
    title : string;
    propType: string;
}

export class Common {
    static getPositionColumns(prepend? : DataColumn[]) : DataColumn[] {
        let cols : DataColumn[] = [];
        if (prepend != undefined) {
            prepend.forEach(col => cols.push(col));
        }

        cols.push({ property: "positionId", title: "Position", propType: "default" });
        cols.push({ property: "symbol", title: "Symbol", propType: "default" });
        cols.push({ property: "direction", title: "Direction", propType: "default" });
        cols.push({ property: "volume", title: "Volume", propType: "default" });
        cols.push({ property: "entryPrice", title: "Entry Price", propType: "default" });
        cols.push({ property: "exitPrice", title: "Exit Price", propType: "default" });
        cols.push({ property: "commission", title: "Comm.", propType: "Currency" });
        cols.push({ property: "swap", title: "Swap", propType: "Currency" });
        cols.push({ property: "grossProfit", title: "Gross P/L", propType: "Currency" });
        cols.push({ property: "netProfit", title: "Net P/L", propType: "Currency" });
        cols.push({ property: "balanceAfter", title: "Balance", propType: "Currency" });
        cols.push({ property: "orderOpenedAt", title: "Opened At", propType: "default" });
        cols.push({ property: "orderClosedAt", title: "Closed At", propType: "default" });

        return cols;
    }

    static getExcecutedOrderColumns(prepend? : DataColumn[]) : DataColumn[] {
        let cols : DataColumn[] = [];
        if (prepend != undefined) {
            prepend.forEach(col => cols.push(col));
        }
        
        cols.push({ property: "orderId", title: "Order ID", propType: "default" });
        cols.push({ property: "positionId", title: "Position ID", propType: "default" });
        cols.push({ property: "symbol", title: "Symbol", propType: "default" });
        cols.push({ property: "direction", title: "Direction", propType: "default" });
        cols.push({ property: "filledVolume", title: "Filled Volume", propType: "default" });
        cols.push({ property: "closedVolume", title: "Filled Volume", propType: "default" });
        cols.push({ property: "executionPrice", title: "Execution Price", propType: "default"});
        cols.push({ property: "commission", title: "Commission", propType: "Currency" });
        cols.push({ property: "swap", title: "Swap", propType: "Currency" });
        cols.push({ property: "grossProfit", title: "Gross Profit", propType: "Currency" });
        cols.push({ property: "balanceAfter", title: "Balance", propType: "Currency" });
        cols.push({ property: "orderExecutedAt", title: "Execution Time", propType: "default"});
        
        return cols;
    }
    
    static convertPositionToRecord(pos : Position, prepend? : Record<string, string>) : Record<string, string> {
        let rec : Record<string,string> = {
            positionId: pos.positionId,
            symbol: pos.symbol,
            direction: pos.direction,
            volume: pos.volumeInLots.toFixed(2),
            entryPrice: pos.entryPrice.toFixed(5),
            exitPrice: pos.exitPrice.toFixed(5),
            commission: pos.commission.toFixed(2),
            swap: pos.swap.toFixed(2),
            grossProfit: pos.grossProfit.toFixed(2),
            netProfit: pos.netProfit.toFixed(2),
            balanceAfter: pos.balanceAfter.toFixed(2),
            orderOpenedAt: new Date(pos.orderOpenedAt).toLocaleString(),
            orderClosedAt: new Date(pos.orderClosedAt).toLocaleString(),
        };
        if (prepend != undefined) {
            let keys = Object.keys(prepend);
            keys.forEach(key => rec[key] = prepend[key]);
        }

        return rec;
    }

    static convertOrderToRecord(order: ExecutedOrder, prepend? : Record<string, string>) : Record<string, string> {
        let rec : Record<string, string> = {
            dealId: order.dealId.toString(),
            positionId: order.positionId.toString(),
            orderId: order.orderId.toString(),
            symbol: order.symbol,
            direction: order.direction,
            filledVolume: order.filledVolume.toString(),
            closedVolume: order.closedVolume.toString(),
            executionPrice: order.executionPrice.toFixed(5),
            commission: order.commission.toFixed(2),
            swap: order.swap.toFixed(2),
            grossProfit: order.grossProfit.toFixed(2),
            balanceAfter: order.balanceAfter.toFixed(2),
            orderExecutedAt: new Date(order.orderExecutedAt).toLocaleString(),
        };
        if (prepend != undefined) {
            let keys = Object.keys(prepend);
            keys.forEach(key => rec[key] = prepend[key]);
        }

        return rec;
    }
}