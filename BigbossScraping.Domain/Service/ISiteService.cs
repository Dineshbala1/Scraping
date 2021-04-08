using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using BigbossScraping.Contracts.Model;
using BigbossScraping.Domain.Entities;

namespace BigbossScraping.Domain.Service
{
    interface ISiteService
    {
        Task AddProgramCategory(Contracts.Model.Category category);

        Task AddProgramCategories(IList<Contracts.Model.Category> categories);

        Task<IEnumerable<ChannelCategory>> GetProgramCategories();

        Task<IEnumerable<ChannelCategory>> GetProgramCategoriesByMatchString(
            Func<ChannelCategory, bool> filterPredicate);

        Task RemoveCategories();

        Task RemoveCategory(Guid categoryId);

        Task AddProgram(ProgramInformation programInformation);

        Task AddPrograms(IList<ProgramInformation> programInformationList);

        Task<IEnumerable<ProgramMetadata>> GetProgramInformationList();

        Task<IEnumerable<ProgramMetadata>> GetProgramInformationList(Guid categoryId);

        Task<ProgramMetadata> GetProgramInformationList(
            Expression<Func<ProgramMetadata, bool>> filterPredicate);

        Task<ProgramMetadata> GetProgramInformation(Guid programMetadataId);

        Task RemoveProgram(Guid programId);

        Task RemovePrograms();

        Task<IEnumerable<ProgramDetails>> GetAllProgramDetails();

        Task<ProgramDetails> GetProgramDetails(Guid programId);

        Task AddProgramDetails(ProgrammeDetails programmeDetails);
    }
}
