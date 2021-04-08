using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using BigbossScraping.Contracts.Model;
using BigbossScraping.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BigbossScraping.Domain.Service.Implementation
{
    class SiteService : ISiteService
    {
        private readonly IScrapingContext _databaseContext;

        public SiteService(IScrapingContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public Task AddProgramCategory(Category category)
        {
            _databaseContext.ProgramCategory.AddAsync(new ChannelCategory { Url = category.Url, Name = category.Title, Id = Guid.NewGuid() });
            return _databaseContext.SaveDbChanges();
        }

        public Task AddProgramCategories(IList<Category> categories)
        {
            if (!categories.Any())
            {
                return Task.CompletedTask;
            }

            foreach (var category in categories)
            {
                _databaseContext.ProgramCategory.AddAsync(new ChannelCategory
                {
                    Url = category.Url,
                    Name = category.Title,
                    Id = Guid.NewGuid()
                });
            }

            return _databaseContext.SaveDbChanges();
        }

        public async Task<IEnumerable<ChannelCategory>> GetProgramCategories()
        {
            return await _databaseContext.ProgramCategory.AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<ChannelCategory>> GetProgramCategoriesByMatchString(Func<ChannelCategory, bool> filterPredicate)
        {
            return _databaseContext.ProgramCategory.AsEnumerable().Where(x => filterPredicate.Invoke(x)).ToList();
        }

        public async Task RemoveCategories()
        {
            var categories = await _databaseContext.ProgramCategory.AsNoTracking().ToListAsync();
            if (!categories.Any())
            {
                return;
                // throw new NotFound<ChannelCategory>($"No {nameof(ChannelCategory)} found");
            }
            _databaseContext.ProgramCategory.RemoveRange(categories);
            await _databaseContext.SaveDbChanges();
        }

        public async Task RemoveCategory(Guid categoryId)
        {
            var category = await _databaseContext.ProgramCategory.FindAsync(categoryId);
            if (category == null)
            {
                return;
                // throw new NotFound<ChannelCategory>($"{nameof(ChannelCategory)} not found for Id :{categoryId}");
            }

            _databaseContext.ProgramCategory.Remove(category);
            await _databaseContext.SaveDbChanges();
        }

        public async Task AddProgram(ProgramInformation programInformation)
        {
            await _databaseContext.ProgramMetadata.AddAsync(new ProgramMetadata
            {
                Id = programInformation.Id,
                Url = programInformation.Url,
                Title = programInformation.Title,
                Image = programInformation.Image,
                ImageAlternative = programInformation.ImageAlternative,
                ProgramCategoryId = programInformation.CategoryId
            });

            await _databaseContext.SaveDbChanges();
        }

        public async Task AddPrograms(IList<ProgramInformation> programInformationList)
        {
            if (!programInformationList.Any())
            {
                return;
                // throw new Exception("Requires at least one program to add it in the database");
            }

            foreach (var programInformation in programInformationList)
            {
                var existingKey = await _databaseContext.ProgramMetadata.FindAsync(programInformation.Id);
                if (existingKey != null)
                {
                    continue;
                }

                await _databaseContext.ProgramMetadata.AddAsync(new ProgramMetadata
                {
                    Id = programInformation.Id,
                    Url = programInformation.Url,
                    Title = programInformation.Title,
                    Image = programInformation.Image,
                    ImageAlternative = programInformation.ImageAlternative,
                    ProgramCategoryId = programInformation.CategoryId
                });
            }

            await _databaseContext.SaveDbChanges();
        }

        public async Task<IEnumerable<ProgramMetadata>> GetProgramInformationList()
        {
            return await _databaseContext.ProgramMetadata.AsNoTracking().ToListAsync();
        }

        public async Task<ProgramMetadata> GetProgramInformationList(
            Expression<Func<ProgramMetadata, bool>> filterPredicate)
        {
            return await _databaseContext.ProgramMetadata.FirstOrDefaultAsync(filterPredicate);
        }

        public async Task<IEnumerable<ProgramMetadata>> GetProgramInformationList(Guid categoryId)
        {
            if (categoryId == Guid.Empty)
            {
                return await GetProgramInformationList();
            }

            return await _databaseContext.ProgramMetadata.Where(x => x.ProgramCategoryId == categoryId)
                .AsNoTracking().ToListAsync();
        }

        public async Task<ProgramMetadata> GetProgramInformation(Guid programMetadataId)
        {
            var programMetadata = await _databaseContext.ProgramMetadata.FindAsync(programMetadataId);
            if (programMetadata == null)
            {
                //throw new NotFound<ProgramMetadata>($"Program not found for Id :{programMetadataId}");
            }

            return programMetadata;
        }

        public async Task RemoveProgram(Guid programMetadataId)
        {
            var programMetadataToRemove = await _databaseContext.ProgramMetadata.FindAsync(programMetadataId);
            if (programMetadataToRemove == null)
            {
                return;
                //throw new NotFound<ProgramMetadata>($"Program not found for Id :{programMetadataId}");
            }

            _databaseContext.ProgramMetadata.RemoveRange(programMetadataToRemove);
            await _databaseContext.SaveDbChanges();
        }

        public async Task RemovePrograms()
        {
            var programMetadataItemsToRemove = await _databaseContext.ProgramMetadata.AsNoTracking().ToListAsync();
            if (!programMetadataItemsToRemove.Any())
            {
                return;
                //throw new NotFound<ProgramMetadata>($"Programs not found");
            }

            _databaseContext.ProgramMetadata.RemoveRange(programMetadataItemsToRemove);
            await _databaseContext.SaveDbChanges();
        }

        public async Task<IEnumerable<ProgramDetails>> GetAllProgramDetails()
        {
            return await _databaseContext.ProgramDetails.ToListAsync();
        }

        public Task<ProgramDetails> GetProgramDetails(Guid programId)
        {
            return _databaseContext.ProgramDetails.AsNoTracking().SingleOrDefaultAsync(x => x.ProgramId == programId);
        }

        public Task AddProgramDetails(ProgrammeDetails programmeDetails)
        {
            _databaseContext.ProgramDetails.AddAsync(new ProgramDetails
            {
                Id = programmeDetails.Id,
                Content = programmeDetails.Content,
                ProgramId = programmeDetails.ProgramId,
                Title = programmeDetails.Title,
                VideoBanner = programmeDetails.VideoBanner,
                VideoUrl = programmeDetails.VideoUrl.First()
            });

            return _databaseContext.SaveDbChanges();
        }
    }
}
