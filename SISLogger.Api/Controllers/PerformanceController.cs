using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SISLogger.Core;

namespace SISLogger.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PerformanceController : ControllerBase
    {
        private readonly ILogger<PerformanceController> _logger;
        public PerformanceController(ILogger<PerformanceController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public void Write([FromBody] LogDetail logEntry)
        {
            Logger.WritePerf(logEntry);
            _logger.LogInformation("Sent new Performance log entry to kibana: {message}", logEntry.Messages);

        }
    }
}
