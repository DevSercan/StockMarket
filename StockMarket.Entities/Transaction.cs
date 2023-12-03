using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockMarket.Entities
{
    public class Transaction : IEntity
    {
        public int Id { get; set; } // Primary Key
        public int UserId { get; set; } // Foreign Key
        public int StockId { get; set; } // Foreign Key
        public string Type { get; set; }
        public string Date { get; set; } // DateTime
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Commission { get; set; }
    }
}
