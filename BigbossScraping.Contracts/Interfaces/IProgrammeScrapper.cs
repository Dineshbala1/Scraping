using System;
using BigbossScraping.Contracts.Model;

namespace BigbossScraping.Contracts.Interfaces
{
    public interface IProgrammeScrapper : IScrapper
    {
        Action<ProgrammeDetails> ParsedResponse { get; set; }
    }
}