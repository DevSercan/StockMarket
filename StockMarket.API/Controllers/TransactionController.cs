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
        private readonly IPortfolioRepository _portfolioRepository;
        private readonly IBalanceCardRepository _balanceCardRepository;
        private readonly StockService _stockService;
        public TransactionController(ITransactionRepository transactionRepository, IStockRepository stockRepository, ICommissionRepository commissionRepository, IUserRepository userRepository, IPortfolioRepository portfolioRepository, IBalanceCardRepository balanceCardRepository, StockService stockService)
        {
            _transactionRepository = transactionRepository;
            _stockRepository = stockRepository;
            _commissionRepository = commissionRepository;
            _userRepository = userRepository;
            _portfolioRepository = portfolioRepository;
            _balanceCardRepository = balanceCardRepository;
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
                var vaultList = await _userRepository.GetByRoleId(3);
                if (vaultList == null || vaultList.Count == 0)
                {
                    return StatusCode(200, "The vault in the system could not be accessed.");

                }
                decimal balance = await _userRepository.GetBalanceById(userId);
                decimal price = await _stockRepository.GetPriceById(stockId);
                price = price * quantity;
                decimal commissionRate = await _commissionRepository.GetCommissionRateById(1);
                decimal commission = price * commissionRate;
                if (balance < (price + commission))
                {
                    return StatusCode(200, "The user does not have enough money!");
                }
                int stockQuantity = await _stockRepository.GetQuantityById(stockId);
                if (stockQuantity == 0)
                {
                    return StatusCode(200, "The stock is out of quantity.");
                }
                else if (stockQuantity < quantity)
                {
                    return StatusCode(200, "The stock is less than the quantity you want to buy.");
                }
                DateTime date = DateTime.Now;
                var newTransaction = new Transaction
                {
                    UserId = userId,
                    StockId = stockId,
                    Type = "Buy",
                    Date = date,
                    Quantity = quantity,
                    Price = price,
                    Commission = commissionRate
                };
                var buyingTransaction = await _transactionRepository.Create(newTransaction);
                await _userRepository.UpdateBalance(userId, balance - (price + commission));
                var vault = vaultList[0];
                vault.Balance = vault.Balance + commission;
                await _userRepository.Update(vault);
                await _stockRepository.UpdateQuantityById(stockId, stockQuantity - quantity);
                var portfolio = await _portfolioRepository.GetPortfolio(userId, stockId);
                if (portfolio == null)
                {
                    var newPortfolio = new Portfolio
                    {
                        UserId = userId,
                        StockId = stockId,
                        Quantity = quantity
                    };
                    await _portfolioRepository.Create(newPortfolio);
                } else
                {
                    portfolio.Quantity = portfolio.Quantity + quantity;
                    await _portfolioRepository.Update(portfolio);
                }
                return CreatedAtAction(nameof(BuyStock), new { id = buyingTransaction.Id }, buyingTransaction);
            } else
            {
                return StatusCode(200, "The stock is not active!");
            }
        }

        [HttpPost("SellStock/{userId:int}/{stockId:int}/{quantity:int}")]
        public async Task<ActionResult> SellStock(int userId, int stockId, int quantity)
        {
            if (userId <= 0 || stockId <= 0 || quantity <= 0)
            {
                return StatusCode(400, "The parameters must be positive value.");
            }
            await _stockService.FetchStockData();
            bool isActive = await _stockRepository.GetActivityById(stockId);
            if (isActive)
            {
                var vaultList = await _userRepository.GetByRoleId(3);
                if (vaultList == null || vaultList.Count == 0)
                {
                    return StatusCode(200, "The vault in the system could not be accessed.");
                    
                }
                decimal balance = await _userRepository.GetBalanceById(userId);
                decimal price = await _stockRepository.GetPriceById(stockId);
                price = price * quantity;
                var portfolio = await _portfolioRepository.GetPortfolio(userId, stockId);
                if (portfolio == null)
                {
                    return StatusCode(200, "You do not have this stock in your portfolio.");
                }
                int stockQuantity = portfolio.Quantity;
                if (stockQuantity == 0)
                {
                    return StatusCode(200, "You do not have this stock.");
                }
                else if (stockQuantity < quantity)
                {
                    return StatusCode(200, "You do not have this stock for the amount you want to sell.");
                }
                DateTime date = DateTime.Now;
                decimal commissionRate = await _commissionRepository.GetCommissionRateById(1);
                decimal commission = price * commissionRate;
                var newTransaction = new Transaction
                {
                    UserId = userId,
                    StockId = stockId,
                    Type = "Sell",
                    Date = date,
                    Quantity = quantity,
                    Price = price,
                    Commission = commissionRate
                };
                var sellingTransaction = await _transactionRepository.Create(newTransaction);
                await _userRepository.UpdateBalance(userId, balance + (price - commission));
                var vault = vaultList[0];
                vault.Balance = vault.Balance + commission;
                await _userRepository.Update(vault);
                await _stockRepository.UpdateQuantityById(stockId, stockQuantity + quantity);
                portfolio.Quantity = portfolio.Quantity - quantity;
                await _portfolioRepository.Update(portfolio);
                return CreatedAtAction(nameof(SellStock), new { id = sellingTransaction.Id }, sellingTransaction);
            }
            else
            {
                return StatusCode(200, "The stock is not active!");
            }
        }

        [HttpPost("UseBalanceCard/{userId:int}")]
        public async Task<ActionResult> UseBalanceCard(int userId, [FromBody] string balanceCardCode)
        {
            var balanceCard = await _balanceCardRepository.GetByCode(balanceCardCode);
            if (balanceCard == null)
            {
                return StatusCode(200, "This balance card code is invalid.");
            }
            if (balanceCard.Balance == 0)
            {
                return StatusCode(200, "There is no money on this balance card.");
            }
            if (balanceCard.IsUsed == true)
            {
                return StatusCode(200, "This balance card code has already been used.");
            }
            decimal balance = await _userRepository.GetBalanceById(userId);
            await _userRepository.UpdateBalance(userId, balance + balanceCard.Balance);
            balanceCard.UserId = userId;
            balanceCard.IsUsed = true;
            balanceCard.Balance = 0;
            await _balanceCardRepository.Update(balanceCard);
            return StatusCode(200, $"The account has been loaded with 1000 units of balance using the code '{balanceCardCode}'.");
        }
    }
}
