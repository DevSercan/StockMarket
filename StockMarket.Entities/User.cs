using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace StockMarket.Entities
{
    public class User : IEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } // Primary Key
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int RoleId { get; set; } // Foreign Key
        public decimal Balance { get; set; }

        [JsonIgnore]
        public Role? Role { get; set; }
        [JsonIgnore]
        public ICollection<Portfolio>? Portfolios { get; set; }
        [JsonIgnore]
        public ICollection<Transaction>? Transactions { get; set; }
        [JsonIgnore]
        public ICollection<BalanceCard>? BalanceCards { get; set; }

    }
}
