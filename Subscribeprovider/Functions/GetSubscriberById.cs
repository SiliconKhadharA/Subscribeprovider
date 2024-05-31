using Data.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Subscribeprovider.Functions
{
    public class GetSubscriberById
    {
        private readonly ILogger<GetSubscriberById> _logger;
        private readonly DataContext _context;

        public GetSubscriberById(ILogger<GetSubscriberById> logger, DataContext context)
        {
            _logger = logger;
            _context = context;
        }

        [Function("GetSubscriberById")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "subscriber/{id}")] HttpRequest req,
            string id)
        {
            _logger.LogInformation("Retrieving subscriber with ID: {Id}", id);

            try
            {
                var subscriber = await _context.Subscribers.FirstOrDefaultAsync(s => s.Id == id);
                return subscriber != null ? new OkObjectResult(subscriber) : new NotFoundObjectResult(new { Status = 404, Message = "Subscriber not found." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the subscriber.");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
