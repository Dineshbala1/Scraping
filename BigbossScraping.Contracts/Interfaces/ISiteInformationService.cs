using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BigbossScraping.Contracts.Model;

namespace BigbossScraping.Contracts.Interfaces
{
    public interface ISiteInformationService
    {
        Task<IList<Category>> GetCategories();

        Task<Category> GetCategoryFromSearchString(string searchString);

        Task RemoveCategories();

        Task RemoveCategory(Guid categoryId);

        Task<IEnumerable<ProgramInformation>> GetProgramInformationList();

        Task<IEnumerable<ProgramInformation>> GetProgramInformationListByCategory(Guid categoryId);

        Task<ProgramInformation> GetProgramInformation(Guid programId);

        Task<ProgrammeDetails> GetProgramDetails(Guid programId);
    }
}
