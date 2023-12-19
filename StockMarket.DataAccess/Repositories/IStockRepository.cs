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
        Task<List<Stock>> GetAll();
        Task<Stock> Create(Stock stock);
        Task Update(Stock stock);
        Task Delete(int id);
        Task<Stock?> GetByName(string name);
        Task ChangeStockActivity(int id, bool isActive);
        Task UpdatePriceByName(string name, decimal price);
        Task UpdateQuantityById(int id, int quantity);
        Task<decimal> GetPriceById(int id);
        Task<bool> GetActivityById(int id);
        Task<int> GetQuantityById(int id);
    }
}
