using System.Threading.Tasks;

namespace BigbossScraping.Contracts.Interfaces
{
    public interface IScheduledSiteInformationService : ISiteInformationService
    {
        Task StartSiteParsing(string siteUrl);

        void ScheduleUpdateJob(string url, int jobId);

        Task AddResponseToStorage(ResponsePayload responsePayload);

        Task StartCategoryParsing(bool includeHome = false);

        Task PurgeAllPrograms();
    }
}