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
    public class BalanceCardRepository : IBalanceCardRepository
    {
        private readonly StockMarketContext _context;
        private readonly ILogger<BalanceCardRepository> _logger;
        public BalanceCardRepository(StockMarketContext context, ILogger<BalanceCardRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<BalanceCard?> Get(int id)
        {
            _logger.LogInformation($"Getting BalanceCard with Id: {id}");
            try
            {
                var result = await _context.BalanceCards.FindAsync(id);
                _logger.LogInformation($"Successfully retrieved BalanceCard with Id: {id}");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting BalanceCard with Id: {id}. Error: {ex.Message}");
                throw;
            }
        }

        public async Task<List<BalanceCard>> GetAll()
        {
            _logger.LogInformation("Getting all BalanceCards");
            try
            {
                var result = await _context.BalanceCards.ToListAsync();
                _logger.LogInformation($"Successfully retrieved {result.Count} BalanceCards");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting all BalanceCards. Error: {ex.Message}");
                throw;
            }
        }

        public async Task Update(BalanceCard balanceCard)
        {
            _logger.LogInformation($"Updating BalanceCard with Id: {balanceCard.Id}");
            try
            {
                _context.Entry(balanceCard).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Successfully updated BalanceCard with Id: {balanceCard.Id}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating BalanceCard with Id: {balanceCard.Id}. Error: {ex.Message}");
                throw;
            }
        }

        public async Task<BalanceCard> Create(BalanceCard balanceCard)
        {
            _logger.LogInformation("Creating a new BalanceCard");
            try
            {
                _context.BalanceCards.Add(balanceCard);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Successfully created a new BalanceCard with Id: {balanceCard.Id}");
                return balanceCard;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating a new BalanceCard. Error: {ex.Message}");
                throw;
            }
        }

        public async Task Delete(int id)
        {
            _logger.LogInformation($"Deleting BalanceCard with Id: {id}");
            try
            {
                var balanceCardToDelete = await _context.BalanceCards.FindAsync(id);
                if (balanceCardToDelete != null)
                {
                    _context.BalanceCards.Remove(balanceCardToDelete);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"Successfully deleted BalanceCard with Id: {id}");
                }
                else
                {
                    _logger.LogWarning($"BalanceCard with Id: {id} not found during deletion");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting BalanceCard with Id: {id}. Error: {ex.Message}");
                throw;
            }
        }

        public async Task ChangeCardUsageById(int id, bool isUsed)
        {
            _logger.LogInformation($"Changing card usage for BalanceCard with Id: {id} to IsUsed: {isUsed}");
            try
            {
                var balanceCard = await _context.BalanceCards.FindAsync(id);
                if (balanceCard == null)
                {
                    _logger.LogWarning($"BalanceCard with Id: {id} not found during card usage change");
                    throw new ArgumentException("Invalid Balance Card Id");
                }

                balanceCard.IsUsed = isUsed;
                _context.Entry(balanceCard).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Successfully changed card usage for BalanceCard with Id: {id}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error changing card usage for BalanceCard with Id: {id}. Error: {ex.Message}");
                throw;
            }
        }

        public async Task UpdateUserId(int id, int userId)
        {
            _logger.LogInformation($"Updating UserId for BalanceCard with Id: {id} to UserId: {userId}");
            try
            {
                var balanceCard = await _context.BalanceCards.FindAsync(id);
                if (balanceCard == null)
                {
                    _logger.LogWarning($"BalanceCard with Id: {id} not found during UserId update");
                    throw new ArgumentException("Invalid Balance Card Id");
                }

                balanceCard.UserId = userId;
                _context.Entry(balanceCard).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Successfully updated UserId for BalanceCard with Id: {id}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating UserId for BalanceCard with Id: {id}. Error: {ex.Message}");
                throw;
            }
        }

        public async Task<List<BalanceCard>> GetByUserId(int userId)
        {
            _logger.LogInformation($"Getting BalanceCards for UserId: {userId}");
            try
            {
                var balanceCards = await _context.BalanceCards.Where(bc => bc.UserId == userId).ToListAsync();
                _logger.LogInformation($"Successfully retrieved BalanceCards for UserId: {userId}");
                return balanceCards;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting BalanceCards for UserId: {userId}. Error: {ex.Message}");
                throw;
            }
        }

        public async Task<BalanceCard?> GetByCode(string code)
        {
            _logger.LogInformation($"Getting BalanceCard by code: {code}");
            try
            {
                var balanceCard = await _context.BalanceCards.FirstOrDefaultAsync(bc => bc.Code == code);
                if (balanceCard != null)
                {
                    _logger.LogInformation($"Successfully retrieved BalanceCard by code: {code}");
                }
                else
                {
                    _logger.LogWarning($"BalanceCard with code: {code} not found");
                }
                return balanceCard;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting BalanceCard by code: {code}. Error: {ex.Message}");
                throw;
            }
        }
    }
}
