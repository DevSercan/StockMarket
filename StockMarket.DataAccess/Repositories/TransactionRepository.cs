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
    public class TransactionRepository : ITransactionRepository
    {
        private readonly StockMarketContext _context;
        public TransactionRepository(StockMarketContext context)
        {
            _context = context;
        }

        public async Task<Transaction?> Get(int id)
        {
            return await _context.Transactions.FindAsync(id);
        }

        public async Task<Transaction> Create(Transaction transaction)
        {
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();
            return transaction;
        }

        public async Task Update(Transaction transaction)
        {
            _context.Entry(transaction).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var transactionToDelete = await _context.Transactions.FindAsync(id);
            if (transactionToDelete != null)
            {
                _context.Transactions.Remove(transactionToDelete);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Transaction?> GetByUserId(int userId)
        {
            return await _context.Transactions.Where(t => t.UserId == userId).OrderByDescending(t => t.Date).FirstOrDefaultAsync();
        }
    }
}
