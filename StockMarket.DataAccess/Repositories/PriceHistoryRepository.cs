using Microsoft.EntityFrameworkCore;
using StockMarket.DataAccess.Context;
using StockMarket.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockMarket.DataAccess.Repositories
{
    public class PriceHistoryRepository : IPriceHistoryRepository
    {
        private readonly StockMarketContext _context;
        public PriceHistoryRepository(StockMarketContext context)
        {
            _context = context;
        }

        public async Task<PriceHistory?> Get(int id)
        {
            return await _context.PriceHistories.FindAsync(id);
        }

        public async Task<PriceHistory> Create(int stockId, decimal price)
        {
            DateTime date = DateTime.Now;
            var priceHistory = new PriceHistory
            {
                StockId = stockId,
                Date = date,
                Price = price
            };
            _context.PriceHistories.Add(priceHistory);
            await _context.SaveChangesAsync();
            return priceHistory;
        }

        public async Task Update(PriceHistory priceHistory)
        {
            _context.Entry(priceHistory).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var priceHistoryToDelete = await _context.PriceHistories.FindAsync(id);
            if (priceHistoryToDelete != null)
            {
                _context.PriceHistories.Remove(priceHistoryToDelete);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<PriceHistory?> GetByStockId(int stockId)
        {
            return await _context.PriceHistories.Where(ph => ph.StockId == stockId).OrderByDescending(ph => ph.Date).FirstOrDefaultAsync();
        }

    }
}
