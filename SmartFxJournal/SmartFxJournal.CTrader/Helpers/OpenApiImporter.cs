using OpenAPI.Net.Helpers;
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
                var transactions = await openApiService.GetTransactions(accountId, act.IsLive, start, end);
                entry.StartBalance = (decimal)(transactions[0].Balance / 100);
                entry.CreatedOn = DateTime.Now;
                
                parent.FxAccounts.Add(entry);
            }
            entry.CurrentBalance = (decimal)MonetaryConverter.FromMonetary(trader.Balance);
            entry.LastModifiedOn = DateTime.Now;

            return entry;
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
                    ExecutionPrice = (decimal?)position.Price,
                    IsGuaranteedSL = position.GuaranteedStopLoss,
                    IsTrailingStopLoss = position.TrailingStopLoss,
                    Label = position.TradeData.Label,
                    OrderOpenedAt = DateTimeOffset.FromUnixTimeMilliseconds(position.TradeData.OpenTimestamp),
                    PositionId = pId,
                    SymbolName = name
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
