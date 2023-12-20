using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace StockMarket.Entities
{
    public class Portfolio : IEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } // Primary Key
        public int UserId { get; set; } // Foreign Key
        public int StockId { get; set; } // Foreign Key
        public int Quantity { get; set; }

        [JsonIgnore]
        public User User { get; set; }
        [JsonIgnore]
        public Stock Stock { get; set; }
    }
}
