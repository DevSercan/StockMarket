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
    public class PriceHistoryRepository : IPriceHistoryRepository
    {
        private readonly StockMarketContext _context;
        private readonly ILogger<PriceHistoryRepository> _logger;
        public PriceHistoryRepository(StockMarketContext context, ILogger<PriceHistoryRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<PriceHistory?> Get(int id)
        {
            _logger.LogInformation($"Getting PriceHistory by Id: {id}");
            try
            {
                var priceHistory = await _context.PriceHistories.FindAsync(id);
                if (priceHistory != null)
                {
                    _logger.LogInformation($"Successfully retrieved PriceHistory by Id: {id}");
                }
                else
                {
                    _logger.LogWarning($"PriceHistory with Id: {id} not found");
                }
                return priceHistory;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting PriceHistory by Id: {id}. Error: {ex.Message}");
                throw;
            }
        }

        public async Task<PriceHistory> Create(int stockId, decimal price)
        {
            _logger.LogInformation($"Creating PriceHistory for Stock Id: {stockId}");
            try
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

                _logger.LogInformation($"Successfully created PriceHistory for Stock Id: {stockId}");
                return priceHistory;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating PriceHistory for Stock Id: {stockId}. Error: {ex.Message}");
                throw;
            }
        }

        public async Task Update(PriceHistory priceHistory)
        {
            _logger.LogInformation($"Updating PriceHistory with Id: {priceHistory.Id}");
            try
            {
                _context.Entry(priceHistory).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Successfully updated PriceHistory with Id: {priceHistory.Id}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating PriceHistory with Id: {priceHistory.Id}. Error: {ex.Message}");
                throw;
            }
        }

        public async Task Delete(int id)
        {
            _logger.LogInformation($"Deleting PriceHistory with Id: {id}");
            try
            {
                var priceHistoryToDelete = await _context.PriceHistories.FindAsync(id);
                if (priceHistoryToDelete != null)
                {
                    _context.PriceHistories.Remove(priceHistoryToDelete);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation($"Successfully deleted PriceHistory with Id: {id}");
                }
                else
                {
                    _logger.LogWarning($"PriceHistory with Id: {id} not found during deletion");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting PriceHistory with Id: {id}. Error: {ex.Message}");
                throw;
            }
        }

        public async Task<PriceHistory?> GetByStockId(int stockId)
        {
            _logger.LogInformation($"Getting latest PriceHistory by Stock Id: {stockId}");
            try
            {
                var priceHistory = await _context.PriceHistories
                    .Where(ph => ph.StockId == stockId)
                    .OrderByDescending(ph => ph.Date)
                    .FirstOrDefaultAsync();

                if (priceHistory != null)
                {
                    _logger.LogInformation($"Successfully retrieved latest PriceHistory by Stock Id: {stockId}");
                }
                else
                {
                    _logger.LogWarning($"No PriceHistory found for Stock Id: {stockId}");
                }
                return priceHistory;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting latest PriceHistory by Stock Id: {stockId}. Error: {ex.Message}");
                throw;
            }
        }

    }
}
