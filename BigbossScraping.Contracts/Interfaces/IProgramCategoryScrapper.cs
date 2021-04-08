using System;
using System.Collections.Generic;
using BigbossScraping.Contracts.Model;

namespace BigbossScraping.Contracts.Interfaces
{
    public interface IProgramCategoryScrapper : IScrapper
    {
        Action<IDictionary<string, IList<Category>>> ParsedResponse { get; set; }
    }
}