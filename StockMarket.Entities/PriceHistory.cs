using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace StockMarket.Entities
{
    public class PriceHistory : IEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } // Primary Key
        public int StockId { get; set; } // Foreign Key
        public DateTime Date { get; set; }
        public decimal Price { get; set; }

        [JsonIgnore]
        public Stock? Stock { get; set; }
    }
}
