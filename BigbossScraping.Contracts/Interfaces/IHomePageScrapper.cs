using System.Threading.Tasks;

namespace BigbossScraping.Contracts.Interfaces
{
    public interface IHomePageScrapper : IScrapper
    {
        Task Start(string pageUrl);
    }
}