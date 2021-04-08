using System;
using System.Collections.Generic;

namespace BigbossScraping.Contracts.Model
{
    public class ProgrammeDetails
    {
        public string Title { get; set; }
        public string EpisodeDate { get; set; }
        public string Content { get; set; }
        public string[] VideoUrl { get; set; }
        public string VideoBanner { get; set; }
        public Guid ProgramId { get; set; }
        public Guid Id { get; set; }
        public IList<ProgramInformation> ProgramInformations { get; set; }

        public static ProgrammeDetails Default => new ProgrammeDetails();
    }
}
