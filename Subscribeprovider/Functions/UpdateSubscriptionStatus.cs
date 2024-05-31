using Data.Context;
using Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;

namespace Subscribeprovider.Functions
{
	public class UpdateSubscriptionStatus
	{
		private readonly ILogger<UpdateSubscriptionStatus> _logger;
		private readonly DataContext _context;

		public UpdateSubscriptionStatus(ILogger<UpdateSubscriptionStatus> logger, DataContext context)
		{
			_logger = logger;
			_context = context;
		}

		[Function("UpdateSubscriptionStatus")]
		public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
		{
			var body = await new StreamReader(req.Body).ReadToEndAsync();
			if (!string.IsNullOrEmpty(body))
			{
				var subscriptionRequest = JsonConvert.DeserializeObject<SubscriptionRequest>(body);
				if (subscriptionRequest != null)
				{
					var existingSubscriber = await _context.Subscribers.FirstOrDefaultAsync(s => s.Email == subscriptionRequest.Email);
					if (subscriptionRequest.IsSubscribed)
					{
						if (existingSubscriber == null)
						{
							_context.Subscribers.Add(new SubscribeEntity { Email = subscriptionRequest.Email });
						}
					}
					else
					{
						if (existingSubscriber != null)
						{
							_context.Subscribers.Remove(existingSubscriber);
						}
					}

					await _context.SaveChangesAsync();
					return new OkObjectResult(new { IsSubscribed = subscriptionRequest.IsSubscribed });
				}
			}
			return new BadRequestResult();
		}
	}

	public class SubscriptionRequest
	{
		public string? Email { get; set; }
		public bool IsSubscribed { get; set; }
	}
}
