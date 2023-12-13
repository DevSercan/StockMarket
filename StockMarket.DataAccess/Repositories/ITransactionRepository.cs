using StockMarket.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockMarket.DataAccess.Repositories
{
    public interface ITransactionRepository
    {
        Task<Transaction?> Get(int id);
        Task<Transaction> Create(Transaction transaction);
        Task Update(Transaction transaction);
        Task Delete(int id);
        Task<Transaction?> GetByUserId(int userId);
    }
}
