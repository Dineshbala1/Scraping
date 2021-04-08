using System.Collections.Generic;
using System.Threading.Tasks;
using BigbossScraping.Contracts.Model;

namespace BigbossScraping.Contracts.Interfaces
{
    public interface IOnDemandSiteInformationService
    {
        Task<IDictionary<string, IList<Category>>> GetCategories(string siteUrl);

        Task<PagedProgramInformation> GetArticlesList(string categoryUrl);

        Task<ProgrammeDetails> GetProgramInformation(string articleUrl);
    }
}