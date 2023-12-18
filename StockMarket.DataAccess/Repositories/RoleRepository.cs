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
    public class RoleRepository : IRoleRepository
    {
        private readonly StockMarketContext _context;
        private readonly ILogger<RoleRepository> _logger;
        public RoleRepository(StockMarketContext context, ILogger<RoleRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Role?> Get(int id)
        {
            _logger.LogInformation($"Getting Role by Id: {id}");
            try
            {
                var role = await _context.Roles.FindAsync(id);
                if (role != null)
                {
                    _logger.LogInformation($"Successfully retrieved Role by Id: {id}");
                }
                else
                {
                    _logger.LogWarning($"Role with Id: {id} not found");
                }
                return role;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting Role by Id: {id}. Error: {ex.Message}");
                throw;
            }
        }
    }
}
