using Microsoft.EntityFrameworkCore;
using StockMarket.DataAccess.Context;
using StockMarket.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockMarket.DataAccess.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly StockMarketContext _context;
        public RoleRepository(StockMarketContext context)
        {
            _context = context;
        }

        public async Task<Role?> Get(int id)
        {
            return await _context.Roles.FindAsync(id);
        }

    }
}
