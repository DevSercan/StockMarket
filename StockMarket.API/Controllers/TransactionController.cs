using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StockMarket.Business.DTOs;
using StockMarket.DataAccess.Repositories;
using StockMarket.Entities;

namespace StockMarket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IStockRepository _stockRepository;
        private readonly ICommissionRepository _commissionRepository;
        public TransactionController(ITransactionRepository transactionRepository, IStockRepository stockRepository, ICommissionRepository commissionRepository)
        {
            _transactionRepository = transactionRepository;
            _stockRepository = stockRepository;
            _commissionRepository = commissionRepository;
        }

        [HttpPost("BuyStock/{userId:int}/{stockId:int}/{quantity:int}")]
        public async Task<ActionResult> BuyStock(int userId, int stockId, int quantity)
        {
            DateTime date = DateTime.Now;
            decimal price = await _stockRepository.GetPriceById(stockId);
            decimal commission = await _commissionRepository.GetCommissionRateById(1);
            var newTransaction = new Transaction
            {
                UserId = userId,
                StockId = stockId,
                Type = "Buy",
                Date = date,
                Quantity = quantity,
                Price = price,
                Commission = commission
            };
            var buyingTransaction = await _transactionRepository.Create(newTransaction);
            return CreatedAtAction(nameof(BuyStock), new { id = buyingTransaction.Id }, buyingTransaction);
        }
    }
}
