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
    public class UserRepository : IUserRepository
    {
        private readonly StockMarketContext _context;
        private readonly ILogger<UserRepository> _logger;
        public UserRepository(StockMarketContext context, ILogger<UserRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<User?> Get(int id)
        {
            _logger.LogInformation($"Getting User by Id: {id}");
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user != null)
                {
                    _logger.LogInformation($"Successfully retrieved User by Id: {id}");
                }
                else
                {
                    _logger.LogWarning($"User with Id: {id} not found");
                }
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting User by Id: {id}. Error: {ex.Message}");
                throw;
            }
        }

        public async Task Update(User user)
        {
            _logger.LogInformation($"Updating User with Id: {user.Id}");
            try
            {
                _context.Entry(user).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Successfully updated User with Id: {user.Id}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating User with Id: {user.Id}. Error: {ex.Message}");
                throw;
            }
        }

        public async Task<User> Create(User user)
        {
            _logger.LogInformation("Creating new User");
            try
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Successfully created new User with Id: {user.Id}");
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating User. Error: {ex.Message}");
                throw;
            }
        }

        public async Task Delete(int id)
        {
            _logger.LogInformation($"Deleting User with Id: {id}");
            try
            {
                var userToDelete = await _context.Users.FindAsync(id);
                if (userToDelete != null)
                {
                    _context.Users.Remove(userToDelete);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"Successfully deleted User with Id: {id}");
                }
                else
                {
                    _logger.LogWarning($"User with Id: {id} not found");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting User with Id: {id}. Error: {ex.Message}");
                throw;
            }
        }

        public async Task<User?> GetByEmail(string email)
        {
            _logger.LogInformation($"Getting User by Email: {email}");
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
                if (user != null)
                {
                    _logger.LogInformation($"Successfully retrieved User by Email: {email}");
                }
                else
                {
                    _logger.LogWarning($"User with Email: {email} not found");
                }
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting User by Email: {email}. Error: {ex.Message}");
                throw;
            }
        }

        public async Task ChangeUserRole(int userId, int roleId)
        {
            _logger.LogInformation($"Changing User Role. UserId: {userId}, New RoleId: {roleId}");
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    throw new ArgumentException("Invalid User Id");
                }
                user.RoleId = roleId;
                _context.Entry(user).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Successfully changed User Role. UserId: {userId}, New RoleId: {roleId}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error changing User Role. UserId: {userId}, New RoleId: {roleId}. Error: {ex.Message}");
                throw;
            }
        }

        public async Task UpdateBalance(int userId, decimal balance)
        {
            _logger.LogInformation($"Updating User Balance. UserId: {userId}, New Balance: {balance}");
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    throw new ArgumentException("Invalid User Id");
                }
                user.Balance = balance;
                _context.Entry(user).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Successfully updated User Balance. UserId: {userId}, New Balance: {balance}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating User Balance. UserId: {userId}, New Balance: {balance}. Error: {ex.Message}");
                throw;
            }
        }

        public async Task<decimal> GetBalanceById(int id)
        {
            _logger.LogInformation($"Getting User Balance by Id: {id}");
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(s => s.Id == id);
                if (user != null)
                {
                    _logger.LogInformation($"Successfully retrieved User Balance by Id: {id}");
                    return user.Balance;
                }
                else
                {
                    _logger.LogWarning($"User with Id: {id} not found");
                    return 0m;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting User Balance by Id: {id}. Error: {ex.Message}");
                throw;
            }
        }

        public async Task<List<User>> GetByRoleId(int roleId)
        {
            _logger.LogInformation($"Getting Users by RoleId: {roleId}");
            try
            {
                var users = await _context.Users.Where(u => u.RoleId == roleId).ToListAsync();
                _logger.LogInformation($"Successfully retrieved Users by RoleId: {roleId}");
                return users;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting Users by RoleId: {roleId}. Error: {ex.Message}");
                throw;
            }
        }
    }

}
