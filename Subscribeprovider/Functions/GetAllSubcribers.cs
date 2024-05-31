using Data.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Subscribeprovider.Functions
{
    public class GetAllSubscribers
    {
        private readonly ILogger<GetAllSubscribers> _logger;
        private readonly DataContext _context;

        public GetAllSubscribers(ILogger<GetAllSubscribers> logger, DataContext context)
        {
            _logger = logger;
            _context = context;
        }

        [Function("GetAllSubscribers")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "subscribers")] HttpRequest req)
        {
            _logger.LogInformation("Retrieving all subscribers.");

            try
            {
                var subscribers = await _context.Subscribers.ToListAsync();
                return new OkObjectResult(subscribers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all subscribers.");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
