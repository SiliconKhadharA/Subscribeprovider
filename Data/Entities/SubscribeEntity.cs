
using System.ComponentModel.DataAnnotations;


namespace Data.Entities;

public class SubscribeEntity
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Key]
    public string Email { get; set; } = null!;

    public bool DailyNewsletter { get; set; }

    public bool AdvertisingUpdates { get; set; }

    public bool WeekInReview { get; set; }

    public bool EventUpdates { get; set; }

    public bool StartupWeekly { get; set; }

    public bool Podcasts { get; set; }
}
