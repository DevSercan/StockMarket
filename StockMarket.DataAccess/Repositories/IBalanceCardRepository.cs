using StockMarket.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockMarket.DataAccess.Repositories
{
    public interface IBalanceCardRepository
    {
        Task<BalanceCard?> Get(int id);
        Task<List<BalanceCard>> GetAll();
        Task Update(BalanceCard balanceCard);
        Task<BalanceCard> Create(BalanceCard balanceCard);
        Task Delete(int id);
        Task ChangeCardUsageById(int id, bool isUsed);
        Task UpdateUserId(int id, int userId);
        Task<List<BalanceCard>> GetByUserId(int userId);
        Task<BalanceCard?> GetByCode(string code);
    }
}
