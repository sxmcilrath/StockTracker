using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using StockTracker.Data;
using System.Text.Json;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;

//TODO 
/*
 * Need to switch this to using the nasdaq json
 */
namespace StockTracker.BackgroundServices
{
    public class StockCheckerService : BackgroundService
    {
        private readonly ILogger<StockCheckerService> _logger;
        private readonly IServiceProvider _serviceProvider; 

        //we use the factory rather than injecting HTTPClient because it has better resc mngmnt and is 
        //meant to be reused without socket exhaustion 
        private readonly IHttpClientFactory _httpClientFactory;

        

        public StockCheckerService(ILogger<StockCheckerService> logger, IHttpClientFactory httpClientFactory, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _serviceProvider = serviceProvider;
        }

        //I believe this is the function that must be defined
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("TASK STARTED");
            while (!stoppingToken.IsCancellationRequested)
            {

                //get current time
                var now = DateTime.Now;

                //Check if we're within 8AM - 5PM
                if (now.Hour >= 8 && now.Hour < 17)
                {
                    _logger.LogInformation("inside hours\n");
                    await CheckStocksAsync();
                }
                else
                {
                    _logger.LogInformation("outside hours");
                    await CheckStocksAsync();
                }
                  

                //wait for 5 minutes
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }

        private async Task UpdateDB()
        {
            List<Stock> stocksFromApi;


            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync("https://api.nasdaq.com/api/screener/stocks?tableonly=false&limit=25&download=true");

            response.EnsureSuccessStatusCode();

            //read the JSON
            var jsonString = await response.Content.ReadAsStringAsync();

            //parse 
            using (JsonDocument jsonDoc = JsonDocument.Parse(jsonString))
            {
                var rows = jsonDoc.RootElement.GetProperty("data").GetProperty("rows");

                // Set up options to ignore case (if needed) and ignore unknown properties.
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    IgnoreNullValues = true  // For System.Text.Json versions prior to .NET 5.0. For .NET 5+, use DefaultIgnoreCondition
                };

                // Deserialize the rows into a list of Stock objects.
                stocksFromApi = JsonSerializer.Deserialize<List<Stock>>(rows.GetRawText(), options);
            }

            //get db context
            var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<STdbContext>();
            
            //upsert to table 
            foreach (var stockItem in stocksFromApi)
            {
                //first see if stock exists
                var existingStock = dbContext.Stocks.FirstOrDefault(s => s.Symbol == stockItem.Symbol);

                //already exists, must update 
                if (existingStock != null)
                {
                    existingStock.Name = stockItem.Name;
                    existingStock.LastSaleStr = stockItem.LastSaleStr;
                    existingStock.NetChange = stockItem.NetChange;
                    existingStock.PercChangeStr = stockItem.PercChangeStr;
                    existingStock.MarketCap = stockItem.MarketCap;
                    existingStock.Country = stockItem.Country;
                    existingStock.IpoYear = stockItem.IpoYear;
                    existingStock.Volume = stockItem.Volume;
                    existingStock.Sector = stockItem.Sector;
                    existingStock.Industry = stockItem.Industry;
                    existingStock.url = stockItem.url;
                }
            }
        }
        /*
         * TODO
         * keep monitoring share volume number 
         */
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
                _logger.LogInformation($"Share volume as string: {shareVolumeString}");

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
                        $"Volume for {symbols} is {shareVolumeString} (interpreted as: {volume}"
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
