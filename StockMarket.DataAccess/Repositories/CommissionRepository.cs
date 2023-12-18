using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<CommissionRepository> _logger;
        public CommissionRepository(StockMarketContext context, ILogger<CommissionRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Commission?> Get(int id)
        {
            _logger.LogInformation($"Getting Commission by Id: {id}");
            try
            {
                var commission = await _context.Commissions.FindAsync(id);
                if (commission != null)
                {
                    _logger.LogInformation($"Successfully retrieved Commission by Id: {id}");
                }
                else
                {
                    _logger.LogWarning($"Commission with Id: {id} not found");
                }
                return commission;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting Commission by Id: {id}. Error: {ex.Message}");
                throw;
            }
        }

        public async Task<List<Commission>> GetAll()
        {
            _logger.LogInformation("Getting all Commissions");
            try
            {
                var commissions = await _context.Commissions.ToListAsync();
                _logger.LogInformation("Successfully retrieved all Commissions");
                return commissions;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting all Commissions. Error: {ex.Message}");
                throw;
            }
        }

        public async Task UpdateCommissionRate(int id, decimal commissionRate)
        {
            _logger.LogInformation($"Updating Commission Rate for Commission with Id: {id} to Commission Rate: {commissionRate}");
            try
            {
                var commission = await _context.Commissions.FindAsync(id);
                if (commission == null)
                {
                    _logger.LogWarning($"Commission with Id: {id} not found during Commission Rate update");
                    throw new ArgumentException("Invalid Commission Id");
                }

                commission.CommissionRate = Math.Round(commissionRate, 3);
                _context.Entry(commission).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Successfully updated Commission Rate for Commission with Id: {id}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating Commission Rate for Commission with Id: {id}. Error: {ex.Message}");
                throw;
            }
        }

        public async Task<decimal> GetCommissionRateById(int id)
        {
            _logger.LogInformation($"Getting Commission Rate by Id: {id}");
            try
            {
                var commission = await _context.Commissions.FirstOrDefaultAsync(c => c.Id == id);
                if (commission != null)
                {
                    _logger.LogInformation($"Successfully retrieved Commission Rate by Id: {id}");
                }
                else
                {
                    _logger.LogWarning($"Commission with Id: {id} not found during Commission Rate retrieval");
                }
                return commission?.CommissionRate ?? 0;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting Commission Rate by Id: {id}. Error: {ex.Message}");
                throw;
            }
        }
    }
}
