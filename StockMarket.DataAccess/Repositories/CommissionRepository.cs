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
    public class CommissionRepository : ICommissionRepository
    {
        private readonly StockMarketContext _context;
        public CommissionRepository(StockMarketContext context)
        {
            _context = context;
        }

        public async Task<Commission?> Get(int id)
        {
            return await _context.Commissions.FindAsync(id);
        }

        public async Task<List<Commission>> GetAll()
        {
            return await _context.Commissions.ToListAsync();
        }

        public async Task UpdateCommissionRate(int id, decimal commissionRate)
        {
            var commission = await _context.Commissions.FindAsync(id);
            if (commission == null)
            {
                throw new ArgumentException("Invalid Commission Id");
            }
            commission.CommissionRate = Math.Round(commissionRate, 3);
            _context.Entry(commission).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task<decimal> GetCommissionRateById(int id)
        {
            var commission = await _context.Commissions.FirstOrDefaultAsync(c => c.Id == id);
            if (commission == null)
            {
                throw new ArgumentException("Invalid Commission Id");
            }
            return commission.CommissionRate;
        }
    }
}
