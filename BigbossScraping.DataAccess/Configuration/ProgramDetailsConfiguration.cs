using BigbossScraping.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BigbossScraping.DataAccess.Configuration
{
    public class ProgramDetailsConfiguration : IEntityTypeConfiguration<ProgramDetails>
    {
        public void Configure(EntityTypeBuilder<ProgramDetails> builder)
        {
            builder.ToTable(nameof(ProgramDetails));

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Title).IsRequired().HasMaxLength(250);
            builder.Property(x => x.Content).HasColumnType("text");
            builder.Property(x => x.VideoUrl).HasMaxLength(750);
            builder.Property(x => x.VideoBanner).HasMaxLength(750);

            builder.HasOne<ProgramMetadata>().WithOne().HasForeignKey<ProgramDetails>(x => x.ProgramId);
        }
    }
}