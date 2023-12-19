using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<TransactionRepository> _logger;
        public TransactionRepository(StockMarketContext context, ILogger<TransactionRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Transaction?> Get(int id)
        {
            _logger.LogInformation($"Getting Transaction by Id: {id}");
            try
            {
                var transaction = await _context.Transactions.FindAsync(id);
                if (transaction != null)
                {
                    _logger.LogInformation($"Successfully retrieved Transaction by Id: {id}");
                }
                else
                {
                    _logger.LogWarning($"Transaction with Id: {id} not found");
                }
                return transaction;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting Transaction by Id: {id}. Error: {ex.Message}");
                throw;
            }
        }

        public async Task<Transaction> Create(Transaction transaction)
        {
            _logger.LogInformation("Creating new Transaction");
            try
            {
                _context.Transactions.Add(transaction);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Successfully created new Transaction with Id: {transaction.Id}");
                return transaction;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating Transaction. Error: {ex.Message}");
                throw;
            }
        }

        public async Task Update(Transaction transaction)
        {
            _logger.LogInformation($"Updating Transaction with Id: {transaction.Id}");
            try
            {
                _context.Entry(transaction).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Successfully updated Transaction with Id: {transaction.Id}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating Transaction with Id: {transaction.Id}. Error: {ex.Message}");
                throw;
            }
        }

        public async Task Delete(int id)
        {
            _logger.LogInformation($"Deleting Transaction with Id: {id}");
            try
            {
                var transactionToDelete = await _context.Transactions.FindAsync(id);
                if (transactionToDelete != null)
                {
                    _context.Transactions.Remove(transactionToDelete);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"Successfully deleted Transaction with Id: {id}");
                }
                else
                {
                    _logger.LogWarning($"Transaction with Id: {id} not found");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting Transaction with Id: {id}. Error: {ex.Message}");
                throw;
            }
        }

        public async Task<List<Transaction>> GetByUserId(int userId)
        {
            _logger.LogInformation($"Getting Transactions by UserId: {userId}");
            try
            {
                var transactions = await _context.Transactions
                    .Where(t => t.UserId == userId)
                    .OrderByDescending(t => t.Date)
                    .ToListAsync();

                if (transactions != null && transactions.Any())
                {
                    _logger.LogInformation($"Successfully retrieved Transactions by UserId: {userId}. Count: {transactions.Count}");
                }
                else
                {
                    _logger.LogWarning($"No transactions found for UserId: {userId}");
                }

                return transactions;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting Transactions by UserId: {userId}. Error: {ex.Message}");
                throw;
            }
        }

    }

}
