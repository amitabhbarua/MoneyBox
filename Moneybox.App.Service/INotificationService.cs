namespace Moneybox.App.Services
{
    public interface INotificationService
    {
        void NotifyApproachingPayInLimit(string emailAddress);

        void NotifyFundsLow(string emailAddress);
    }
}
