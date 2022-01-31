using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SISLogger.Core;

namespace SISLogger.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ErrorController : ControllerBase
    {
        private readonly ILogger<ErrorController> _logger;
        public ErrorController(ILogger<ErrorController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public void Write([FromBody] LogDetail logEntry)
        {
            Logger.WriteError(logEntry);
            _logger.LogInformation("Sent new Error log entry to kibana: {message}", logEntry.Messages);
        }
    }
}
