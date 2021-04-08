using System;

namespace BigbossScraping.Domain.Entities
{
    public class ChannelCategory
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public DateTimeOffset Created { get; set; } = DateTimeOffset.Now;
        public DateTimeOffset LastModified { get; set; } = DateTimeOffset.Now;

        public Contracts.Model.Category Transform(ChannelCategory channelCategory)
        {
            return new Contracts.Model.Category
            {
                Id = channelCategory.Id,
                Url = channelCategory.Url,
                Title = channelCategory.Name
            };
        }
    }

    public class ProgramMetadata
    {
        public Guid Id { get; set; }
        public Guid ProgramCategoryId { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string Image { get; set; }
        public string ImageAlternative { get; set; }
        public DateTimeOffset Created { get; set; } = DateTimeOffset.Now;
        public DateTimeOffset LastModified { get; set; } = DateTimeOffset.Now;
    }

    public class ProgramDetails
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string VideoUrl { get; set; }
        public string VideoBanner { get; set; }
        public DateTimeOffset Created { get; set; } = DateTimeOffset.Now;
        public DateTimeOffset LastModified { get; set; } = DateTimeOffset.Now;
        public Guid ProgramId { get; set; }
    }
}
