using Data.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Subscribeprovider.Functions
{
    public class DeleteSubscription
    {
        private readonly ILogger<DeleteSubscription> _logger;
        private readonly DataContext _context;

        public DeleteSubscription(ILogger<DeleteSubscription> logger, DataContext context)
        {
            _logger = logger;
            _context = context;
        }

        [Function("DeleteSubscription")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "subscribers/{email}")] HttpRequest req, string email)
        {
            _logger.LogInformation("Processing a subscription delete request for email: {Email}", email);

            try
            {
                var existingSubscriber = await _context.Subscribers.FirstOrDefaultAsync(s => s.Email == email);
                if (existingSubscriber != null)
                {
                    _context.Subscribers.Remove(existingSubscriber);
                    await _context.SaveChangesAsync();
                    return new OkResult();
                }

                _logger.LogWarning("Subscriber with email {Email} not found.", email);
                return new NotFoundResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the subscription delete request.");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
