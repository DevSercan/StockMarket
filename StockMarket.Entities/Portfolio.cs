using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockMarket.Entities
{
    public class Portfolio : IEntity
    {
        public int Id { get; set; } // Primary Key
        public int UserId { get; set; } // Foreign Key
        public int StockId { get; set; } // Foreign Key
    }
}
