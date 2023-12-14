using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StockMarket.Business.DTOs;
using StockMarket.DataAccess.Repositories;
using StockMarket.Entities;

namespace StockMarket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PortfolioController : ControllerBase
    {
        private readonly IPortfolioRepository _portfolioRepository;
        public PortfolioController(IPortfolioRepository portfolioRepository)
        {
            _portfolioRepository = portfolioRepository;
        }

        [Authorize(Roles = "admin")]
        [HttpPost("CreatePortfolio")]
        public async Task<ActionResult<Portfolio>> CreatePortfolio([FromBody] PortfolioDTO portfolio)
        {
            var newPortfolio = new Portfolio
            {
                UserId = portfolio.UserId,
                StockId = portfolio.StockId
            };
            var createdPortfolio = await _portfolioRepository.Create(newPortfolio);
            return CreatedAtAction(nameof(CreatePortfolio), new { id = createdPortfolio.Id }, createdPortfolio);
        }

        [Authorize(Roles = "admin")]
        [HttpGet("GetByUserId/{userId}")]
        public async Task<ActionResult<Portfolio?>> GetByUserId(int id)
        {
            var portfolio = await _portfolioRepository.GetByUserId(id);
            if (portfolio == null)
            {
                return NotFound($"Portfolio not found for user with ID: {id}");
            }
            return portfolio;
        }

        [Authorize(Roles = "admin")]
        [HttpGet("GetByStockId/{stockId}")]
        public async Task<ActionResult<Portfolio?>> GetByStockId(int id)
        {
            var portfolio = await _portfolioRepository.GetByStockId(id);
            if (portfolio == null)
            {
                return NotFound($"Portfolio not found for stock with ID: {id}");
            }
            return portfolio;
        }

        [Authorize(Roles = "admin")]
        [HttpGet("GetPortfolio/{id}")]
        public async Task<ActionResult<Portfolio?>> GetPortfolio(int id)
        {
            return await _portfolioRepository.Get(id);
        }
    }
}
