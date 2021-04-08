using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using BigbossScraping.Contracts;
using BigbossScraping.Contracts.Model;
using IronWebScraper;

namespace BigbossScraping.IronScraper.Extensions
{
    static class ScraperExtension
    {

        public static HtmlNode[] GetHeaderNodes(this HtmlNode response)
        {
            return response.GetElementsByTagName(HtmlNodeConstants.Header);
        }

        public static HtmlNode[] GetArticleNodes(this HtmlNode response)
        {
            return response.GetElementsByTagName(HtmlNodeConstants.Article);
        }

        public static HtmlNode[] GetAnchorNodes(this HtmlNode response)
        {
            return response.GetElementsByTagName(HtmlNodeConstants.AnchorNode);
        }

        public static HtmlNode[] GetIFrameNodes(this HtmlNode response)
        {
            return response.GetElementsByTagName(HtmlNodeConstants.IFrame);
        }

        public static HtmlNode GetFirstHeaderNode(this HtmlNode response)
        {
            return response.GetElementsByTagName(HtmlNodeConstants.Header).FirstOrDefault();
        }

        public static HtmlNode GetFirstHeader1Node(this HtmlNode response)
        {
            return response.GetElementsByTagName(HtmlNodeConstants.Header1).FirstOrDefault();
        }

        public static HtmlNode GetFirstArticleNode(this HtmlNode response)
        {
            return response.GetElementsByTagName(HtmlNodeConstants.Article).FirstOrDefault();
        }

        public static HtmlNode GetFirstAnchorNode(this HtmlNode response)
        {
            return response.GetElementsByTagName(HtmlNodeConstants.AnchorNode).FirstOrDefault();
        }

        public static HtmlNode[] GetImageNodes(this HtmlNode response)
        {
            return response.GetElementsByTagName(HtmlNodeConstants.Image);
        }

        public static HtmlNode GetFirstImageNode(this HtmlNode response)
        {
            return response.GetElementsByTagName(HtmlNodeConstants.Image).FirstOrDefault();
        }

        public static HtmlNode GetFirstIFrameNode(this HtmlNode response)
        {
            return response.GetElementsByTagName(HtmlNodeConstants.IFrame).FirstOrDefault();
        }

        public static string GetHyperlinkReferenceFromAnchor(this HtmlNode node)
        {
            return node?.GetAttribute(AttributeConstants.HrefAttribute) ?? ErrorConstants.NotAvailable;
        }

        public static string GetSourceAttribute(this HtmlNode node)
        {
            return node?.GetAttribute(AttributeConstants.SrcAttribute) ?? ErrorConstants.NotAvailable;
        }

        public static string GetAltFromImage(this HtmlNode node)
        {
            return node?.GetAttribute(AttributeConstants.AlternativeAttribute) ?? ErrorConstants.NotAvailable;
        }

        public static string GetCleanTextFromNode(this HtmlNode node)
        {
            return node?.InnerTextClean ?? ErrorConstants.NotAvailable;
        }

        public static IList<HtmlNode[]> GetRelatedPostNode(this HtmlNode node)
        {
            return node.Css(".related_posts").Select(x => x.GetElementsByTagName("li")).ToList();
        }

        public static string GetRelatedPost(this HtmlNode node)
        {
            return node.Css(".related-post-title").FirstOrDefault()?.InnerTextClean;
        }

        public static ProgramInformation GetArticleFromNode(this HtmlNode res, string categoryId = null, Func<string, Guid> findParentId = null)
        {
            var cardUrl = res.GetFirstAnchorNode().GetHyperlinkReferenceFromAnchor();
            var cardImg = res.GetFirstAnchorNode().GetFirstImageNode().GetSourceAttribute();
            var cardImgAlt = res.GetFirstAnchorNode().GetFirstImageNode().GetAltFromImage();
            var cardContent = res.GetFirstHeaderNode()?.GetCleanTextFromNode();

            return new ProgramInformation
            {
                Id = Guid.NewGuid(),
                Title = cardContent,
                Image = cardImg,
                Url = cardUrl,
                ImageAlternative = cardImgAlt,
                CategoryId = findParentId?.Invoke(cardContent) ?? (categoryId != null ? Guid.Parse(categoryId) : Guid.Empty)
            };
        }

        public static DateTime? GetFirstDateFromString(this string inputText)
        {
            var regex = new Regex(@"\b\d{2}\-\d{2}-\d{4}\b");
            foreach (Match m in regex.Matches(inputText))
            {
                DateTime dt;
                if (DateTime.TryParseExact(m.Value, "dd-MM-yyyy", null, DateTimeStyles.None, out dt))
                    return dt;
            }
            return null;
        }
    }
}
