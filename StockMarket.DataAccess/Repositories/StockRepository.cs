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
    public class StockRepository : IStockRepository
    {
        private readonly StockMarketContext _context;
        private readonly ILogger<StockRepository> _logger;
        public StockRepository(StockMarketContext context, ILogger<StockRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Stock?> Get(int id)
        {
            _logger.LogInformation($"Getting Stock by Id: {id}");
            try
            {
                var stock = await _context.Stocks.FindAsync(id);
                if (stock != null)
                {
                    _logger.LogInformation($"Successfully retrieved Stock by Id: {id}");
                }
                else
                {
                    _logger.LogWarning($"Stock with Id: {id} not found");
                }
                return stock;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting Stock by Id: {id}. Error: {ex.Message}");
                throw;
            }
        }

        public async Task<List<Stock>> GetAll()
        {
            _logger.LogInformation("Getting all stocks.");
            try
            {
                var stocks = await _context.Stocks.ToListAsync();
                _logger.LogInformation($"Successfully retrieved all stocks. Count: {stocks.Count}");
                return stocks;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting all stocks. Error: {ex.Message}");
                throw;
            }
        }

        public async Task<Stock> Create(Stock stock)
        {
            _logger.LogInformation($"Creating new Stock with Name: {stock.Name}");
            try
            {
                _context.Stocks.Add(stock);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Stock with Name: {stock.Name} successfully created.");
                return stock;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating Stock. Error: {ex.Message}");
                throw;
            }
        }

        public async Task Update(Stock stock)
        {
            _logger.LogInformation($"Updating Stock with Id: {stock.Id}");
            try
            {
                _context.Entry(stock).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Stock with Id: {stock.Id} successfully updated.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating Stock with Id: {stock.Id}. Error: {ex.Message}");
                throw;
            }
        }

        public async Task Delete(int id)
        {
            _logger.LogInformation($"Deleting Stock with Id: {id}");
            try
            {
                var stockToDelete = await _context.Stocks.FindAsync(id);
                if (stockToDelete != null)
                {
                    _context.Stocks.Remove(stockToDelete);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"Stock with Id: {id} successfully deleted.");
                }
                else
                {
                    _logger.LogWarning($"Stock with Id: {id} not found for deletion");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting Stock with Id: {id}. Error: {ex.Message}");
                throw;
            }
        }

        public async Task ChangeStockActivity(int id, bool isActive)
        {
            _logger.LogInformation($"Changing Stock Activity for Stock Id: {id} to IsActive: {isActive}");
            try
            {
                var stock = await _context.Stocks.FindAsync(id);
                if (stock == null)
                {
                    _logger.LogWarning($"Stock with Id: {id} not found. Unable to change activity.");
                    throw new ArgumentException("Invalid Stock Id");
                }

                stock.IsActive = isActive;
                _context.Entry(stock).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Stock Activity for Stock Id: {id} successfully changed to IsActive: {isActive}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error changing Stock Activity for Stock Id: {id}. Error: {ex.Message}");
                throw;
            }
        }

        public async Task<Stock?> GetByName(string name)
        {
            _logger.LogInformation($"Getting Stock by Name: {name}");
            try
            {
                var stock = await _context.Stocks.FirstOrDefaultAsync(s => s.Name == name);
                if (stock != null)
                {
                    _logger.LogInformation($"Successfully retrieved Stock by Name: {name}");
                }
                else
                {
                    _logger.LogWarning($"Stock with Name: {name} not found");
                }
                return stock;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting Stock by Name: {name}. Error: {ex.Message}");
                throw;
            }
        }

        public async Task UpdatePriceByName(string name, decimal price)
        {
            _logger.LogInformation($"Updating Stock Price by Name: {name} to Price: {price}");
            try
            {
                var stock = await _context.Stocks.FirstOrDefaultAsync(s => s.Name == name);
                if (stock == null)
                {
                    _logger.LogWarning($"Stock with Name: {name} not found. Unable to update price.");
                    throw new ArgumentException("Invalid Stock Name");
                }

                stock.Price = price;
                _context.Entry(stock).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Stock Price for Stock Name: {name} successfully updated to Price: {price}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating Stock Price for Stock Name: {name}. Error: {ex.Message}");
                throw;
            }
        }

        public async Task UpdateQuantityById(int id, int quantity)
        {
            _logger.LogInformation($"Updating Stock Quantity by Id: {id} to Quantity: {quantity}");
            try
            {
                var stock = await _context.Stocks.FirstOrDefaultAsync(s => s.Id == id);
                if (stock == null)
                {
                    _logger.LogWarning($"Stock with Id: {id} not found. Unable to update quantity.");
                    throw new ArgumentException("Invalid Stock Id");
                }

                stock.Quantity = quantity;
                _context.Entry(stock).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Stock Quantity for Stock Id: {id} successfully updated to Quantity: {quantity}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating Stock Quantity for Stock Id: {id}. Error: {ex.Message}");
                throw;
            }
        }

        public async Task<decimal> GetPriceById(int id)
        {
            _logger.LogInformation($"Getting Stock Price by Id: {id}");
            try
            {
                var stock = await _context.Stocks.FirstOrDefaultAsync(s => s.Id == id);
                if (stock != null)
                {
                    _logger.LogInformation($"Successfully retrieved Stock Price by Id: {id}. Price: {stock.Price}");
                    return stock.Price;
                }
                else
                {
                    _logger.LogWarning($"Stock with Id: {id} not found. Unable to retrieve Price.");
                    return 0m;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting Stock Price by Id: {id}. Error: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> GetActivityById(int id)
        {
            _logger.LogInformation($"Getting Stock Activity by Id: {id}");
            try
            {
                var stock = await _context.Stocks.FirstOrDefaultAsync(s => s.Id == id);
                if (stock != null)
                {
                    _logger.LogInformation($"Successfully retrieved Stock Activity by Id: {id}. IsActive: {stock.IsActive}");
                    return stock.IsActive;
                }
                else
                {
                    _logger.LogWarning($"Stock with Id: {id} not found. Unable to retrieve Activity.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting Stock Activity by Id: {id}. Error: {ex.Message}");
                throw;
            }
        }

        public async Task<int> GetQuantityById(int id)
        {
            _logger.LogInformation($"Getting Stock Quantity by Id: {id}");
            try
            {
                var stock = await _context.Stocks.FirstOrDefaultAsync(s => s.Id == id);
                if (stock != null)
                {
                    _logger.LogInformation($"Successfully retrieved Stock Quantity by Id: {id}. Quantity: {stock.Quantity}");
                    return stock.Quantity;
                }
                else
                {
                    _logger.LogWarning($"Stock with Id: {id} not found. Unable to retrieve Quantity.");
                    return 0;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting Stock Quantity by Id: {id}. Error: {ex.Message}");
                throw;
            }
        }
    }
}
