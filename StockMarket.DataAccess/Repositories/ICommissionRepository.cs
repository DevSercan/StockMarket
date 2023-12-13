using StockMarket.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockMarket.DataAccess.Repositories
{
    public interface ICommissionRepository
    {
        Task<Commission?> Get(int id);
        Task<List<Commission>> GetAll();
        Task UpdateCommissionRate(int id, decimal commissionRate);
        Task<decimal> GetCommissionRateById(int id);
    }
}
