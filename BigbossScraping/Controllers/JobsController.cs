using System;
using System.Threading.Tasks;
using BigbossScraping.Contracts.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BigbossScraping.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JobsController : ControllerBase
    {
        private const int LatestProgramJob = 2689752;

        private readonly IScheduledSiteInformationService _scheduledSiteInformationService;
        private readonly ILogger<JobsController> _logger;

        public JobsController(IScheduledSiteInformationService scheduledSiteInformationService, ILogger<JobsController> logger)
        {
            _scheduledSiteInformationService = scheduledSiteInformationService;
            _logger = logger;
        }

        [HttpOptions]
        [Route("schedule/recurring")]
        public IActionResult ScheduleRecurring()
        {
            try
            {
                _scheduledSiteInformationService.ScheduleUpdateJob("http://www.biggboss4.net", LatestProgramJob);
                return AcceptedAtAction(nameof(ScheduleRecurring));
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actionUrl">Url that has to be scraped</param>
        /// <returns>200 if the URL is valid and the job has been scheduled successfully</returns>
        [HttpOptions]
        [Route("schedule/parsing")]
        public async Task<IActionResult> SchedulePageScraping([FromQuery] string actionUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(actionUrl))
                {
                    return BadRequest($"{actionUrl} cannot be empty");
                }

                if (!Uri.IsWellFormedUriString(actionUrl, UriKind.Absolute))
                {
                    return UnprocessableEntity($"{actionUrl} - malformed url");
                }

                await _scheduledSiteInformationService.StartSiteParsing(actionUrl);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
            }

            return AcceptedAtAction(nameof(SchedulePageScraping));
        }
    }
}