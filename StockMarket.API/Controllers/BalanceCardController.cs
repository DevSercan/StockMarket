using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StockMarket.Business.DTOs;
using StockMarket.DataAccess.Repositories;
using StockMarket.Entities;

namespace StockMarket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BalanceCardController : ControllerBase
    {
        private readonly IBalanceCardRepository _balanceCardRepository;
        private readonly ILogger<BalanceCardController> _logger;
        public BalanceCardController(IBalanceCardRepository balanceCardRepository, ILogger<BalanceCardController> logger)
        {
            _balanceCardRepository = balanceCardRepository;
            _logger = logger;
        }

        [Authorize(Roles = "admin")]
        [HttpPost("CreateBalanceCard/")]
        public async Task<ActionResult<BalanceCard>> CreateBalanceCard([FromBody] BalanceCardDTO balanceCard)
        {
            _logger.LogInformation("'CreateBalanceCard' method executed.");
            try
            {
                var newBalanceCard = new BalanceCard
                {
                    UserId = 0,
                    Code = balanceCard.Code,
                    Balance = balanceCard.Balance
                };
                var createdBalanceCard = await _balanceCardRepository.Create(newBalanceCard);
                _logger.LogInformation("Balance card created successfully. BalanceCardId: {BalanceCardId}", createdBalanceCard.Id);
                return CreatedAtAction(nameof(CreateBalanceCard), new { id = createdBalanceCard.Id }, createdBalanceCard);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database update exception occurred during balance card creation.");
                return StatusCode(500, "An error occurred while updating the database. Please try again.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during balance card creation.");
                return StatusCode(500, "An unexpected error occurred. Please try again.");
            }
        }

        [Authorize(Roles = "admin")]
        [HttpPost("GetBalanceCards/")]
        public async Task<ActionResult<List<BalanceCard>>> GetBalanceCards()
        {
            _logger.LogInformation("'GetBalanceCards' method executed.");
            try
            {
                var balanceCards = await _balanceCardRepository.GetAll();
                _logger.LogInformation("All balance cards have been fetched.");
                return balanceCards;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting balance cards.");
                return StatusCode(500, "An unexpected error occurred. Please try again.");
            }
        }

        [Authorize(Roles = "admin")]
        [HttpPost("UpdateBalance/{balance:decimal}")]
        public async Task<ActionResult> UpdateBalance([FromBody] string balanceCardCode, decimal balance)
        {
            _logger.LogInformation("'UpdateBalance' method executed.");
            try
            {
                var balanceCard = await _balanceCardRepository.GetByCode(balanceCardCode);
                if (balanceCard == null)
                {
                    _logger.LogInformation("Invalid balance card code: {BalanceCardCode}", balanceCardCode);
                    return StatusCode(200, "This balance card code is invalid.");
                }
                if (balanceCard.IsUsed == true)
                {
                    _logger.LogInformation("Balance card with code '{BalanceCardCode}' has already been used.", balanceCardCode);
                    return StatusCode(200, "This balance card code has already been used.");
                }
                balanceCard.Balance = balance;
                await _balanceCardRepository.Update(balanceCard);
                _logger.LogInformation("Balance updated to '{Balance}' for the balance card with code '{BalanceCardCode}'.", balance, balanceCardCode);
                return StatusCode(200, $"Balance updated to '{balance}' for the balance card with code '{balanceCardCode}'.");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database update exception occurred during balance update.");
                return StatusCode(500, "An error occurred while updating the database. Please try again.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during balance update.");
                return StatusCode(500, "An unexpected error occurred. Please try again.");
            }
        }
    }
}
