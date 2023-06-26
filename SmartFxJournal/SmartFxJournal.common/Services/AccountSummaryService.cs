﻿using Microsoft.Extensions.DependencyInjection;
using SmartFxJournal.Common.Model;
using SmartFxJournal.JournalDB.model;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace SmartFxJournal.Common.Services
{
    public class AccountSummaryService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public AccountSummaryService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        private void CollectAggregate(ClosedPosition tr, SummaryAggregate agg)
        {
            decimal tot = tr.GrossProfit + tr.Commission + tr.Swap;
            agg.TotalPL += tot;
            if (tr.Direction == GlobalEnums.TradeDirection.BUY)
            {
                agg.PLFromLongs += tot;
            }
            else
            {
                agg.PLFromShorts += tot;
            }
        }

        public async Task<List<SummaryAggregate>> GetSummaryAggregatesAsync(long AccountNo)
        {
            Dictionary<int, SummaryAggregate> aggregates = new();

            // Prepare aggregates
            int curYear = DateTime.Now.Year;
            int curMonth = DateTime.Now.Month;
            int prevYear = curYear - 1;

            aggregates.Add(curYear, new SummaryAggregate(curYear.ToString()));
            aggregates.Add(prevYear, new SummaryAggregate(prevYear.ToString()));

            for (int i = 1; i <= curMonth; i++)
            {
                string key = new DateTime(curYear, i, 1).ToString("MMM", CultureInfo.InvariantCulture) + "-" + curYear.ToString();
                aggregates.Add(i, new SummaryAggregate(key));
            }

            using (var serviceScope = _scopeFactory.CreateScope())
            {
                JournalDbContext _context = serviceScope.ServiceProvider.GetService<JournalDbContext>() ?? throw new ArgumentNullException(nameof(serviceScope));
                TradingAccount acc = await _context.TradingAccounts.Include(a => a.Positions).FirstAsync(a => a.AccountNo == AccountNo);
                List<ClosedPosition> trades = acc.Positions.Where(o => o.OrderClosedAt.Year >= prevYear).OrderBy(o => o.OrderClosedAt).ToList();

                foreach (ClosedPosition tr in trades)
                {
                    int month = tr.OrderClosedAt.Month;
                    if (tr.OrderClosedAt.Year == prevYear)
                    {
                        CollectAggregate(tr, aggregates[prevYear]);
                    }
                    else if (tr.OrderClosedAt.Year == curYear)
                    {
                        CollectAggregate(tr, aggregates[curYear]);
                        if (month > 0)
                        {
                            CollectAggregate(tr, aggregates[month]);
                        }
                    }
                }

            }
            // Collect aggregate data

            return aggregates.Values.ToList().FindAll(a => a.TotalPL != 0);
        }

        public async Task<EquityCurve> GetEquityCurveAsync(long AccountNo)
        {
            EquityCurve curve = new(AccountNo);

            using (var serviceScope = _scopeFactory.CreateScope())
            {
                JournalDbContext _context = serviceScope.ServiceProvider.GetService<JournalDbContext>() ?? throw new ArgumentNullException(nameof(serviceScope));

                TradingAccount acc = await _context.TradingAccounts.Include(a => a.Positions).FirstAsync(a => a.AccountNo == AccountNo);
                List<ClosedPosition> trades = acc.Positions.OrderBy(o => o.OrderClosedAt).ToList();

                decimal lastBal = acc.StartBalance;
                DateOnly at = acc.OpenedOn;
                DateOnly cur = DateOnly.MinValue;
                long datePoint = DateOnlyToMs(at);

                // Add account start balance first
                //curve.DataPoints.Add(new(lastBal, datePoint));

                foreach (ClosedPosition tra in trades)
                {
                    if (tra.PositionId < 0)
                    {
                        continue; // Reconcilation position
                    }

                    if (!at.Equals(cur))
                    {
                        curve.DataPoints.Add(new(lastBal, datePoint));
                        cur = DateOnly.FromDateTime(tra.OrderClosedAt.DateTime);
                        at = cur;
                    }

                    cur = DateOnly.FromDateTime(tra.OrderClosedAt.DateTime);
                    datePoint = DateOnlyToMs(cur);
                    lastBal = tra.BalanceAfter;
                }
                curve.DataPoints.Add(new(lastBal, datePoint));
            }
            return curve;
        }

        private long DateOnlyToMs(DateOnly date)
        {
            DateTime dt = date.ToDateTime(TimeOnly.FromTimeSpan(TimeSpan.FromHours(2)));
            return new DateTimeOffset(dt).ToUnixTimeMilliseconds();
        }

    }
}