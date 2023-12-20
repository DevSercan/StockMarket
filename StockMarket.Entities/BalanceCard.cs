using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace StockMarket.Entities
{
    public class BalanceCard : IEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } // Primary Key
        public int UserId { get; set; } // Foreign Key
        public string Code { get; set; }
        public decimal Balance { get; set; }
        public bool IsUsed { get; set; }

        [JsonIgnore]
        public User User { get; set; }
    }
}
