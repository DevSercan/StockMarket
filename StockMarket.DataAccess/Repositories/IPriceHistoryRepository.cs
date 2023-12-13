using StockMarket.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockMarket.DataAccess.Repositories
{
    public interface IPriceHistoryRepository
    {
        Task<PriceHistory?> Get(int id);
        Task<PriceHistory> Create(int stockId, decimal price);
        Task Update(PriceHistory priceHistory);
        Task Delete(int id);
        Task<PriceHistory?> GetByStockId(int stockId);
    }
}
