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
    public class PortfolioRepository : IPortfolioRepository
    {
        private readonly StockMarketContext _context;
        public PortfolioRepository(StockMarketContext context)
        {
            _context = context;
        }

        public async Task<Portfolio> Create(Portfolio portfolio)
        {
            _context.Portfolios.Add(portfolio);
            await _context.SaveChangesAsync();
            return portfolio;
        }

        public async Task Delete(int id)
        {
            var portfolioToDelete = await _context.Portfolios.FindAsync(id);
            if (portfolioToDelete != null)
            {
                _context.Portfolios.Remove(portfolioToDelete);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Portfolio?> Get(int id)
        {
            return await _context.Portfolios.FindAsync(id);
        }

        public async Task<Portfolio?> GetByStockId(int id)
        {
            return await _context.Portfolios.FirstOrDefaultAsync(p => p.StockId == id);
        }

        public async Task<Portfolio?> GetByUserId(int id)
        {
            return await _context.Portfolios.FirstOrDefaultAsync(p => p.UserId == id);
        }

        public async Task Update(Portfolio portfolio)
        {
            _context.Entry(portfolio).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}
