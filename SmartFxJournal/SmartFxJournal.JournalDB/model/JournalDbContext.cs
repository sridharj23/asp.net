using Microsoft.EntityFrameworkCore;

namespace SmartFxJournal.JournalDB.model
{
    public partial class JournalDbContext : DbContext
    {
        public JournalDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<CTraderAccount> CTraderAccounts { get; set; }
        public DbSet<FxAccount> FxAccounts { get; set; }
        public DbSet<FxPosition> FxPositions { get; set; }
        public DbSet<FxHistoricalTrade> FxOrderHistory { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            CTraderAccount.OnModelCreate(modelBuilder);
            FxAccount.OnModelCreate(modelBuilder);
            FxPosition.OnModelCreate(modelBuilder);
            FxHistoricalTrade.OnModelCreate(modelBuilder);
        }
    }
}
