using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace StockTracker.BackgroundServices
{
    public class StockCheckerService : BackgroundService
    {
        private readonly ILogger<StockCheckerService> _logger;

        //we use the factory rather than injecting HTTPClient because it has better resc mngmnt and is 
        //meant to be reused without socket exhaustion 
        private readonly IHttpClientFactory _httpClientFactory;

        public StockCheckerService(ILogger<StockCheckerService> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        //I believe this is the function that must be defined
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {

                //get current time
                var now = DateTime.Now;

                //Check if we're within 8AM - 5PM
                if (now.Hour >= 8 && now.Hour < 17)
                {
                    await CheckStocksAsync();
                }
                else
                {
                    await CheckStocksAsync();
                }
                  

                //wait for 5 minutes
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }

        private async Task CheckStocksAsync()
        {
            try
            {
                _logger.LogInformation("Checking stock vol...");

                //call the python scraper API
                var client = _httpClientFactory.CreateClient();
                var symbols = "AAPL"; //will need to change later
                var response = await client.GetAsync($"http://localhost:5000/get-stock-info?symbols={symbols}");

                response.EnsureSuccessStatusCode();
                var shareVolumeString = await response.Content.ReadAsStringAsync();

                //convert shareVolume to integer 
                if (int.TryParse(shareVolumeString.Replace(",", ""), out int volume)) ;

                //check threshold
                int threshold = 1000;

                if (volume > threshold)
                {
                    //if the threshold is met then send the email
                    await EmailNotificationService.SendEmailAsync(
                        "stocktracker34@gmail.com",
                        "Stock Alert",
                        $"Volume for {symbols} is {volume}"
                        );
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking stock volume.");
            }
        }
    }
}
