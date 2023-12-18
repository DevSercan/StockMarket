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
    public class PortfolioRepository : IPortfolioRepository
    {
        private readonly StockMarketContext _context;
        private readonly ILogger<PortfolioRepository> _logger;
        public PortfolioRepository(StockMarketContext context, ILogger<PortfolioRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Portfolio?> Get(int id)
        {
            _logger.LogInformation($"Getting Portfolio by Id: {id}");
            try
            {
                var portfolio = await _context.Portfolios.FindAsync(id);
                if (portfolio != null)
                {
                    _logger.LogInformation($"Successfully retrieved Portfolio by Id: {id}");
                }
                else
                {
                    _logger.LogWarning($"Portfolio with Id: {id} not found");
                }
                return portfolio;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting Portfolio by Id: {id}. Error: {ex.Message}");
                throw;
            }
        }

        public async Task<Portfolio> Create(Portfolio portfolio)
        {
            _logger.LogInformation("Creating Portfolio");
            try
            {
                _context.Portfolios.Add(portfolio);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Successfully created Portfolio with Id: {portfolio.Id}");
                return portfolio;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating Portfolio. Error: {ex.Message}");
                throw;
            }
        }

        public async Task Update(Portfolio portfolio)
        {
            _logger.LogInformation($"Updating Portfolio with Id: {portfolio.Id}");
            try
            {
                _context.Entry(portfolio).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Successfully updated Portfolio with Id: {portfolio.Id}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating Portfolio with Id: {portfolio.Id}. Error: {ex.Message}");
                throw;
            }
        }

        public async Task Delete(int id)
        {
            _logger.LogInformation($"Deleting Portfolio with Id: {id}");
            try
            {
                var portfolioToDelete = await _context.Portfolios.FindAsync(id);
                if (portfolioToDelete != null)
                {
                    _context.Portfolios.Remove(portfolioToDelete);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation($"Successfully deleted Portfolio with Id: {id}");
                }
                else
                {
                    _logger.LogWarning($"Portfolio with Id: {id} not found during deletion");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting Portfolio with Id: {id}. Error: {ex.Message}");
                throw;
            }
        }

        public async Task<List<Portfolio>> GetByStockId(int id)
        {
            _logger.LogInformation($"Getting Portfolios by Stock Id: {id}");
            try
            {
                var portfolios = await _context.Portfolios.Where(p => p.StockId == id).ToListAsync();
                _logger.LogInformation($"Successfully retrieved Portfolios by Stock Id: {id}");
                return portfolios;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting Portfolios by Stock Id: {id}. Error: {ex.Message}");
                throw;
            }
        }

        public async Task<List<Portfolio>> GetByUserId(int id)
        {
            _logger.LogInformation($"Getting Portfolios by User Id: {id}");
            try
            {
                var portfolios = await _context.Portfolios.Where(p => p.UserId == id).ToListAsync();
                _logger.LogInformation($"Successfully retrieved Portfolios by User Id: {id}");
                return portfolios;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting Portfolios by User Id: {id}. Error: {ex.Message}");
                throw;
            }
        }

        public async Task<Portfolio?> GetPortfolio(int userId, int stockId)
        {
            _logger.LogInformation($"Getting Portfolio by User Id: {userId} and Stock Id: {stockId}");
            try
            {
                var portfolio = await _context.Portfolios.FirstOrDefaultAsync(p => p.UserId == userId && p.StockId == stockId);
                if (portfolio != null)
                {
                    _logger.LogInformation($"Successfully retrieved Portfolio by User Id: {userId} and Stock Id: {stockId}");
                }
                else
                {
                    _logger.LogWarning($"Portfolio not found by User Id: {userId} and Stock Id: {stockId}");
                }
                return portfolio;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting Portfolio by User Id: {userId} and Stock Id: {stockId}. Error: {ex.Message}");
                throw;
            }
        }
    }
}
