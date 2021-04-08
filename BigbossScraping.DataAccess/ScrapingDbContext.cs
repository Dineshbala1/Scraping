using System.Threading.Tasks;
using BigbossScraping.Domain;
using BigbossScraping.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BigbossScraping.DataAccess
{
    public class ScrapingDbContext : DbContext, IScrapingContext
    {
        public const string Schema = "BigBoss";

        public ScrapingDbContext(DbContextOptions options) : base(options)
        {
            
        }

        public DbSet<ChannelCategory> ProgramCategory { get; set; }
        public DbSet<ProgramDetails> ProgramDetails { get; set; }
        public DbSet<ProgramMetadata> ProgramMetadata { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasDefaultSchema(schema: Schema);
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        }

        public async Task SaveDbChanges()
        {
            await base.SaveChangesAsync();
        }
    }
}
