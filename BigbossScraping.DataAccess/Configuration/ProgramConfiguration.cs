using BigbossScraping.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BigbossScraping.DataAccess.Configuration
{
    public class ProgramConfiguration : IEntityTypeConfiguration<ProgramMetadata>
    {
        public void Configure(EntityTypeBuilder<ProgramMetadata> builder)
        {
            builder.ToTable(nameof(ProgramMetadata));

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Title).HasMaxLength(250);
            builder.Property(x => x.Image).HasMaxLength(1000);
            builder.Property(x => x.ImageAlternative).HasMaxLength(1000);
            builder.Property(x => x.Url).HasMaxLength(1000);

            builder.HasOne<ChannelCategory>().WithMany().HasForeignKey(x => x.ProgramCategoryId);
        }
    }
}