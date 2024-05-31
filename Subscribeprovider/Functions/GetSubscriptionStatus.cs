using Data.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Subscribeprovider.Functions
{
	public class GetSubscriptionStatus
	{
		private readonly ILogger<GetSubscriptionStatus> _logger;
		private readonly DataContext _context;

		public GetSubscriptionStatus(ILogger<GetSubscriptionStatus> logger, DataContext context)
		{
			_logger = logger;
			_context = context;
		}

		[Function("GetSubscriptionStatus")]
		public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "subscription/{email}")] HttpRequest req, string email)
		{
			var existingSubscriber = await _context.Subscribers.FirstOrDefaultAsync(s => s.Email == email);
			return existingSubscriber != null ? new OkObjectResult(true) : new OkObjectResult(false);
		}
	}
}
