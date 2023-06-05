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
        internal static async Task<FxAccount> ImportAccountAsync(ProtoOACtidTraderAccount act, OpenApiService openApiService, CTraderAccount parent)
        {
            long accountId = (long)act.CtidTraderAccountId;
            var trader = await openApiService.GetTrader(accountId, act.IsLive);
            var assets = await openApiService.GetAssets(accountId, act.IsLive);

            FxAccount? entry = parent.FxAccounts.Find(a => a.AccountNo.Equals(act.TraderLogin));

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
                
                parent.FxAccounts.Add(entry);
            }
            entry.CurrentBalance = (decimal)MonetaryConverter.FromMonetary(trader.Balance);
            entry.LastModifiedOn = DateTime.Now;

            return entry;
        }

        internal static async Task<FxAccount> ImportHistoryAsync(HistoricalTrade[] history, OpenApiService openApiService, FxAccount parent)
        {
            var symbols = await openApiService.GetLightSymbols((long)parent.CTraderAccountId, parent.IsLive);

            foreach (HistoricalTrade tr in history)
            {
                FxHistoricalTrade? toImport = parent.OrderHistory.Find(a => a.OrderId == tr.OrderId);
                if (toImport == null)
                {
                    toImport = new()
                    {
                        DealId = tr.Id,
                        OrderId = tr.OrderId,
                        AccountNo = parent.AccountNo,
                        Symbol = (Symbol) tr.SymbolId,
                        ExecutionPrice = (decimal)tr.ExecutionPrice,
                        Direction = EnumUtil.ToEnum<TradeDirection>(tr.Direction),
                        Volume = (long)tr.Volume,
                        OrderOpenedAt = tr.ExecutionTime,
                    };
                    parent.OrderHistory.Add(toImport);
                }
                toImport.Commission = (decimal)(tr.Commission / 100);
                toImport.DealStatus = EnumUtil.ToEnum<DealStatus>(tr.Status);
                toImport.Swap = (decimal)(tr.Swap / 100);
                toImport.LastUpdatedAt = tr.LastUpdateTime;
                toImport.FilledVolume = (long)tr.FilledVolume;
                toImport.ClosedVolume = (long)tr.ClosedVolume;
                toImport.BalanceAfterClose = (decimal) (tr.ClosedBalance / 100);
                toImport.GrossProfit = (decimal)(tr.GrossProfit / 100);
                toImport.IsClosing = tr.IsClosing;
            }

            return parent;
        }

        internal static async Task<FxPosition> ImportPositionAsync(ProtoOAPosition position, OpenApiService openApiService, FxAccount parent)
        {
            long pId = position.PositionId;
            long symId = position.TradeData.SymbolId;
            var symbols = await openApiService.GetLightSymbols((long)parent.CTraderAccountId, parent.IsLive);
            string name = symbols.First(sym => sym.SymbolId == symId).SymbolName;
            
            FxPosition? fxPosition = parent.Positions.Find(p => p.PositionId == pId);

            if (fxPosition == null)
            {
                fxPosition = new()
                {
                    AccountNo = parent.AccountNo,
                    Comment = position.TradeData.Comment,
                    Commission = (decimal)(position.Commission / Math.Pow(10, position.MoneyDigits)),
                    Direction = (TradeDirection)position.TradeData.TradeSide,
                    ExecutionPrice = (decimal)position.Price,
                    IsGuaranteedSL = position.GuaranteedStopLoss,
                    IsTrailingStopLoss = position.TrailingStopLoss,
                    Label = position.TradeData.Label,
                    OrderOpenedAt = DateTimeOffset.FromUnixTimeMilliseconds(position.TradeData.OpenTimestamp),
                    PositionId = pId,
                    Symbol = EnumUtil.ToEnum<Symbol>(name)
                };
                parent.Positions.Add(fxPosition);
            }
            fxPosition.LastUpdatedAt = DateTimeOffset.FromUnixTimeMilliseconds(position.UtcLastUpdateTimestamp);
            fxPosition.PositionStatus = (PositionStatus)position.PositionStatus;
            fxPosition.StopLoss = (decimal?)position.StopLoss;
            fxPosition.TakeProfit = (decimal?)position.TakeProfit;
            fxPosition.Volume = position.TradeData.Volume;
            fxPosition.Swap = (decimal)(position.Swap / Math.Pow(10, position.MoneyDigits));

            return fxPosition;
        }

    }
}
