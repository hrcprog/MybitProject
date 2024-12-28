using MybitProject.Shared.Dtos.PushNotification;

namespace MybitProject.Client.Maui.Platforms.Windows.Services;

public partial class WindowsPushNotificationService : PushNotificationServiceBase
{
    public override Task<PushNotificationSubscriptionDto> GetSubscription(CancellationToken cancellationToken) => 
        throw new NotImplementedException();
}
