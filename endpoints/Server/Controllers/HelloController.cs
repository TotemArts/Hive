using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Amazon.SecretsManager;
using Hive.Endpoints.Server.Extensions;
using Hive.Endpoints.Server.Models;
using Microsoft.Extensions.Logging;

namespace Hive.Endpoints.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HelloController : ControllerBase
    {
        private readonly ILogger<HelloController> _logger;
        private readonly IAmazonSecretsManager _secrets;

        public HelloController(ILogger<HelloController> logger, IAmazonSecretsManager secrets)
        {
            _logger = logger;
            _secrets = secrets;
        }

        public async Task<IActionResult> Index()
        {
            var result = await _secrets.GetValue<TestSecret>("test_secret");

            _logger?.LogWarning($"Fetched secret: {result.Hello}");

            return Content("Successful.");
        }
    }
}
