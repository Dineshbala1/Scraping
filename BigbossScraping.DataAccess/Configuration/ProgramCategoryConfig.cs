using BigbossScraping.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BigbossScraping.DataAccess.Configuration
{
    public class ProgramCategoryConfig : IEntityTypeConfiguration<ChannelCategory>
    {
        public void Configure(EntityTypeBuilder<ChannelCategory> builder)
        {
            builder.ToTable(nameof(ChannelCategory));

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).IsRequired().HasMaxLength(250);
            builder.Property(x => x.Url).IsRequired().HasMaxLength(1000);
        }
    }
}
