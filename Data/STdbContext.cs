using Microsoft.EntityFrameworkCore;

namespace StockTracker.Data
{
    public class STdbContext
    {
        public STdbContext(DbContextOptions<STdbContext> options)
        : base(options){
        }


    }
}
