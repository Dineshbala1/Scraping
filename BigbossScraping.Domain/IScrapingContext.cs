using System.Threading.Tasks;
using BigbossScraping.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BigbossScraping.Domain
{
    public interface IScrapingContext
    {
        DbSet<ChannelCategory> ProgramCategory { get; set; }

        DbSet<ProgramDetails> ProgramDetails { get; set; }

        DbSet<ProgramMetadata> ProgramMetadata { get; set; }

        Task SaveDbChanges();
    }
}
