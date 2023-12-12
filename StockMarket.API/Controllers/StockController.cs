using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StockMarket.API.Controllers.Services;
using StockMarket.DataAccess.Repositories;
using StockMarket.Entities;

namespace StockMarket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockController : ControllerBase
    {
        private readonly IStockRepository _stockRepository;

        public StockController(IStockRepository stockRepository)
        {
            _stockRepository = stockRepository;
        }

        [HttpGet("GetStock/{id}")]
        public async Task<ActionResult<Stock?>> GetStock(int id)
        {
            return await _stockRepository.Get(id);
        }

        [HttpPost("CreateStock")]
        public async Task<ActionResult<Stock>> CreateStock([FromBody] Stock stock)
        {
            var newStock = await _stockRepository.Create(stock);
            return CreatedAtAction(nameof(CreateStock), new { id = newStock.Id }, newStock);
        }

        [HttpPut("UpdateStock/{id}")]
        public async Task<ActionResult> UpdateStock(int id, [FromBody] Stock stock)
        {
            if (id != stock.Id)
            {
                return BadRequest();
            }
            await _stockRepository.Update(stock);

            return NoContent();
        }

        [HttpDelete("DeleteStock/{id}")]
        public async Task<ActionResult> DeleteStock(int id)
        {
            var stockToDelete = await _stockRepository.Get(id);
            if (stockToDelete == null)
                return BadRequest();
            await _stockRepository.Delete(stockToDelete.Id);

            return NoContent();
        }
    }
}
