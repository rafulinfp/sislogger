using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SISLogger.Core;

namespace SISLogger.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsageController : ControllerBase
    {
        private readonly ILogger<UsageController> _logger;
        public UsageController(ILogger<UsageController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public void Write([FromBody] LogDetail logEntry)
        {
            Logger.WriteUsage(logEntry);
            _logger.LogInformation("Sent new Usage log entry to kibana: {message}", logEntry.Messages);

        }
    }
}
