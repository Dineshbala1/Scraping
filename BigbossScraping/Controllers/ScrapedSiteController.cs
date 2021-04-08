using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Mime;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BigbossScraping.Contracts.Interfaces;
using BigbossScraping.Contracts.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BigbossScraping.Controllers
{
    [Route("api/[controller]")]
    [Produces(MediaTypeNames.Application.Json)]
    [ApiController]
    public class ScrapedSiteController : ControllerBase
    {
        private readonly ILogger<ScrapedSiteController> _logger;
        private readonly IScheduledSiteInformationService _scheduledSiteInformationService;

        public ScrapedSiteController(ILogger<ScrapedSiteController> logger, IScheduledSiteInformationService scheduledSiteInformationService)
        {
            _logger = logger;
            _scheduledSiteInformationService = scheduledSiteInformationService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("categories")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IList<Category>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(NotFoundResult))]
        public async Task<IActionResult> GetProgramCategories()
        {
            try
            {
                var result = await _scheduledSiteInformationService.GetCategories();
                if (!result.Any())
                {
                    return NotFound();
                }
                return Ok(result.Select(x => new { title = x.Title, url = x.Url }).ToList());
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
            }

            return BadRequest();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{categoryName}/articles")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IList<ProgramInformation>))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(NotFoundResult))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetProgramForCategory(string categoryName)
        {
            try
            {
                var category = await _scheduledSiteInformationService.GetCategoryFromSearchString(categoryName);
                if (category == null || category?.Id == Guid.Empty)
                {
                    return NotFound();
                }

                return Ok(await _scheduledSiteInformationService.GetProgramInformationListByCategory(category.Id));
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        [Route("articles")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IList<ProgramInformationDated>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAllPrograms()
        {
            try
            {
                var result = await _scheduledSiteInformationService.GetProgramInformationList();
                return Ok(result.Select(x => new ProgramInformationDated
                {
                    ShowTime = GetFirstDateFromString(x.Title),
                    Url = x.Url,
                    Id = x.Id,
                    CategoryId = x.CategoryId,
                    Image = x.Image,
                    ImageAlternative = x.ImageAlternative,
                    Title = x.Title
                }).OrderByDescending(dated => dated.ShowTime).ToList());

                DateTime? GetFirstDateFromString(string input)
                {
                    DateTime d;

                    string patternString = "{0}-{1}-{2}";

                    try
                    {
                        // Exclude strings with no matching substring
                        if (input.Contains("|") && (input.Contains("th") || input.Contains("st")))
                        {
                            var dateString = input.Substring(input.IndexOf('|') + 1, 3);
                            var monthString = input.Substring(input.IndexOf("th") + 2,
                                (input.IndexOf("20") - input.IndexOf("th") - 2));
                            var yearString = input.Substring(input.IndexOf('2'), 4);
                            var datetimeString = string.Format(patternString, dateString, monthString, yearString);
                            DateTime.TryParse(datetimeString, out var result);
                            return result;
                        }

                        foreach (Match m in Regex.Matches(input, @"\d{2}\-\d{2}\-\d{4}"))
                        {
                            // Exclude matching substrings which aren't valid DateTimes
                            if (DateTime.TryParseExact(m.Value, "dd-MM-yyyy", null,
                                DateTimeStyles.None, out d))
                            {
                                return d;
                            }
                        }

                        return DateTime.Now;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, ex.Message);
                        return DateTime.Now;
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        [Route("articles/{articleId}/details")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProgrammeDetails))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetProgramDetails(Guid articleId)
        {
            try
            {
                return Ok(await _scheduledSiteInformationService.GetProgramDetails(articleId));
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return BadRequest(e.Message);
            }
        }
    }
}