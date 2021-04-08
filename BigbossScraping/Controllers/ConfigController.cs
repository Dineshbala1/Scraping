using System;
using System.Threading.Tasks;
using BigbossScraping.Contracts.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BigbossScraping.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigController : ControllerBase
    {
        private readonly ILogger<ConfigController> _logger;
        private readonly IScheduledSiteInformationService _scheduledSiteInformationService;

        public ConfigController(
            ILogger<ConfigController> logger,
            IScheduledSiteInformationService scheduledSiteInformationService)
        {
            _logger = logger;
            _scheduledSiteInformationService = scheduledSiteInformationService;
        }

        [HttpDelete]
        [Route("jobs")]
        public IActionResult PurgeQueuedJobs()
        {
            try
            {
                return Ok();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                throw;
            }
        }

        [HttpDelete]
        [Route("programs")]
        public async Task<IActionResult> PurgeProgrammes()
        {
            try
            {
                await _scheduledSiteInformationService.PurgeAllPrograms();
                return Ok();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                throw;
            }
        }
    }
}
