using Microsoft.EntityFrameworkCore;

namespace SmartFxJournal.JournalDB.model
{
    public partial class JournalDbContext : DbContext
    {
        public JournalDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<CTraderAccount> CTraderAccounts { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            Account.OnModelCreate(modelBuilder);
            CTraderAccount.OnModelCreate(modelBuilder);
        }
    }
}
