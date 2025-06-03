using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace MijnApp
{
    public class Function1
    {
        private readonly ILogger<Function1> _logger;
        private readonly IConfiguration _config;

        public Function1(ILogger<Function1> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        [Function("MijnFunction")]
        public IActionResult RunX([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route ="RX/{a:int}/{b:int}")] HttpRequest req, int a, int b)
        {
            var data = _config["Test:Setting1"];
            _logger.LogInformation($"C# HTTP trigger function processed a request. [{data}]");
            var res = a + b;
            return new OkObjectResult($"Welcome to Azure Functions! [{data}] Het antwoorrd is {res}");
        }

        [Function("TimerFunction")]
        public void Run([TimerTrigger("0 */1 * * * *")] TimerInfo myTimer)
        {
            _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            if (myTimer.ScheduleStatus is not null)
            {
                _logger.LogInformation($"Next timer schedule at: {myTimer.ScheduleStatus.Next}");
            }
        }
    }
}
