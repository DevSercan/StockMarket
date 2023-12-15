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
    public class BalanceCardRepository : IBalanceCardRepository
    {
        private readonly StockMarketContext _context;
        public BalanceCardRepository(StockMarketContext context)
        {
            _context = context;
        }

        public async Task<BalanceCard?> Get(int id)
        {
            return await _context.BalanceCards.FindAsync(id);
        }

        public async Task<List<BalanceCard>> GetAll()
        {
            return await _context.BalanceCards.ToListAsync();
        }

        public async Task Update(BalanceCard balanceCard)
        {
            _context.Entry(balanceCard).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task<BalanceCard> Create(BalanceCard balanceCard)
        {
            _context.BalanceCards.Add(balanceCard);
            await _context.SaveChangesAsync();
            return balanceCard;
        }

        public async Task Delete(int id)
        {
            var balanceCardToDelete = await _context.BalanceCards.FindAsync(id);
            if (balanceCardToDelete != null)
            {
                _context.BalanceCards.Remove(balanceCardToDelete);
                await _context.SaveChangesAsync();
            }
        }

        public async Task ChangeCardUsageById(int id, bool isUsed)
        {
            var balanceCard = await _context.BalanceCards.FindAsync(id);
            if (balanceCard == null)
            {
                throw new ArgumentException("Invalid Balance Card Id");
            }
            balanceCard.IsUsed = isUsed;
            _context.Entry(balanceCard).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task UpdateUserId(int id, int userId)
        {
            var balanceCard = await _context.BalanceCards.FindAsync(id);
            if (balanceCard == null)
            {
                throw new ArgumentException("Invalid Balance Card Id");
            }
            balanceCard.UserId = userId;
            _context.Entry(balanceCard).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task<List<BalanceCard>> GetByUserId(int userId)
        {
            return await _context.BalanceCards.Where(bc => bc.UserId == userId).ToListAsync();
        }

        public async Task<BalanceCard?> GetByCode(string code)
        {
            return await _context.BalanceCards.FirstOrDefaultAsync(bc => bc.Code == code);
        }
    }
}
