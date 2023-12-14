using Microsoft.AspNetCore.Authorization;
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

        [Authorize(Roles = "admin")]
        [HttpPost("CreateStock")]
        public async Task<ActionResult<Stock>> CreateStock([FromBody] Stock stock)
        {
            var newStock = await _stockRepository.Create(stock);
            return CreatedAtAction(nameof(CreateStock), new { id = newStock.Id }, newStock);
        }

        [Authorize(Roles = "admin")]
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

        [Authorize(Roles = "admin")]
        [HttpDelete("DeleteStock/{id}")]
        public async Task<ActionResult> DeleteStock(int id)
        {
            var stockToDelete = await _stockRepository.Get(id);
            if (stockToDelete == null)
                return BadRequest();
            await _stockRepository.Delete(stockToDelete.Id);

            return NoContent();
        }

        [Authorize(Roles = "admin")]
        [HttpPatch("SetActivity/{id:int}/{status:bool}")]
        public async Task<ActionResult<Stock>> SetActivity(int id, bool status)
        {
            try
            {
                await _stockRepository.ChangeStockActivity(id, status);
                var stock = await _stockRepository.Get(id);
                if (stock == null)
                {
                    return NotFound("Stock not found after activity status update");
                }
                return Ok(stock);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}
