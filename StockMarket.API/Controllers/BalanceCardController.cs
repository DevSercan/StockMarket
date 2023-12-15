using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
        public BalanceCardController(IBalanceCardRepository balanceCardRepository)
        {
            _balanceCardRepository = balanceCardRepository;
        }

        [Authorize(Roles = "admin")]
        [HttpPost("CreateBalanceCard/")]
        public async Task<ActionResult<BalanceCard>> CreateBalanceCard([FromBody] BalanceCardDTO balanceCard)
        {
            try
            {
                var newBalanceCard = new BalanceCard
                {
                    UserId = 0,
                    Code = balanceCard.Code,
                    Balance = balanceCard.Balance
                };
                var createdBalanceCard = await _balanceCardRepository.Create(newBalanceCard);
                return CreatedAtAction(nameof(CreateBalanceCard), new { id = createdBalanceCard.Id }, createdBalanceCard);
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, "An error occurred while updating the database. Please try again.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occurred. Please try again.");
            }

        }

        [Authorize(Roles = "admin")]
        [HttpPost("GetBalanceCards/")]
        public async Task<ActionResult<List<BalanceCard>>> GetBalanceCards()
        {
            return await _balanceCardRepository.GetAll();
        }

        [Authorize(Roles = "admin")]
        [HttpPost("UpdateBalance/{balance:decimal}")]
        public async Task<ActionResult> UpdateBalance([FromBody] string balanceCardCode, decimal balance)
        {
            try
            {
                var balanceCard = await _balanceCardRepository.GetByCode(balanceCardCode);
                if (balanceCard == null)
                {
                    return StatusCode(200, "This balance card code is invalid.");
                }
                if (balanceCard.IsUsed == true)
                {
                    return StatusCode(200, "This balance card code has already been used.");
                }
                balanceCard.Balance = balance;
                await _balanceCardRepository.Update(balanceCard);
                return StatusCode(200, $"Balance updated to '{balance}' for the balance card with code '{balanceCardCode}'.");
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, "An error occurred while updating the database. Please try again.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occurred. Please try again.");
            }
        }
    }
}
