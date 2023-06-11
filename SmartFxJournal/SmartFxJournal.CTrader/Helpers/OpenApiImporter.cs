using OpenAPI.Net.Helpers;
using SmartFxJournal.CTrader.helpers;
using SmartFxJournal.CTrader.Models;
using SmartFxJournal.CTrader.Services;
using SmartFxJournal.JournalDB.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using static SmartFxJournal.JournalDB.model.GlobalEnums;

namespace SmartFxJournal.CTrader.Helpers
{
    internal class OpenApiImporter
    {
        private static readonly long RECONCILE_POSITION_ID = -1000000;

        internal static async Task<TradingAccount> ImportAccountAsync(ProtoOACtidTraderAccount act, OpenApiService openApiService, CTraderAccount parent)
        {

            long accountId = (long)act.CtidTraderAccountId;
            var trader = await openApiService.GetTrader(accountId, act.IsLive);
            var assets = await openApiService.GetAssets(accountId, act.IsLive);

            TradingAccount? entry = parent.TradingAccounts.Find(a => a.AccountNo.Equals(act.TraderLogin));

            if (entry == null)
            {
                entry = new()
                {
                    CTraderAccountId = accountId,
                    AccountNo = trader.TraderLogin,
                    ImportMode = ImportMode.cTrader,
                    IsDefault = false,
                    IsLive = act.IsLive,
                    AccountCurrency = Enum.Parse<CurrencyType>(assets.First(iAsset => iAsset.AssetId == trader.DepositAssetId).Name),
                    Broker = trader.BrokerName,
                    OpenedOn = DateOnly.FromDateTime(DateTimeOffset.FromUnixTimeMilliseconds(trader.RegistrationTimestamp).DateTime),
                    LastImportedOn = DateTimeOffset.FromUnixTimeMilliseconds(trader.RegistrationTimestamp)
                };

                OffsetIterator iterator = new(entry.LastImportedOn.ToUnixTimeMilliseconds());
                var span = iterator.TimeStampRanges.First();

                var transactions = await openApiService.GetTransactions(accountId, act.IsLive, span.From, span.To);
                entry.StartBalance = (decimal)(transactions[0].Balance / 100);
                entry.CreatedOn = DateTime.Now;
                
                parent.TradingAccounts.Add(entry);
            }
            entry.CurrentBalance = (decimal)MonetaryConverter.FromMonetary(trader.Balance);
            entry.LastModifiedOn = DateTime.Now;

            return entry;
        }

        internal static ClosedPosition CreateReconcilePosition(TradingAccount parent)
        {
            ClosedPosition toReconcile = new()
            {
                PositionId = -1 * parent.AccountNo,
                AccountNo = parent.AccountNo,
                BalanceAfter = Decimal.Zero,
                Direction = TradeDirection.BUY,
                EntryPrice = Decimal.Zero,
                ExitPrice = Decimal.Zero,
                GrossProfit = Decimal.Zero,
                NetProfit = Decimal.Zero,
                PositionStatus = PositionStatus.Open,
                StopLoss = Decimal.Zero,
                Symbol = Decimal.Zero,
                Commission = Decimal.Zero,
                Swap = Decimal.Zero,
                TakeProfit = Decimal.Zero,
                Volume = 0,
                LastUpdatedAt = DateTime.Now.ToUniversalTime(),
                OrderClosedAt = DateTime.Now.ToUniversalTime(),
                OrderOpenedAt = DateTime.Now.ToUniversalTime()
            };

            return toReconcile;
        }

        internal static async Task<TradingAccount> ImportHistoryAsync(HistoricalTrade[] history, OpenApiService openApiService, TradingAccount parent)
        {
            var symbols = await openApiService.GetLightSymbols((long)parent.CTraderAccountId, parent.IsLive);

            foreach (HistoricalTrade tr in history)
            {
                ExecutedOrder? toImport = parent.ExecutedOrders.Find(a => a.OrderId == tr.OrderId);
                if (toImport == null)
                {
                    toImport = new()
                    {
                        DealId = tr.Id,
                        OrderId = tr.OrderId,
                        PositionId = -1 * parent.AccountNo, // orders are added to reconcile position first
                        AccountNo = parent.AccountNo,
                        Symbol = (Symbol) tr.SymbolId,
                        ExecutionPrice = (decimal)tr.ExecutionPrice,
                        Direction = EnumUtil.ToEnum<TradeDirection>(tr.Direction),
                        OrderExecutedAt = tr.ExecutionTime,
                    };
                    parent.ExecutedOrders.Add(toImport);
                }
                toImport.Commission = (decimal)(tr.Commission / 100);
                toImport.DealStatus = EnumUtil.ToEnum<DealStatus>(tr.Status);
                toImport.Swap = (decimal)(tr.Swap / 100);
                toImport.LastUpdatedAt = tr.LastUpdateTime;
                if (tr.IsClosing)
                {
                    toImport.ClosedVolume = (long)tr.ClosedVolume;
                } else
                {
                    toImport.FilledVolume = (long)tr.FilledVolume;
                }

                toImport.BalanceAfter = (decimal) (tr.ClosedBalance / 100);
                toImport.GrossProfit = (decimal)(tr.GrossProfit / 100);
                toImport.IsClosing = tr.IsClosing;
            }

            return parent;
        }
    }
}
