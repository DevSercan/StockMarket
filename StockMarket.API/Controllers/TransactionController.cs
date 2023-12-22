using Microsoft.AspNetCore.Mvc;
using StockMarket.API.Controllers.Services;
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
        private readonly ILogger<TransactionController> _logger;
        private readonly IStockService _stockService;
        private readonly IExcelService _excelService;
        public TransactionController(ITransactionRepository transactionRepository, IStockRepository stockRepository, ICommissionRepository commissionRepository, IUserRepository userRepository, IPortfolioRepository portfolioRepository, IBalanceCardRepository balanceCardRepository, ILogger<TransactionController> logger, IStockService stockService, IExcelService excelService)
        {
            _transactionRepository = transactionRepository;
            _stockRepository = stockRepository;
            _commissionRepository = commissionRepository;
            _userRepository = userRepository;
            _portfolioRepository = portfolioRepository;
            _balanceCardRepository = balanceCardRepository;
            _logger = logger;
            _stockService = stockService;
            _excelService = excelService;
        }

        private async Task<ActionResult> IsStockActive(int stockId)
        {
            bool isActive = await _stockRepository.GetActivityById(stockId);
            if (!isActive)
            {
                _logger.LogWarning("The stock is not active. StockId: {StockId}", stockId);
                return NotFound("The stock is not active");
            }
            return null;
        }

        private ActionResult ValidateParameters(int userId, int stockId, int quantity)
        {
            if (userId <= 0 || stockId <= 0 || quantity <= 0)
            {
                _logger.LogWarning("Invalid parameters. UserId: {UserId}, StockId: {StockId}, Quantity: {Quantity}", userId, stockId, quantity);
                return BadRequest("The parameters must be positive values");
            }
            return null;
        }

        private async Task<User> GetVaultUser()
        {
            var vaultList = await _userRepository.GetByRoleId(3);
            if (vaultList == null || vaultList.Count == 0)
            {
                _logger.LogError("The vault in the system could not be accessed.");
                return null;
            }
            return vaultList[0];
        }

        private Transaction CreateTransaction(int userId, int stockId, string transactionType, int quantity, decimal price, decimal commissionRate)
        {
            DateTime date = DateTime.Now;
            return new Transaction
            {
                UserId = userId,
                StockId = stockId,
                Type = transactionType,
                Date = date,
                Quantity = quantity,
                Price = price,
                Commission = commissionRate
            };
        }

        [HttpPost("BuyStock/{userId:int}/{stockId:int}/{quantity:int}")]
        public async Task<ActionResult> BuyStock(int userId, int stockId, int quantity)
        {
            _logger.LogInformation("'BuyStock' method executed.");
            try
            {
                var parameterValidationResult = ValidateParameters(userId, stockId, quantity);
                if (parameterValidationResult != null)
                    return parameterValidationResult;

                await _stockService.FetchStockData();

                var stockStatus = await IsStockActive(stockId);
                if (stockStatus != null)
                    return stockStatus;

                var vaultUser = await GetVaultUser();
                if (vaultUser == null)
                    return StatusCode(StatusCodes.Status500InternalServerError, "The vault in the system could not be accessed");

                decimal balance = await _userRepository.GetBalanceById(userId);
                decimal price = await _stockRepository.GetPriceById(stockId);
                price = price * quantity;
                decimal commissionRate = await _commissionRepository.GetCommissionRateById(1);
                decimal commission = price * commissionRate;

                if (balance < (price + commission))
                {
                    _logger.LogWarning("The user does not have enough money. UserId: {UserId}, Balance: {Balance}, RequiredAmount: {RequiredAmount}", userId, balance, (price + commission));
                    return Forbid("The user does not have enough money");
                }

                int stockQuantity = await _stockRepository.GetQuantityById(stockId);
                if (stockQuantity == 0)
                {
                    _logger.LogWarning("The stock is out of quantity. StockId: {StockId}", stockId);
                    return Conflict("The stock is out of quantity");
                }
                else if (stockQuantity < quantity)
                {
                    _logger.LogWarning("The stock is less than the quantity you want to buy. StockId: {StockId}, RequestedQuantity: {RequestedQuantity}, AvailableQuantity: {AvailableQuantity}", stockId, quantity, stockQuantity);
                    return Conflict("The stock is less than the quantity you want to buy");
                }

                var newTransaction = CreateTransaction(userId, stockId, "Buy", quantity, price, commissionRate);

                var buyingTransaction = await _transactionRepository.Create(newTransaction);

                await _userRepository.UpdateBalance(userId, balance - (price + commission));

                vaultUser.Balance = vaultUser.Balance + commission;
                await _userRepository.Update(vaultUser);
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
                }
                else
                {
                    portfolio.Quantity = portfolio.Quantity + quantity;
                    await _portfolioRepository.Update(portfolio);
                }

                _logger.LogInformation("Stock purchased successfully. TransactionId: {TransactionId}", buyingTransaction.Id);
                return CreatedAtAction(nameof(BuyStock), new { id = buyingTransaction.Id }, buyingTransaction);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during stock purchase.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

        [HttpPost("SellStock/{userId:int}/{stockId:int}/{quantity:int}")]
        public async Task<ActionResult> SellStock(int userId, int stockId, int quantity)
        {
            _logger.LogInformation("'SellStock' method executed.");
            try
            {
                var parameterValidationResult = ValidateParameters(userId, stockId, quantity);
                if (parameterValidationResult != null)
                    return parameterValidationResult;

                await _stockService.FetchStockData();

                var stockStatus = await IsStockActive(stockId);
                if (stockStatus != null)
                    return stockStatus;

                var vaultUser = await GetVaultUser();
                if (vaultUser == null)
                    return StatusCode(StatusCodes.Status500InternalServerError, "The vault in the system could not be accessed");

                decimal balance = await _userRepository.GetBalanceById(userId);
                decimal price = await _stockRepository.GetPriceById(stockId);
                price = price * quantity;

                var portfolio = await _portfolioRepository.GetPortfolio(userId, stockId);
                if (portfolio == null)
                {
                    _logger.LogWarning("You do not have this stock in your portfolio. UserId: {UserId}, StockId: {StockId}", userId, stockId);
                    return NotFound("You do not have this stock in your portfolio");
                }

                int portfolioQuantity = portfolio.Quantity;

                if (portfolioQuantity == 0)
                {
                    _logger.LogWarning("You do not have this stock. UserId: {UserId}, StockId: {StockId}", userId, stockId);
                    return NotFound("You do not have this stock");
                }
                else if (portfolioQuantity < quantity)
                {
                    _logger.LogWarning("You do not have this stock for the amount you want to sell. UserId: {UserId}, StockId: {StockId}, RequestedQuantity: {RequestedQuantity}, AvailableQuantity: {AvailableQuantity}", userId, stockId, quantity, portfolioQuantity);
                    return BadRequest("You do not have this stock for the amount you want to sell");
                }

                decimal commissionRate = await _commissionRepository.GetCommissionRateById(1);
                decimal commission = price * commissionRate;

                var newTransaction = CreateTransaction(userId, stockId, "Sell", quantity, price, commissionRate);

                var sellingTransaction = await _transactionRepository.Create(newTransaction);

                await _userRepository.UpdateBalance(userId, balance + (price - commission));

                vaultUser.Balance = vaultUser.Balance + commission;
                await _userRepository.Update(vaultUser);

                int stockQuantity = await _stockRepository.GetQuantityById(stockId);
                await _stockRepository.UpdateQuantityById(stockId, stockQuantity + quantity);

                portfolio.Quantity = portfolio.Quantity - quantity;
                await _portfolioRepository.Update(portfolio);

                _logger.LogInformation("Stock sold successfully. TransactionId: {TransactionId}", sellingTransaction.Id);
                return CreatedAtAction(nameof(SellStock), new { id = sellingTransaction.Id }, sellingTransaction);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during stock sale.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }


        [HttpPost("UseBalanceCard/{userId:int}")]
        public async Task<ActionResult> UseBalanceCard(int userId, [FromBody] string balanceCardCode)
        {
            _logger.LogInformation("'UseBalanceCard' method executed.");
            try
            {
                var balanceCard = await _balanceCardRepository.GetByCode(balanceCardCode);

                if (balanceCard == null)
                {
                    _logger.LogWarning("Invalid balance card code. UserId: {UserId}, BalanceCardCode: {BalanceCardCode}", userId, balanceCardCode);
                    return BadRequest("This balance card code is invalid");
                }

                if (balanceCard.IsUsed == true)
                {
                    _logger.LogWarning("Balance card has already been used. UserId: {UserId}, BalanceCardCode: {BalanceCardCode}", userId, balanceCardCode);
                    return Conflict("This balance card code has already been used");
                }

                if (balanceCard.Balance == 0)
                {
                    _logger.LogWarning("Balance card has no money. UserId: {UserId}, BalanceCardCode: {BalanceCardCode}", userId, balanceCardCode);
                    return Forbid("There is no money on this balance card");
                }

                decimal balance = await _userRepository.GetBalanceById(userId);

                await _userRepository.UpdateBalance(userId, balance + balanceCard.Balance);

                var balanceAmount = balanceCard.Balance;

                balanceCard.UserId = userId;
                balanceCard.IsUsed = true;
                balanceCard.Balance = 0;

                await _balanceCardRepository.Update(balanceCard);

                _logger.LogInformation("Balance card used successfully. UserId: {UserId}, BalanceCardCode: {BalanceCardCode}", userId, balanceCardCode);
                return StatusCode(200, $"The account has been loaded with {balanceAmount} units of balance using the code '{balanceCardCode}'");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during balance card usage.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

        [HttpGet("ExportTransactionsToExcel/{userId:int}")]
        public async Task<ActionResult> ExportTransactionsToExcel(int userId)
        {
            _logger.LogInformation("Exporting transactions to Excel file has started.");
            try
            {
                var excel = await _excelService.ExportTransactionsToExcel(userId);
                if (!excel)
                {
                    _logger.LogWarning("Exporting transactions to Excel file failed.");
                    return StatusCode(StatusCodes.Status500InternalServerError, "Exporting transactions to Excel file failed");
                }
                _logger.LogInformation("Exporting transactions to Excel file was successful.");
                return StatusCode(200, "Exporting transactions to Excel file was successful");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting transactions to Excel.");
                return BadRequest($"Error exporting transactions to Excel: {ex.Message}");
            }
        }
    }
}
