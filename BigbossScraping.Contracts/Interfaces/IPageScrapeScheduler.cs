namespace BigbossScraping.Contracts.Interfaces
{
    public interface IPageScrapeScheduler
    {
        string ScheduleArticleJob(string parameters, string parentJob = null, string groupId = null);

        string ScheduleCategoryJob(string parameters, string parentJob = null, string groupId = null);

        string ScheduleProgramJob(string parameters, string parentJob = null, string groupId = null);

        string SchedulePagedArticleJob(string pageUrl, string parentJob = null, string groupId = null);

        string ScheduleParsingJobFromCategory(bool includeHome = false);

        string ScheduleHomeParsingJob(string pageUrl);

        void RaisePageScrapeCompleted(string payload, ResponseType responseType);

        void CreateRecurringJob(string url, int jobId);

        void PurgeJobs();
    }
}