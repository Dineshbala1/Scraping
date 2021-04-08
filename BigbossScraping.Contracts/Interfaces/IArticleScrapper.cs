using System;
using BigbossScraping.Contracts.Model;

namespace BigbossScraping.Contracts.Interfaces
{
    public interface IArticleScrapper : IScrapper
    {
        Action<PagedProgramInformation> ParsedResponse { get; set; }
    }
}