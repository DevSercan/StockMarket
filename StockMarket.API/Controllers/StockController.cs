using Microsoft.AspNetCore.Authorization;
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
        private readonly IExcelService _excelService;
        private readonly ILogger<StockController> _logger;

        public StockController(IStockRepository stockRepository, IExcelService excelService, ILogger<StockController> logger)
        {
            _stockRepository = stockRepository;
            _excelService = excelService;
            _logger = logger;
        }

        [HttpGet("GetStock/{id}")]
        public async Task<ActionResult<Stock?>> GetStock(int id)
        {
            _logger.LogInformation("'GetStock' method executed.");
            try
            {
                var stock = await _stockRepository.Get(id);

                if (stock == null)
                {
                    _logger.LogWarning("Stock not found. StockId: {StockId}", id);
                    return NotFound($"Stock not found. StockId: {id}");
                }

                _logger.LogInformation("Stock retrieved successfully. StockId: {StockId}", id);
                return stock;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting stock by ID.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

        [Authorize(Roles = "admin")]
        [HttpPost("CreateStock")]
        public async Task<ActionResult<Stock>> CreateStock([FromBody] Stock stock)
        {
            _logger.LogInformation("'CreateStock' method executed.");
            try
            {
                var newStock = await _stockRepository.Create(stock);

                _logger.LogInformation("Stock created successfully. StockId: {StockId}", newStock.Id);
                return CreatedAtAction(nameof(CreateStock), new { id = newStock.Id }, newStock);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during stock creation.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

        [Authorize(Roles = "admin")]
        [HttpPatch("SetActivity/{id:int}/{status:bool}")]
        public async Task<ActionResult<Stock>> SetActivity(int id, bool status)
        {
            _logger.LogInformation("'SetActivity' method executed.");
            try
            {
                await _stockRepository.ChangeStockActivity(id, status);
                var stock = await _stockRepository.Get(id);

                if (stock == null)
                {
                    _logger.LogWarning("Stock not found after activity status update. StockId: {StockId}", id);
                    return NotFound("Stock not found after activity status update");
                }

                _logger.LogInformation("Stock activity status updated successfully. StockId: {StockId}, IsActive: {IsActive}", id, status);
                return Ok(stock);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Invalid argument provided during stock activity status update.");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during stock activity status update.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

        [HttpGet("ExportStocksToExcel/")]
        public async Task<ActionResult> ExportStocksToExcel()
        {
            _logger.LogInformation("Exporting stocks to Excel file has started.");
            try
            {
                var excel = await _excelService.ExportStocksToExcel();
                if (!excel)
                {
                    _logger.LogWarning("Exporting stocks to Excel file failed.");
                    return StatusCode(200, "Exporting stocks to Excel file failed.");
                }
                _logger.LogInformation("Exporting stocks to Excel file was successful.");
                return StatusCode(200, "Exporting stocks to Excel file was successful.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting stocks to Excel.");
                return BadRequest($"Error exporting stocks to Excel: {ex.Message}");
            }
        }
    }
}
