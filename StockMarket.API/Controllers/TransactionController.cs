using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StockMarket.API.Controllers.Services;
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
        private readonly IUserRepository _userRepository;
        private readonly StockService _stockService;
        public TransactionController(ITransactionRepository transactionRepository, IStockRepository stockRepository, ICommissionRepository commissionRepository, IUserRepository userRepository, StockService stockService)
        {
            _transactionRepository = transactionRepository;
            _stockRepository = stockRepository;
            _commissionRepository = commissionRepository;
            _userRepository = userRepository;
            _stockService = stockService;
        }

        [HttpPost("BuyStock/{userId:int}/{stockId:int}/{quantity:int}")]
        public async Task<ActionResult> BuyStock(int userId, int stockId, int quantity)
        {
            if (userId <= 0 || stockId <= 0 || quantity <= 0)
            {
                return StatusCode(400, "The parameters must be positive value.");
            }
            await _stockService.FetchStockData();
            bool isActive = await _stockRepository.GetActivityById(stockId);
            if (isActive)
            {
                decimal balance = await _userRepository.GetBalanceById(userId);
                decimal price = await _stockRepository.GetPriceById(stockId);
                price = price * quantity;
                int stockQuantity = await _stockRepository.GetQuantityById(stockId);
                if (balance < price)
                {
                    return StatusCode(200, "The user does not have enough money!");
                }
                else if (stockQuantity == 0)
                {
                    return StatusCode(200, "The stock is out of quantity.");
                }
                else if (stockQuantity < quantity)
                {
                    return StatusCode(200, "The stock is less than the quantity you want to buy.");
                }
                DateTime date = DateTime.Now;
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
                await _userRepository.UpdateBalance(userId, balance - price);
                await _stockRepository.UpdateQuantityById(stockId, stockQuantity - quantity);
                return CreatedAtAction(nameof(BuyStock), new { id = buyingTransaction.Id }, buyingTransaction);
            } else
            {
                return StatusCode(200, "The stock is not active!");
            }
            
        }
    }
}
