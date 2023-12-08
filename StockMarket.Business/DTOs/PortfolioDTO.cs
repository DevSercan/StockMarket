using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockMarket.Business.DTOs
{
    public class PortfolioDTO
    {
        public int UserId { get; set; } // Foreign Key
        public int StockId { get; set; } // Foreign Key
    }
}
