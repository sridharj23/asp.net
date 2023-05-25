using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace SmartFxJournal.JournalDB.model
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<JournalDbContext>
    {
        public JournalDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                                                .SetBasePath(Directory.GetCurrentDirectory())
                                                .AddJsonFile("appsettings.json")
                                                .Build();
            var options = new DbContextOptionsBuilder<JournalDbContext>();
            options.UseNpgsql(configuration.GetConnectionString("JournalDB")).UseSnakeCaseNamingConvention();
            return new JournalDbContext(options.Options);
        }
    }
}
