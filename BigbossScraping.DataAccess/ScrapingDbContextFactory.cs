using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace BigbossScraping.DataAccess
{
    public class ScrapingDbContextFactory : IDesignTimeDbContextFactory<ScrapingDbContext>
    {
        private const string SettingsFile = "appSettings.json";
        private const string ConnectionString = "SqlConnection";

        public ScrapingDbContext CreateDbContext(string[] args)
        {
#if !DEBUG
            // At the moment, we generate sql files from the migrations and use those in the deployment
            // so the connection string here is only used in Visual Studio, that's why it's hard-coded.
            var optionsBuilder = new DbContextOptionsBuilder<ScrapingDbContext>();
            var configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(SettingsFile).Build();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString(ConnectionString));
            return new ScrapingDbContext(optionsBuilder.Options);
#else

            var optionsBuilder = new DbContextOptionsBuilder<ScrapingDbContext>()
                .UseSqlServer("Data Source=OV-DUB-LTP-198;Initial Catalog=BigbossManagement;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False",
                    o => o.MigrationsHistoryTable("__EFMigrationsHistory", ScrapingDbContext.Schema));

            return new ScrapingDbContext(optionsBuilder.Options);
#endif
        }
    }
}