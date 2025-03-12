using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using StockTracker.Components.Pages;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Numerics;
using System.Text.Json.Serialization;

namespace StockTracker.Data
{
    public class STdbContext : DbContext
    {
        public STdbContext(DbContextOptions<STdbContext> options)
        : base(options){
        
        }

        public DbSet<Stock> Stocks { get; set; }
        public DbSet<Alert> Alerts { get; set; }
    }

    /*
     * Class for observations from the nasdaq_screener table 
     */
    [Table("nasdaq_screener")]
    public class Stock
    {
        [Key]
        [Column("symbol")]
        public string Symbol { get; set; }
        
        [Column("name")]
        public string Name { get; set; }

        [JsonPropertyName("lastsale")]
        [NotMapped]
        public string LastSaleStr { get; set; }
        [JsonIgnore]
        [Column("last_sale")]
        public decimal LastSale { get; private set; }

        [JsonPropertyName("netchange")]
        [NotMapped]
        public string NetChangeStr {  get; set; }
        [JsonIgnore]
        [Column("net_change")]
        public decimal NetChange { get; private set; }

        [JsonPropertyName("pctchange")]
        [NotMapped]
        public string PercChangeStr { get; set; }
        [JsonIgnore]
        [Column("perc_change")]
        public decimal PercChange { get; private set; }

        [JsonPropertyName("marketCap")]
        [NotMapped]
        public string MarketCapStr { get; set; }
        [JsonIgnore]
        [Column("market_cap")]
        public decimal MarketCap { get; private set; }

        [Column("country")]
        public string Country { get; set; }
        
        [Column("ipo_year")]
        public string IpoYear { get; set; }

        [JsonPropertyName("volume")]
        [NotMapped]
        public string VolumeStr { get; set; }
        [JsonIgnore]
        [Column("volume")]
        public long Volume { get; private set; }

        [Column("sector")]
        public string Sector { get; set; }

        [Column("industry")]
        public string Industry { get; set; }

        [Column("url")]
        public string Url { get; set; }

        public Stock() { }  // Required by EF Core

        public static Stock FromJson(Stock stock)
        {
            stock.Symbol = stock.Symbol.TrimEnd();
            stock.LastSale = ParseDecimal(stock.LastSaleStr, true);
            stock.NetChange = ParseDecimal(stock.NetChangeStr, false);
            stock.PercChange = ParseDecimal(stock.PercChangeStr.TrimEnd('%'), false);
            stock.Volume = long.TryParse(stock.VolumeStr, out var v) ? v : 0;
            stock.MarketCap = ParseDecimal(stock.MarketCapStr, false);
            return stock;
        }

        //update curr stock with the one most recently retrieved
        public void updateStock(Stock stock)
        {
            Name = stock.Name;
            LastSale = stock.LastSale;
            NetChange = stock.NetChange;
            PercChange = stock.PercChange;
            MarketCap = stock.MarketCap;
            Country = stock.Country;
            IpoYear = stock.IpoYear;
            Volume = stock.Volume;
            Sector = stock.Sector;
            Industry = stock.Industry;
            Url = stock.Url;
        }
        private static decimal ParseDecimal(string input, bool removeCurrency)
        {
            if (string.IsNullOrWhiteSpace(input))
                return 0;

            if (removeCurrency)
                input = input.Replace("$", "").Trim();

            return decimal.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out var value) ? value : 0;
        }
    }


    public class Alert
    {
        [Key]
        public int AlertId { get; set; }  // Unique primary key
        public string Symbol { get; set; }
        public long Threshold { get; set; }
        public int Condition { get; set; }
        public bool Active { get; set; }
        public string Email { get; set; }
    }

}
