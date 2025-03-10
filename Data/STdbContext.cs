using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using StockTracker.Components.Pages;
using System.ComponentModel.DataAnnotations;
using System.Numerics;

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
    public class Stock
    {
        [Key]
        public string Symbol { get; set; }
        public string Name { get; set; }
        public string LastSaleStr { get; set; }
        public decimal NetChange { get; set; }
        public string PercChangeStr { get; set; }
        public decimal MarketCap { get; set; }
        public string Country { get; set; }
        public string IpoYear { get; set; }
        public long Volume { get; set; }
        public string Sector { get; set; }
        public string Industry { get; set; }
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
