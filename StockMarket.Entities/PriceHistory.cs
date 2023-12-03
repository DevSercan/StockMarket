using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockMarket.Entities
{
    public class PriceHistory : IEntity
    {
        public int Id { get; set; } // Primary Key
        public int StockId { get; set; } // Foreign Key
        public string Date { get; set; } // DateTime
        public decimal Price { get; set; }
    }
}
