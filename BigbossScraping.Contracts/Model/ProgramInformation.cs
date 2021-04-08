using System;
using System.Collections.Generic;

namespace BigbossScraping.Contracts.Model
{
    public class ProgramInformation
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public string Image { get; set; }
        public string ImageAlternative { get; set; }
        public Guid CategoryId { get; set; }
        public Guid Id { get; set; }
    }

    public class ProgramInformationDated : ProgramInformation
    {
        public DateTime? ShowTime { get; set; }
    }

    public class Pagination
    {
        public string PageNumber { get; set; }
        public string PageUrl { get; set; }
        public bool IsCurrent { get; set; }
    }

    public class PagedProgramInformation
    {
        public IList<Pagination> PaginationDetail { get; set; }
        public IList<ProgramInformation> ProgramInformations { get; set; }
    }
}