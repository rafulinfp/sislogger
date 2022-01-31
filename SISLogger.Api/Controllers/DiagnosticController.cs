using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SISLogger.Core;

namespace SISLogger.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DiagnosticController : ControllerBase
    {
        private readonly ILogger<DiagnosticController> _logger;
        public DiagnosticController(ILogger<DiagnosticController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public void Write([FromBody] LogDetail logEntry)
        {
            Logger.WriteDiagnostic(logEntry);
            _logger.LogInformation("Sent new Diagnostic log entry to kibana: {message}", logEntry.Messages);

        }
    }
}
