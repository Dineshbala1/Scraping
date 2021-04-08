using System;

namespace BigbossScraping.Contracts.Model
{
    public class Category
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public Guid Id { get; set; }
    }
}