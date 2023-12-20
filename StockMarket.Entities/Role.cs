using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace StockMarket.Entities
{
    public class Role : IEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } // Primary Key
        public string Name { get; set; }

        [JsonIgnore]
        public ICollection<User>? Users { get; set; }
    }
}
