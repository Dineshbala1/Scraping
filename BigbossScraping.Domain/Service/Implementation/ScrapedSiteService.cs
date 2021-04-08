using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using BigbossScraping.Contracts.Interfaces;
using BigbossScraping.Contracts.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BigbossScraping.Domain.Service.Implementation
{
    class ScrapedSiteService : IScheduledSiteInformationService
    {
        private readonly ISiteService _siteService;
        private readonly IPageScrapeScheduler _pageScrapeScheduler;
        private readonly ILogger<ScrapedSiteService> _logger;

        public ScrapedSiteService(ISiteService siteService, IPageScrapeScheduler pageScrapeScheduler, ILogger<ScrapedSiteService> logger)
        {
            _siteService = siteService;
            _pageScrapeScheduler = pageScrapeScheduler;
            _logger = logger;
        }

        Task IScheduledSiteInformationService.PurgeAllPrograms()
        {
            return _siteService.RemovePrograms();
        }

        async Task<IList<Category>> ISiteInformationService.GetCategories()
        {
            var categories = await _siteService.GetProgramCategories();
            return categories.Select(x => x.Transform(x)).ToList();
        }

        async Task<Category> ISiteInformationService.GetCategoryFromSearchString(string searchString)
        {
            var result = new Category();

            try
            {
                var categories =
                    await _siteService.GetProgramCategoriesByMatchString(category => EF.Functions.Like(category.Name, searchString));
                var programCategories = categories.ToList();
                if (programCategories.Any())
                {
                    result = programCategories.Select(x => x.Transform(x)).First();
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                Debugger.Break();
            }

            return result;
        }

        Task ISiteInformationService.RemoveCategories()
        {
            try
            {
                return _siteService.RemoveCategories();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                Debugger.Break();
                return Task.CompletedTask;
            }
        }

        Task ISiteInformationService.RemoveCategory(Guid categoryId)
        {
            try
            {
                return _siteService.RemoveCategory(categoryId);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                Debugger.Break();
                return Task.CompletedTask;
            }
        }

        async Task<IEnumerable<ProgramInformation>> ISiteInformationService.GetProgramInformationList()
        {
            _logger.LogWarning("GetProgramInformationList invoked from the controller");
            return await GetProgramInformationList(Guid.Empty);
        }

        async Task<IEnumerable<ProgramInformation>> ISiteInformationService.GetProgramInformationListByCategory(
            Guid categoryId)
        {
            return await GetProgramInformationList(categoryId);
        }

        async Task<ProgramInformation> ISiteInformationService.GetProgramInformation(Guid programId)
        {
            try
            {
                var programFound = await _siteService.GetProgramInformation(programId);
                return new ProgramInformation
                {
                    Url = programFound.Url,
                    Id = programFound.Id,
                    CategoryId = programFound.ProgramCategoryId,
                    Image = programFound.Image,
                    ImageAlternative = programFound.ImageAlternative,
                    Title = programFound.Title
                };
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
            }

            return new ProgramInformation();
        }

        async Task<ProgrammeDetails> ISiteInformationService.GetProgramDetails(Guid programId)
        {
            try
            {
                var result = await _siteService.GetProgramDetails(programId);
                return new ProgrammeDetails
                {
                    Id = result.Id,
                    Content = result.Content,
                    ProgramId = result.ProgramId,
                    Title = result.Title,
                    VideoBanner = result.VideoBanner,
                    VideoUrl = new[] { result.VideoUrl }
                };
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
            }

            return ProgrammeDetails.Default;
        }

        Task IScheduledSiteInformationService.StartSiteParsing(string siteUrl)
        {
            _pageScrapeScheduler.ScheduleCategoryJob(siteUrl);
            return Task.CompletedTask;
        }

        void IScheduledSiteInformationService.ScheduleUpdateJob(string url, int jobId)
        {
            _pageScrapeScheduler.CreateRecurringJob(url, jobId);
        }

        async Task IScheduledSiteInformationService.AddResponseToStorage(ResponsePayload responsePayload)
        {
            try
            {
                if (!string.IsNullOrEmpty(responsePayload.Response))
                {
                    switch (responsePayload.ResponseType)
                    {
                        case ResponseType.Article:
                            {
                                await PopulatePrograms(responsePayload.Response);
                                break;
                            }
                        case ResponseType.Programme:
                            {
                                await PopulateProgramDetails(responsePayload.Response);
                                break;
                            }
                        case ResponseType.Category:
                            {
                                await PopulateCategory(responsePayload.Response).ContinueWith(async (task) =>
                                    await PopulateCategoriesContinuation(task));
                                break;
                            }
                    }
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                Debugger.Break();
            }
        }

        async Task IScheduledSiteInformationService.StartCategoryParsing(bool includeHome)
        {
            try
            {
                string parentJob = null;
                var categories = await _siteService.GetProgramCategories();
                foreach (var programCategory in categories)
                {
                    if (string.Equals(programCategory.Name, "Home", StringComparison.OrdinalIgnoreCase) && includeHome)
                    {
                        _logger.LogWarning($"{programCategory.Name} is currently being processed scheduler is invoked with {programCategory.Url}");
                        _pageScrapeScheduler.ScheduleHomeParsingJob(programCategory.Url);
                        break;
                    }

                    if (!includeHome &&
                        !string.Equals(programCategory.Name, "Home", StringComparison.OrdinalIgnoreCase))
                    {
                        parentJob = _pageScrapeScheduler.SchedulePagedArticleJob(programCategory.Url, parentJob,
                            programCategory.Id.ToString());

                        _logger.LogWarning(
                            $"{programCategory.Name} is currently being processed scheduler is invoked with {programCategory.Url}");
                        Debugger.Break();
                    }

                    await Task.Delay(500);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                Debugger.Break();
            }
        }

        private async Task PopulateCategoriesContinuation(Task task)
        {
            if (task.Exception != null && (task.IsFaulted || task.IsCanceled ||
                                           task.Exception.InnerExceptions.Any()))
            {
                if (task.Exception.InnerExceptions.Any(x =>
                    x.GetType() == typeof(CategoryNotChangedException)))
                {
                    await StartJob(true);
                }
                else
                {
                    // This is a failure 
                    foreach (var exception in task.Exception.InnerExceptions)
                    {
                        _logger.LogError(exception, exception.Message);
                    }
                }
            }
            else
            {
                await StartJob(false);
            }
        }

        private async Task StartJob(bool includeHome)
        {
            _pageScrapeScheduler.ScheduleParsingJobFromCategory(includeHome);
            await Task.CompletedTask;
        }

        private async Task PopulateCategory(string payload)
        {
            try
            {
                var categories = JsonConvert.DeserializeObject<IDictionary<string, IList<Category>>>(payload);
                var existingProgramCategories = await _siteService.GetProgramCategories();

                if (categories != null && categories.Any())
                {
                    if (existingProgramCategories.Count() == categories.Values.SelectMany(x => x).Count())
                    {
                        throw new CategoryNotChangedException("Categories has not changed");
                    }

                    await _siteService.AddProgramCategories(categories.Values.SelectMany(x => x).ToList());
                }
            }

            catch (CategoryNotChangedException exception)
            {
                _logger.LogError(exception, exception.Message);
                Debugger.Break();
                throw;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                Debugger.Break();
                throw;
            }
        }

        private async Task PopulatePrograms(string payload)
        {
            try
            {
                var articles = JsonConvert.DeserializeObject<IList<ProgramInformation>>(payload);

                if (articles != null && articles.Any())
                {
                    await _siteService.AddPrograms(articles);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                Debugger.Break();
            }
        }

        private async Task PopulateProgramDetails(string payload)
        {
            try
            {
                var detail = JsonConvert.DeserializeObject<ProgrammeDetails>(payload);
                var existingProgrammes = await _siteService.GetAllProgramDetails();
                if (detail.ProgramId == Guid.Empty)
                {
                    var getProgramMetadata =
                        await _siteService.GetProgramInformationList(metadata => metadata.Title == detail.Title);
                    if (getProgramMetadata != null)
                    {
                        detail.ProgramId = getProgramMetadata.Id;
                    }
                }

                if (!existingProgrammes.Any(x => x.Title.Equals(detail.Title)))
                {
                    await _siteService.AddProgramDetails(detail);
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                Debugger.Break();
            }
        }

        private async Task<IEnumerable<ProgramInformation>> GetProgramInformationList(Guid categoryId)
        {
            try
            {
                _logger.LogWarning($"GetProgramInformationList invoked from the interface implementation {categoryId}");
                var result = categoryId == Guid.Empty ? await _siteService.GetProgramInformationList() : await _siteService.GetProgramInformationList(categoryId);
                _logger.LogWarning($"GetProgramInformationList resulted with {result.Count()}");
                return result.Select(x => new ProgramInformation
                {
                    Url = x.Url,
                    Id = x.Id,
                    CategoryId = x.ProgramCategoryId,
                    Image = x.Image,
                    ImageAlternative = x.ImageAlternative,
                    Title = x.Title
                }).ToList();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                Debugger.Break();
            }

            return Enumerable.Empty<ProgramInformation>();
        }
    }
}
