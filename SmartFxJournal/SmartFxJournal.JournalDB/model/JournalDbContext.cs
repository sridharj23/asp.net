using Microsoft.EntityFrameworkCore;

namespace SmartFxJournal.JournalDB.model
{
    public partial class JournalDbContext : DbContext
    {
        public JournalDbContext(DbContextOptions options) : base(options)
        {
            
        }

        public DbSet<CTraderAccount> CTraderAccounts { get; set; }
        public DbSet<TradingAccount> TradingAccounts { get; set; }
        public DbSet<ClosedPosition> ClosedPositions { get; set; }
        public DbSet<ExecutedOrder> ExecutedOrders { get; set; }
        public DbSet<PositionAnalysisEntry> PositionAnalysisEntries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            CTraderAccount.OnModelCreate(modelBuilder);
            TradingAccount.OnModelCreate(modelBuilder);
            ClosedPosition.OnModelCreate(modelBuilder);
            ExecutedOrder.OnModelCreate(modelBuilder);
            PositionAnalysisEntry.OnModelCreate(modelBuilder);
            AnalysisJournalEntry.OnModelCreate(modelBuilder);
            PositionJournalEntry.OnModelCreate(modelBuilder);
        }
    }
}
