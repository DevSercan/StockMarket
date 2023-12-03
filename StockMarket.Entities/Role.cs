using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockMarket.Entities
{
    public class Role : IEntity
    {
        public int Id { get; set; } // Primary Key
        public string Name { get; set; }
    }
}
