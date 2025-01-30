namespace StockTracker.BackgroundServices
{
    public interface IEmailService
    {
        Task Send(EmailMetadata emailMetadata);
    }
}
