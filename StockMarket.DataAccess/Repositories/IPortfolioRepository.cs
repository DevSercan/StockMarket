using StockMarket.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockMarket.DataAccess.Repositories
{
    public interface IPortfolioRepository
    {
        Task<Portfolio?> GetByUserId(int id);
        Task<Portfolio?> GetByStockId(int id);
        Task<Portfolio?> Get(int id);
        Task<Portfolio> Create(Portfolio portfolio);
        Task Update(Portfolio portfolio);
        Task Delete(int id);
    }
}
