using StockMarket.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockMarket.DataAccess.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByEmail(string email);
        Task<User?> Get(int id);
        Task<User> Create(User user);
        Task Update(User user);
        Task Delete(int id);
        Task ChangeUserRole(int userId, int roleId);
        Task UpdateBalance(int userId, decimal balance);
        Task<decimal> GetBalanceById(int id);
        Task<List<User>> GetByRoleId(int roleId);
    }
}
