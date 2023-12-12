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
    public class StockRepository : IStockRepository
    {
        private readonly StockMarketContext _context;
        public StockRepository(StockMarketContext context)
        {
            _context = context;
        }

        public async Task ChangeStockActivity(int id, bool isActive)
        {
            var stock = await _context.Stocks.FindAsync(id);
            if (stock == null)
            {
                throw new ArgumentException("Invalid Stock Id");
            }
            stock.IsActive = isActive;
            _context.Entry(stock).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task<Stock> Create(Stock stock)
        {
            _context.Stocks.Add(stock);
            await _context.SaveChangesAsync();
            return stock;
        }

        public async Task Delete(int id)
        {
            var stockToDelete = await _context.Stocks.FindAsync(id);
            if (stockToDelete != null)
            {
                _context.Stocks.Remove(stockToDelete);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Stock?> Get(int id)
        {
            return await _context.Stocks.FindAsync(id);
        }

        public async Task Update(Stock stock)
        {
            _context.Entry(stock).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}
