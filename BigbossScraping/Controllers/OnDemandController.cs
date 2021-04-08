using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using BigbossScraping.Contracts.Interfaces;
using BigbossScraping.Contracts.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BigbossScraping.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces(MediaTypeNames.Application.Json)]
    public class OnDemandController : ControllerBase
    {
        private readonly ILogger<OnDemandController> _logger;
        private readonly IOnDemandSiteInformationService _onDemandSiteInformationService;

        public OnDemandController(ILogger<OnDemandController> logger,
            IOnDemandSiteInformationService onDemandSiteInformationService)
        {
            _logger = logger;
            _onDemandSiteInformationService = onDemandSiteInformationService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Route("{homeUrl}/categories", Name = "GetCategories")]
        public async Task<ActionResult> GetCategories(string homeUrl)
        {
            if (string.IsNullOrEmpty(homeUrl))
            {
                return BadRequest($"{nameof(homeUrl)} cannot be empty or null");
            }

            homeUrl = Uri.UnescapeDataString(homeUrl);

            return Ok(await _onDemandSiteInformationService.GetCategories(homeUrl));
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Route("{categoryUrl}/articles", Name = "GetArticles")]
        public async Task<ActionResult> GetArticlesForCategory(string categoryUrl)
        {
            if (string.IsNullOrEmpty(categoryUrl))
            {
                return BadRequest($"{nameof(categoryUrl)} cannot be empty or null");
            }

            categoryUrl = Uri.UnescapeDataString(categoryUrl);

            var result = await _onDemandSiteInformationService.GetArticlesList(categoryUrl);

            return Ok(result);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Route("{articleUrl}/article", Name = "GetArticle")]
        public async Task<ActionResult> GetArticle(string articleUrl)
        {
            if (string.IsNullOrEmpty(articleUrl))
            {
                return BadRequest($"{nameof(articleUrl)} cannot be empty or null");
            }

            articleUrl = Uri.UnescapeDataString(articleUrl);

            return Ok(await _onDemandSiteInformationService.GetProgramInformation(articleUrl));
        }
    }
}
