using System.Threading.Tasks;

namespace BigbossScraping.Contracts.Interfaces
{
    public interface IScrapper
    {
        Task StartPageScraper(string pageUrl, bool onDemand = false, string groupId = null);
    }
}