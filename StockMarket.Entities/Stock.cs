using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace StockMarket.Entities
{
    public class Stock : IEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } // Primary Key
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public bool IsActive { get; set; }

        [JsonIgnore]
        public ICollection<Portfolio> Portfolios { get; set; }
        [JsonIgnore]
        public ICollection<Transaction> Transactions { get; set; }
        [JsonIgnore]
        public ICollection<PriceHistory> PriceHistories { get; set; }
    }
}
