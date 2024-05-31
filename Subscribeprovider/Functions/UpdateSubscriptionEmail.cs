using Data.Context;
using Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Subscribeprovider.Functions
{
    public class UpdateSubscriptionEmail
    {
        private readonly ILogger<UpdateSubscriptionEmail> _logger;
        private readonly DataContext _context;

        public UpdateSubscriptionEmail(ILogger<UpdateSubscriptionEmail> logger, DataContext context)
        {
            _logger = logger;
            _context = context;
        }

        [Function("UpdateSubscriptionEmail")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "subscription/update-email")] HttpRequest req)
        {
            _logger.LogInformation("Processing an email update request.");

            try
            {
                var body = await new StreamReader(req.Body).ReadToEndAsync();
                var updateRequest = JsonConvert.DeserializeObject<UpdateEmailRequest>(body);

                if (updateRequest == null || string.IsNullOrWhiteSpace(updateRequest.Id) || string.IsNullOrWhiteSpace(updateRequest.NewEmail))
                {
                    _logger.LogWarning("Invalid request payload: {Payload}", body);
                    return new BadRequestObjectResult(new { Status = 400, Message = "Invalid request." });
                }

                var subscriber = await _context.Subscribers.FirstOrDefaultAsync(s => s.Id == updateRequest.Id);
                if (subscriber == null)
                {
                    _logger.LogWarning("Subscriber with ID {Id} not found.", updateRequest.Id);
                    return new NotFoundObjectResult(new { Status = 404, Message = "Subscriber not found." });
                }

                // Remove the existing subscriber
                _context.Subscribers.Remove(subscriber);
                await _context.SaveChangesAsync();

                // Add a new subscriber with the new email
                var newSubscriber = new SubscribeEntity
                {
                    Id = updateRequest.Id,
                    Email = updateRequest.NewEmail
                };
                _context.Subscribers.Add(newSubscriber);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Email updated successfully for subscriber ID {Id}.", updateRequest.Id);
                return new OkObjectResult(new { Status = 200, Message = "Email updated successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the email.");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }

    public class UpdateEmailRequest
    {
        public string Id { get; set; }
        public string NewEmail { get; set; }
    }
}
