using StockMarket.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockMarket.DataAccess.Repositories
{
    public interface IStockRepository
    {
        Task<Stock?> Get(int id);
        Task<Stock> Create(Stock stock);
        Task Update(Stock stock);
        Task Delete(int id);
        Task ChangeStockActivity(int id, bool isActive);
        
    }
}
